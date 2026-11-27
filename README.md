# ProductStore: MVC 4 (Login, Roles, Filters, Error Pages)

مشروع المحاضرة الرابعة. هو مشروع المحاضرة التالتة بعد ما اتضاف عليه تسجيل الدخول بالـ cookies، والأدوار (Admin وUser)، والـ Authorize، وفلتر بسيط، وصفحات أخطاء.
الكود كله مكتوب بأبسط شكل ممكن: بدون ASP.NET Core Identity الكامل، عشان الطلبة يفهموا اللي بيحصل جوه (الـ Identity بيعمل نفس الأفكار دي بس بشكل جاهز).

## التشغيل
1. فك الـ zip الأول (متفتحش الـ sln من جوه الـ zip).
2. المتطلبات: .NET 8 SDK وSQL Server LocalDB وأداة EF (`dotnet tool install --global dotnet-ef --version 8.*`).
3. من فولدر المشروع:
```
dotnet restore
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
```
افتح http://localhost:5100.

لو كمّلت على مشروع المحاضرة التالتة بدل ما تبدأ من الأول: الجديد في الداتابيز جدول `Users` بس، فاعمل:
```
dotnet ef migrations add AddUsers
dotnet ef database update
```

## أول Admin
أول ما التطبيق يشتغل ولا يلاقي أي مستخدم، بيعمل مستخدم Admin لوحده (في `Program.cs`):
- الإيميل: `admin@store.com`
- الباسورد: `Admin123`

ده للتعليم بس. في مشروع حقيقي مش بنكتب باسورد في الكود.
أي حد يسجّل من صفحة Register بياخد دور `User` (مش Admin).

## اللي في المشروع (وحل الأنشطة الستة)
| النشاط | الحل في المشروع |
|---|---|
| 1. جدول Users | `Models/User.cs`, `DbSet<User>` في `Data/AppDbContext.cs` |
| 2. Logout واسم المستخدم في الـ navbar | `AccountController.Logout`, `Views/Shared/_Layout.cshtml` |
| 3. حماية صفحات الأقسام | `[Authorize]` في `CategoriesController` (في المشروع النهائي `Roles = "Admin"`) |
| 4. الأدمن بس للمنتجات | `ProductsController` و`Views/Products/Index.cshtml` |
| 5. فلتر بيطبع بداية ونهاية الـ action | `Filters/LogActionAttribute.cs` (النهاية: `OnActionExecuted` هي المطلوبة من الطلبة) |
| 6. صفحة 404 خاصة | `Controllers/HomeController.cs` (action `Error`) و`Views/Home/Error.cshtml` |

الأنشطة بتتعمل على مشروع الطلبة نفسه خطوة بخطوة. المشروع ده هو الشكل النهائي بعدها.

## نقاط للشرح
- كلمة السر بتتخزن Hash (`PasswordHasher`) مش نص عادي، والـ Hash مش بيتفك.
- رسالة الخطأ في الـ Login واحدة ("Email or password is wrong") سواء الإيميل غلط أو الباسورد، عشان محدش يعرف أنهي إيميل موجود.
- `returnUrl` بنتأكد إنه جوه موقعنا (`Url.IsLocalUrl`) قبل ما نرجّع المستخدم له.
- الـ Logout بـ POST ومعاه الـ anti-forgery token، مش لينك عادي.
- `UseAuthentication` لازم قبل `UseAuthorization` في `Program.cs`.
- إخفاء الأزرار بـ `User.IsInRole("Admin")` للشكل بس. الحماية الحقيقية هي `[Authorize(Roles = "Admin")]` على الـ controller.
- ما فيش unique index على الإيميل في الداتابيز (بنفحصه بالكود). كتحسين: `HasIndex(u => u.Email).IsUnique()`.
