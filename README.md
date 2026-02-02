# Talep Yönetim Sistemi (TalepYonetimi)

## 📋 Proje Hakkında

TalepYonetimi, kullanıcıların talep oluşturup takip edebileceği, rol tabanlı yetkilendirme sistemi ile desteklenen modern bir web uygulamasıdır. ASP.NET Core 9.0 MVC mimarisi ile geliştirilmiş olup, Entity Framework Core ve SQL Server veritabanı kullanmaktadır.

## 🏗️ Kullanılan Mimari Yaklaşım

### Genel Mimari

Proje **MVC (Model-View-Controller)** tasarım deseni kullanılarak geliştirilmiştir. Katmanlı mimari prensiplerine uygun olarak aşağıdaki yapıda organize edilmiştir:

#### 1. **Models (Veri Katmanı)**

- `User`: Kullanıcı bilgilerini tutan model
- `Role`: Rol tanımlamaları
- `Permission`: İzin/yetki tanımlamaları
- `Request`: Talep modeli
- `RequestStatusHistory`: Talep durum geçmişi
- `RolePermission`: Rol-izin ilişkilendirme

#### 2. **Controllers (Kontrol Katmanı)**

- `AccountController`: Kimlik doğrulama ve kullanıcı işlemleri
- `AdminController`: Yönetici işlemleri (kullanıcı/rol yönetimi)
- `HomeController`: Ana sayfa ve genel işlemler
- `RequestsController`: Talep yönetimi işlemleri

#### 3. **Views (Görünüm Katmanı)**

- Razor View Engine kullanılarak dinamik HTML sayfa oluşturma
- Modüler yapı: Account, Admin, Home, Requests, Shared klasörleri
- Bootstrap ile responsive tasarım

#### 4. **Services (İş Mantığı Katmanı)**

- `IAuthService / AuthService`: Kimlik doğrulama işlemleri
- `IRequestService / RequestService`: Talep işlemleri
- `IAdminService / AdminService`: Yönetici işlemleri
- **Dependency Injection** ile servis yönetimi

#### 5. **Data Access Layer**

- `ApplicationDbContext`: Entity Framework Core DbContext
- **Code-First** yaklaşımı ile migration'lar
- SQL Server veritabanı

### Teknoloji Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Language**: C# (.NET 9.0)
- **ORM**: Entity Framework Core 9.0
- **Database**: Microsoft SQL Server 2022
- **Authentication**: Cookie-based Authentication
- **Frontend**: Razor Views, Bootstrap, jQuery
- **Containerization**: Docker & Docker Compose

## 🚀 Kurulum Adımları

### Ön Gereksinimler

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server/sql-server-downloads) veya Docker
- (Opsiyonel) [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Yöntem 1: Yerel Kurulum (.NET SDK ile)

#### 1. Projeyi Klonlayın

```bash
git clone <repository-url>
cd talepMvc
```

#### 2. Bağımlılıkları Yükleyin

```bash
dotnet restore
```

#### 3. Veritabanı Bağlantısını Yapılandırın

`appsettings.Development.json` dosyasını oluşturun ve veritabanı bağlantı dizesini ekleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TalepYonetimDB;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### 4. Veritabanını Oluşturun

Uygulama otomatik migration uygulayacaktır, ancak manuel olarak da yapabilirsiniz:

```bash
dotnet ef database update
```

#### 5. Uygulamayı Çalıştırın

```bash
dotnet run
```

Tarayıcınızda `https://localhost:7256` veya `http://localhost:5250` adresine gidin.
