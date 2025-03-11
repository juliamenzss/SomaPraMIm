using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SomaPraMim.Application.Services.ShoppingListServices;
using SomaPraMim.Application.Services.UserServices;
using SomaPraMim.Application.Validators;
using SomaPraMim.Application.Validators.UserValidator;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados PostgreSQL
builder.Services.AddDbContext<SomaPraMimDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeção de dependência
builder.Services.AddScoped<IUserContext, SomaPraMimDbContext>();
builder.Services.AddScoped<IShoppingListContext, SomaPraMimDbContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IValidator<UserCreateRequest>, UserCreateValidator>();
builder.Services.AddScoped<IValidator<UserUpdateRequest>, UserUpdateValidator>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();
