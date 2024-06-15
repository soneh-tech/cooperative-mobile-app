global using CooperativeAppAPI.Models;
global using Microsoft.EntityFrameworkCore;
global using CooperativeAppAPI.Data;
global using Microsoft.AspNetCore.Mvc;
global using CooperativeAppAPI.Repositories;
global using CooperativeAppAPI.Helpers;
global using System.Data;
global using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDBContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString"));
});

builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IMemberService, MemberService>();
builder.Services.AddTransient<IDashboardService, DashboardService>();
builder.Services.AddTransient<IAccountTypeService, AccountTypeService>();
builder.Services.AddTransient<IStatesService, StatesService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
