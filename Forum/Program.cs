using Forum.Controllers.GraphQL.Mutation;
using Forum.Controllers.GraphQL.Query;
using Forum.Model.DB;
using HotChocolate.Execution.Processing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Forum.Model.Services;
using Forum.Model;
using Microsoft.Extensions.Caching.Distributed;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});


builder.Services.AddScoped<Forum.Model.Services.IAuthorizationService, AuthorizationService>();


//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = options.DefaultPolicy;
//});

builder.Services.AddAuthorization(options =>
{
    // Не требовать авторизации по умолчанию, если это допустимо
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build();
});


//бд

builder.Services.AddDbContext<ForumDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//redis

var redisConfig = builder.Configuration.GetSection("Redis");

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = redisConfig["Configuration"];
    options.InstanceName = redisConfig["InstanceName"];
});





//graphQL
builder.Services
    .AddGraphQLServer()
     .UseField(next => async context =>
    {
        var cache = context.Service<IDistributedCache>();
        var middleware = new GraphQLCacheMiddleware(next, cache);
        await middleware.InvokeAsync(context);
    })
    .AddQueryType<Query>()
    .AddTypeExtension<UsersQuery>()
    .AddTypeExtension<PostsQuery>()
    .AddTypeExtension<PostPartialQuery>()


       .AddMutationType<Mutation>()
       .AddTypeExtension<Authorization>()


    .AddAuthorization()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();




app.UseRouting();

app.MapGraphQL("/graphql");

app.MapControllers();

app.Run();
