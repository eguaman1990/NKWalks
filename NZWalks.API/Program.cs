using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZWalks.API.Data;
using NZWalks.API.Mappings;
using NZWalks.API.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder( args );

// Add services to the container.

builder.Services.AddControllers( );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen(
        options =>
        {
            options.SwaggerDoc( "v1", new( ) { Title = "NZ Walks API", Version = "v1" } );
            options.AddSecurityDefinition( JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            } );

            options.AddSecurityRequirement( new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "Oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header

                    },
                    new List<string>()
                }
            } );
        } );

// add bd context
builder.Services.AddDbContext<NZWalksDbContext>( options =>
{
    options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) );
} );

builder.Services.AddDbContext<NZWalksAuthDbContext>( options =>
{
    options.UseSqlServer( builder.Configuration.GetConnectionString( "AuthConnection" ) );
} );

// add repositories
builder.Services.AddScoped<IRegionRepository, InMemoryRegionRepository>( );
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>( );
builder.Services.AddScoped<ITokenRepository, TokenRepository>( );

builder.Services.AddAutoMapper( typeof( AutoMapperProfiles ) );

// add jwt authentication
builder.Services.AddIdentityCore<IdentityUser>( options => { } )
    .AddRoles<IdentityRole>( )
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>( "NZWalks" )
    .AddEntityFrameworkStores<NZWalksAuthDbContext>( )
    .AddDefaultTokenProviders( );

builder.Services.Configure<IdentityOptions>( options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
} );

// add authentication
builder.Services.AddAuthentication( JwtBearerDefaults.AuthenticationScheme )
    .AddJwtBearer( options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudiences = new[] { builder.Configuration["Jwt:Audience"] },
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes( builder.Configuration["Jwt:Key"] ) )
        }
    );

var app = builder.Build( );

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment( ))
{
    app.UseSwagger( );
    app.UseSwaggerUI( );
}

app.UseHttpsRedirection( );

app.UseAuthentication( );
app.UseAuthorization( );

app.MapControllers( );

app.Run( );
