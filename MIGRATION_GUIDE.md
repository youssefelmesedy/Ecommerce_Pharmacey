# 📦 Entity Refactoring Migration Guide

## 📝 الملخص

هذا الدليل يشرح كيفية إكمال تحديث المشروع بعد تعديلات الكيانات.

## ✅ ما تم إنجازه

- ✅ **Domain Layer**: تحديث جميع الكيانات (9 entities)
- ✅ **Domain Enums**: إضافة 4 enums جديدة
- ✅ **Infrastructure Configurations**: تحديث جميع الـ EF Core configurations (9 files)

## ⚠️ ما يتطلب إكماله يدوياً

### 1️⃣ إنشاء Migration جديد

```bash
# من مجلد الـ solution
cd EcommercPharmacy

# إنشاء migration
dotnet ef migrations add RefactorEntitiesWithEnums \
  --project Pharmacy.Infarstructure \
  --startup-project Pharmacy.Api

# مراجعة الـ migration قبل التطبيق
# تفحص ملف Migration في Migrations folder

# تطبيق الـ migration
dotnet ef database update \
  --project Pharmacy.Infarstructure \
  --startup-project Pharmacy.Api
```

### 2️⃣ تحديث Application Layer

#### 📄 DTOs التي تحتاج تحديث:

**UserDto / CreateUserDto / UpdateUserDto:**
```csharp
using Pharmacy.Domain.Enums;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }  // ⚠️ تغيير من string
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }  // ✨ جديد
}
```

**ProductDto / CreateProductDto / UpdateProductDto:**
```csharp
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? SKU { get; set; }  // ✨ جديد
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }  // ✨ جديد
    public int StockQuantity { get; set; }  // ⚠️ تغيير من decimal
    public bool IsActive { get; set; }  // ✨ جديد
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }  // ✨ جديد
}
```

**OrderDto / CreateOrderDto / UpdateOrderDto:**
```csharp
using Pharmacy.Domain.Enums;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }  // ⚠️ تغيير من string
    public PaymentStatus PaymentStatus { get; set; }  // ✨ جديد
    public PaymentMethod? PaymentMethod { get; set; }  // ✨ جديد
    public string DeliveryAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }  // ✨ جديد
    public DateTime OrderDate { get; set; }
    public DateTime? UpdatedAt { get; set; }  // ✨ جديد
    public DateTime? CompletedAt { get; set; }  // ✨ جديد
}
```

**OrderItemDto:**
```csharp
public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }  // ⚠️ تغيير من decimal
    public decimal UnitPrice { get; set; }
    public string? ProductName { get; set; }  // ✨ جديد
    public decimal Total { get; set; }
}
```

**PhoneNumberDto:**
```csharp
public class PhoneNumberDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;  // ⚠️ تغيير من phoneNumber
    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }  // ✨ جديد
}
```

**CategoryDto:**
```csharp
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }  // ✨ جديد
    public DateTime CreatedAt { get; set; }  // ✨ جديد
}
```

**ProductImageDto:**
```csharp
public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }  // ✨ جديد
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }  // ✨ جديد
}
```

#### 🔧 Services التي تحتاج تحديث:

**AuthenticationService** - تحديث RefreshToken.Id:
```csharp
// قبل
var refreshToken = new RefreshToken
{
    // Id سيتم توليده بواسطة القاعدة (int identity)
    Token = GenerateRefreshToken(),
    ...
};

// بعد
var refreshToken = new RefreshToken
{
    Id = Guid.NewGuid(),  // ⚠️ يجب توليد Guid يدوياً
    Token = GenerateRefreshToken(),
    ...
};
```

**UserService** - تحديث Role:
```csharp
// قبل
var user = new User
{
    ...
    Role = "Customer"  // string
};

// بعد
using Pharmacy.Domain.Enums;

var user = new User
{
    ...
    Role = UserRole.Customer  // enum
};
```

**OrderService** - تحديث Status و Payment:
```csharp
// قبل
var order = new Order
{
    ...
    Status = "Pending"  // string
};

// بعد
using Pharmacy.Domain.Enums;

var order = new Order
{
    ...
    Status = OrderStatus.Pending,  // enum
    PaymentStatus = PaymentStatus.Unpaid,
    PaymentMethod = PaymentMethod.Cash
};

// تحديث حالة الطلب
order.Status = OrderStatus.Processing;
order.UpdatedAt = DateTime.UtcNow;

// إكمال الطلب
order.Status = OrderStatus.Completed;
order.CompletedAt = DateTime.UtcNow;
```

