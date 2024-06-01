using Dexma_cpt_DBLibrary;
using Dexma_cpt_ServerSide.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddDbContext<DexmaDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=DexmaDb;Username=TestStudent;Password=\"teststudent\""));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.Run();
