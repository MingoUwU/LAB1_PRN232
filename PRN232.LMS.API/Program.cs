using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PRN232.LMS.Repositories;
using PRN232.LMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Cấu hình Database
// Chương trình sẽ ưu tiên đọc chuỗi kết nối "DefaultConnection" từ file appsettings.json (Dành cho chạy nội bộ bằng Visual Studio).
// Nếu không tìm thấy, nó sẽ dùng chuỗi kết nối mặc định phía sau (Dành cho khi chạy bằng Docker).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Xử lý triệt để lỗi "LocalDB is not supported on this platform" khi chạy Docker
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker")
{
    connectionString = "Server=db;Database=LMSDb;User Id=sa;Password=Trungnam@12345;TrustServerCertificate=True;";
}

builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Layer Services
builder.Services.AddLmsServices();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => 
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll"); // Enable CORS

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Apply migrations automatically with retry logic (vì Docker SQL Server có thể khởi động chậm)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    int maxRetry = 5;
    for (int i = 0; i < maxRetry; i++)
    {
        try
        {
            db.Database.EnsureCreated();
            break; // Thành công thì thoát vòng lặp
        }
        catch (System.Exception)
        {
            if (i == maxRetry - 1) throw; // Nếu thử 5 lần vẫn lỗi thì throw
            System.Threading.Thread.Sleep(3000); // Chờ 3s rồi thử lại
        }
    }
}

app.Run();
