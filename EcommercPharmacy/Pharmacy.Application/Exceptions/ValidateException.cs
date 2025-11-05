namespace Pharmacy.Application.Exceptions;
public class ValidateException
    : Exception
{
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    public ValidateException()
        : base("One or more validation errors occurred.")
    {
    }

    public ValidateException(string message)
        : base(message)
    {
    }

    public ValidateException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public ValidateException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    // ✅ دالة: تضيف خطأ جديد (بدون إنشاء Exception جديد)
    public void AddError(string key, string errorMessage)
    {
        if (!Errors.ContainsKey(key))
            Errors[key] = new[] { errorMessage };
        else
            Errors[key] = Errors[key].Concat(new[] { errorMessage }).ToArray();
    }

    // ✅ دالة: تحول كل الأخطاء إلى نص موحد (مثلاً للـ Logging)
    public string GetFormattedErrors()
    {
        return string.Join("; ", Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"));
    }

    // ✅ دالة: تدمج أخطاء استثناء آخر (مثلاً من ValidateException ثانية)
    public void Merge(ValidateException other)
    {
        foreach (var kv in other.Errors)
            AddError(kv.Key, string.Join(", ", kv.Value));
    }

    // ✅ دالة: للتحقق بسرعة من وجود أخطاء
    public bool HasErrors => Errors.Any();
}


