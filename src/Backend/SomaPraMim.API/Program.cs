using Microsoft.EntityFrameworkCore;
using SomaPraMim.Application.Services.ShoppingItemServices;
using SomaPraMim.Application.Services.ShoppingListServices;
using SomaPraMim.Application.Services.UserServices;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados PostgreSQL
builder.Services.AddDbContext<SomaPraMimDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularLocalhost",
        policy => policy.WithOrigins("http://localhost:4200") 
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddScoped<IUserContext, SomaPraMimDbContext>();
builder.Services.AddScoped<IShoppingListContext, SomaPraMimDbContext>();
builder.Services.AddScoped<IShoppingItemContext, SomaPraMimDbContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IShoppingItemService, ShoppingItemService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAngularLocalhost");
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();
