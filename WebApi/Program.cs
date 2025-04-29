using Application.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Data.Seeders;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using ISession = NHibernate.ISession;
using Npgsql;
using RabbitMQ.Client;
using System.Text;
using FluentNHibernate.Cfg;
using Infrastructure.Data.Mappings;
using Domain.Events;
using NHibernate.Tool.hbm2ddl;
using Infrastructure.Security;
using Environment = NHibernate.Cfg.Environment;
using FluentNHibernate.Conventions.Helpers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
#endregion

#region Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyConstants.AdminOnly, policy =>
        policy.RequireRole(RoleConstants.Admin));

    options.AddPolicy(PolicyConstants.DeliveryDriverOnly, policy =>
        policy.RequireRole(RoleConstants.DeliveryDriver));
});
#endregion

#region NHibernate Configuration
var configuration = new Configuration();
configuration.DataBaseIntegration(db =>
{
    var pgConfig = builder.Configuration.GetSection("PostgreSQL");
    db.ConnectionString = new NpgsqlConnectionStringBuilder
    {
        Host = pgConfig["Host"],
        Port = int.Parse(pgConfig["Port"]),
        Database = pgConfig["Database"],
        Username = pgConfig["Username"],
        Password = pgConfig["Password"],
        Pooling = bool.Parse(pgConfig["Pooling:Enabled"] ?? "true"),
        SslMode = Enum.Parse<SslMode>(pgConfig["SslMode"] ?? "Prefer")
    }.ToString();

    db.Dialect<PostgreSQL83Dialect>();
    db.Driver<NpgsqlDriver>();
    db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
});

var fluentConfig = Fluently.Configure(configuration)
    .Mappings(m =>
        m.FluentMappings
            .AddFromAssembly(typeof(RoleMap).Assembly)
            .Conventions.Add(
                FluentNHibernate.Conventions.Helpers.DefaultLazy.Never(),
                ConventionBuilder.Id.Always(x =>
                {
                    if (x.Type == typeof(Guid))
                    {
                        x.GeneratedBy.GuidComb();
                    }
                })
            )
    )
    .ExposeConfiguration(cfg =>
    {
        cfg.Properties[Environment.ConnectionDriver] = typeof(NHibernate.Driver.NpgsqlDriver).AssemblyQualifiedName;
        cfg.Properties[Environment.Dialect] = typeof(NHibernate.Dialect.PostgreSQL83Dialect).AssemblyQualifiedName;

        if (builder.Environment.IsDevelopment())
        {
            new SchemaUpdate(cfg)
                .Execute(false, true);
        }
        else
        {
            new SchemaUpdate(cfg)
                .Execute(false, true);
        }
    });

configuration = fluentConfig.BuildConfiguration();

builder.Services.AddSingleton(configuration);
builder.Services.AddSingleton<ISessionFactory>(provider =>
    provider.GetRequiredService<Configuration>().BuildSessionFactory());
builder.Services.AddScoped<ISession>(provider =>
    provider.GetRequiredService<ISessionFactory>().OpenSession());
#endregion

#region Dependency Injection - Repositories & Services
// Repositories
builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IRepository<MotorcycleRegisteredEvent>, EventRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<IDeliveryDriverService, DeliveryDriverService>();

// Application Services
builder.Services.AddScoped<IMotorcycleService, MotorcycleService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IDeliveryDriverRepository, DeliveryDriverRepository>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
#endregion

#region RabbitMQ Configuration
builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:Host"],
    UserName = builder.Configuration["RabbitMQ:Username"] ?? "guest",
    Password = builder.Configuration["RabbitMQ:Password"] ?? "guest",
    DispatchConsumersAsync = true,
    AutomaticRecoveryEnabled = true
});

builder.Services.AddSingleton<IConnection>(provider =>
{
    var factory = provider.GetRequiredService<IConnectionFactory>();
    return factory.CreateConnection();
});

builder.Services.AddSingleton<IMessageBroker, RabbitMQService>();
builder.Services.AddHostedService<RabbitMQConsumerService>();
builder.Services.AddScoped<MotorcycleEventConsumer>();
#endregion

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

#region Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
#endregion

#region Seed Roles
using (var scope = app.Services.CreateScope())
{
    var session = scope.ServiceProvider.GetRequiredService<ISession>();
    using var transaction = session.BeginTransaction();
    try
    {
        var existingRoles = session.Query<Role>().Count();
        if (existingRoles == 0)
        {
            Console.WriteLine("Seeding initial roles...");
            RoleSeeder.Seed(session);
            transaction.Commit();
            Console.WriteLine("Roles seeded successfully");
        }
        else
        {
            Console.WriteLine("Roles already exist, skipping seeding");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding roles: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
        transaction.Rollback();

        if (app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}
#endregion

app.Run();