**ProductService** - تحديث StockQuantity:
```csharp
// قبل
product.StockQuantity = 10.5m;  // decimal

// بعد
product.StockQuantity = 10;  // int
```

### 3️⃣ تحديث API Layer

#### 🎮 Controllers:

**UsersController:**
```csharp
using Pharmacy.Domain.Enums;

[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto dto)
{
    // تحديث validation للـ Role
    if (!Enum.IsDefined(typeof(UserRole), dto.Role))
    {
        return BadRequest("Invalid role");
    }
    
    // ...
}
```

**OrdersController:**
```csharp
using Pharmacy.Domain.Enums;

[HttpPatch("{id}/status")]
public async Task<IActionResult> UpdateStatus(
    Guid id, 
    [FromBody] UpdateOrderStatusDto dto)
{
    // validation
    if (!Enum.IsDefined(typeof(OrderStatus), dto.Status))
    {
        return BadRequest("Invalid order status");
    }
    
    // ...
}

[HttpPost("{id}/complete")]
public async Task<IActionResult> CompleteOrder(Guid id)
{
    var order = await _orderService.GetByIdAsync(id);
    if (order == null) return NotFound();
    
    order.Status = OrderStatus.Completed;
    order.CompletedAt = DateTime.UtcNow;
    order.UpdatedAt = DateTime.UtcNow;
    
    await _orderService.UpdateAsync(order);
    return Ok();
}
```

**ProductsController:**
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
{
    // تحديث validation
    if (dto.StockQuantity < 0)
    {
        return BadRequest("Stock quantity must be non-negative integer");
    }
    
    // ...
}
```

#### 📝 Validation Attributes:

أضف validation للـ DTOs:

```csharp
using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    // ...
}

public class CreateOrderDto
{
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod? PaymentMethod { get; set; }
    
    // ...
}
```

### 4️⃣ تحديث AutoMapper Profiles (إن وُجد)

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
        
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        
        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod));
        
        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        
        // OrderItem mappings
        CreateMap<OrderItems, OrderItemDto>()
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));
        
        // PhoneNumber mappings
        CreateMap<PhoneNumbers, PhoneNumberDto>();
        
        // Category mappings
        CreateMap<Category, CategoryDto>();
        
        // ProductImage mappings
        CreateMap<ProductImage, ProductImageDto>();
    }
}
```

## 🧪 قائمة الاختبار

### Infrastructure:
- [ ] تشغيل Migration بنجاح
- [ ] التحقق من schema في قاعدة البيانات
- [ ] فحص الـ indexes الجديدة

### Application:
- [ ] تحديث جميع DTOs
- [ ] تحديث Services
- [ ] تحديث AutoMapper profiles
- [ ] فحص unit tests

### API:
- [ ] تحديث Controllers
- [ ] تحديث Validation
- [ ] اختبار جميع endpoints
- [ ] تحديث Swagger documentation

### Functional Testing:
- [ ] اختبار User Registration مع Role enum
- [ ] اختبار Authentication flow مع RefreshToken (Guid)
- [ ] اختبار إنشاء Order مع enums
- [ ] اختبار تحديث Order Status
- [ ] اختبار Product creation مع StockQuantity (int)
- [ ] اختبار إضافة OrderItems مع Quantity (int)

## ⚠️ Breaking Changes

### API Responses:
1. **User.Role** أصبح enum بدلاً من string:
   ```json
   // قبل
   {"role": "Customer"}
   
   // بعد
   {"role": 0}  // أو {"role": "Customer"} حسب serializer settings
   ```

2. **Order.Status** و **PaymentStatus** أصبحوا enums
3. **Product.StockQuantity** و **OrderItems.Quantity** أصبحوا `int`
4. **RefreshToken.Id** أصبح `Guid` بدلاً من `int`

### Configuration Changes:
أضف في `Program.cs` للتعامل مع enums في JSON:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // عرض enums كـ strings بدلاً من numbers
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
```

## 🔗 مراجع مفيدة

- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Enum Conversions in EF Core](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)
- [ASP.NET Core Model Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [JSON Serialization of Enums](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties#enums-as-strings)

## 📞 دعم

إذا واجهتك مشكلة:
1. راجع هذا الدليل
2. فحص commit history في PR #1
3. اختبر على branch منفصل قبل merge

---

**Good Luck!** 🚀
