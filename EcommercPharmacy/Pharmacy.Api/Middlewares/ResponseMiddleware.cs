using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pharmacy.Api.Middlewares
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseMiddleware> _logger;

        public ResponseMiddleware(RequestDelegate next, ILogger<ResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // احفظ الستريم الأصلي
            var originalBodyStream = context.Response.Body;

            // استخدم MemoryStream مؤقت لالتقاط أي شيء يُكتب للرد
            await using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                // نفّذ باقي الـ pipeline
                await _next(context);

                // نرجّع مؤشر الذاكرة لبداية ونقرأ المحتوى
                memoryStream.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(memoryStream, Encoding.UTF8).ReadToEndAsync();

                // لو الرد عبارة عن ProblemDetails (جاء من ExceptionMiddleware) — لا نعدله، نعيده كما هو
                if (IsProblemDetailsResponse(bodyText))
                {
                    // قبل إعادة النسخ، ارجع البادي الأصلي
                    context.Response.Body = originalBodyStream;
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalBodyStream);
                    return;
                }

                // لو الرد موجود وكان على شكل JSON وداخلياً ResultDto موجود (succeeded) — نضيف statusCode + timestamp
                string output = bodyText;
                if (!string.IsNullOrWhiteSpace(bodyText))
                {
                    try
                    {
                        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var doc = JsonDocument.Parse(bodyText);

                        // إذا الرد هو ResultDto (فيش خاصية succeeded) — نحرص على اضافة statusCode+timestamp
                        if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("succeeded", out _))
                        {
                            // نعيد بناء JSON كـ Dictionary لاضافة الحقول
                            var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(bodyText, opts) ?? new();
                            dict["statusCode"] = context.Response.StatusCode;
                            dict["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'");
                            output = JsonSerializer.Serialize(dict, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
                        }
                        else
                        {
                            var dataObj = JsonSerializer.Deserialize<object>(bodyText, opts);

                            // ✅ لو الرد أصلاً فيه statusCode و message → لا نغلفه مرة تانية
                            if (doc.RootElement.TryGetProperty("statusCode", out _) &&
                                doc.RootElement.TryGetProperty("message", out _))
                            {
                                output = bodyText; // نسيبه زي ما هو
                            }
                            else
                            {
                                var wrapped = new
                                {
                                    statusCode = context.Response.StatusCode,
                                    success = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
                                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                                    message = GetDefaultMessage(context.Response.StatusCode),
                                    data = dataObj
                                };
                                output = JsonSerializer.Serialize(wrapped, new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                    WriteIndented = true
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "ResponseMiddleware: failed parsing body, returning raw content");
                        // إترك output كما هو bodyText إذا لم نستطع المعالجة
                    }
                }
                else
                {
                    // body فارغ => No Content
                    var wrapped = new
                    {
                        statusCode = context.Response.StatusCode,
                        success = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                        message = GetDefaultMessage(context.Response.StatusCode),
                        data = (object?)null
                    };
                    output = JsonSerializer.Serialize(wrapped, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
                }

                // اكتب النتيجة النهائية في الستريم الأصلي
                context.Response.Body = originalBodyStream;
                context.Response.ContentType = "application/json";

                await using var writer = new StreamWriter(originalBodyStream, Encoding.UTF8, leaveOpen: true);
                await writer.WriteAsync(output);
                await writer.FlushAsync();
            }
            finally
            {
                // تأكد دائما أننا أعدنا الـ Response.Body الأصلي
                context.Response.Body = originalBodyStream;
            }
        }

        private static bool IsProblemDetailsResponse(string bodyText)
        {
            if (string.IsNullOrWhiteSpace(bodyText)) return false;

            try
            {
                using var doc = JsonDocument.Parse(bodyText);
                return doc.RootElement.TryGetProperty("type", out _) &&
                       doc.RootElement.TryGetProperty("title", out _) &&
                       (doc.RootElement.TryGetProperty("status", out _) || doc.RootElement.TryGetProperty("statusCode", out _));
            }
            catch
            {
                return false;
            }
        }

        private static string GetDefaultMessage(int statusCode) => statusCode switch
        {
            StatusCodes.Status200OK => "Operation completed successfully.",
            StatusCodes.Status201Created => "Resource created successfully.",
            StatusCodes.Status204NoContent => "No content.",
            StatusCodes.Status400BadRequest => "Bad request.",
            StatusCodes.Status401Unauthorized => "Unauthorized.",
            StatusCodes.Status403Forbidden => "Forbidden.",
            StatusCodes.Status404NotFound => "Not found.",
            StatusCodes.Status500InternalServerError => "Internal server error.",
            _ => "Request processed."
        };
    }
}
