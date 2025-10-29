using Media1.Models;
using Media1.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUpload"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<FileUploadSettings>>().Value);
builder.Services.AddScoped<MediaService>();

var app = builder.Build();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status413PayloadTooLarge)
    {
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Upload failed: File size exceeds 200 MB.");
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


