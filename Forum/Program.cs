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
using Forum.Controllers.GraphQL.Subscription;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region регистраци€ сервисов 

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

builder.Services.AddAuthorization(options =>
{
    // Ќе требовать авторизации по умолчанию, если это допустимо
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build();
});



#endregion

#region регистраци€  DI


builder.Services.AddScoped<Forum.Model.Services.IAuthorizationService, AuthorizationService>();

builder.Services.AddScoped<PostsMangerService>(); //сделать интерфейс

builder.Services.AddScoped<ICommentManager, CommentManagerService>();

builder.Services.AddScoped<IGrateService, GradeService>();

builder.Services.AddScoped<SubscriptionService>();

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = options.DefaultPolicy;
//});



//бд

builder.Services.AddDbContext<ForumDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//redis

var redisConfig = builder.Configuration.GetSection("Redis");

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = redisConfig["Configuration"];
    options.InstanceName = redisConfig["InstanceName"];
});

#endregion


#region graphQL

builder.Services
    .AddGraphQLServer()
    .AddInMemorySubscriptions()
     .UseField(next => async context =>
    {
        var cache = context.Service<IDistributedCache>();
        var middleware = new GraphQLCacheMiddleware(next, cache);
        await middleware.InvokeAsync(context);
    })
     //запросы
    .AddQueryType<Query>()
    .AddTypeExtension<UsersQuery>()
    .AddTypeExtension<PostsQuery>()
    .AddTypeExtension<RoleQuery>()
    .AddTypeExtension<PostPartialQuery>()
    
       //мутации
       .AddMutationType<Mutation>()
       .AddTypeExtension<Authorization>()
       .AddTypeExtension<PostsMutation>()
       .AddTypeExtension<GradeMutation>()
       .AddTypeExtension<CommentMutation>()

        //подписки
        .AddSubscriptionType<Subscription>()

       .AddType<UploadType>()
//атрибуты
    .AddAuthorization()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

#endregion

//cors политика 
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


#region настройка сервисов 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();

app.UseRouting();

app.MapGraphQL("/graphql");

app.MapControllers();

#endregion

app.Run();
