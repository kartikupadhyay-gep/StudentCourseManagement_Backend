
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Services;
using System.Text;

namespace StudentCourseManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtKey = builder.Configuration["JwtSettings:Key"];
            //var jwtKeyCourse = builder.Configuration["JwtSettings:Key:1"];
            //var jwtKeyEnrollment = builder.Configuration["JwtSettings:Key:2"];

            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            //var keyBytesCourse = Encoding.UTF8.GetBytes(jwtKeyCourse);
            //var keyBytesEnrollment = Encoding.UTF8.GetBytes(jwtKeyEnrollment);

            // Add services to the container.

            builder.Services.AddSingleton<JwtHelper>();
            builder.Services.AddScoped<AuthService>();
            
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
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudiences = new List<string>
                    {
                        builder.Configuration["JwtSettings:Audiences:0"],
                        builder.Configuration["JwtSettings:Audiences:1"],
                        builder.Configuration["JwtSettings:Audiences:2"]
                    },
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("StudentScheme", policy =>
                {
                    policy.AddAuthenticationSchemes("StudentScheme");
                    policy.RequireAuthenticatedUser();
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CourseScheme", policy =>
                {
                    policy.AddAuthenticationSchemes("CourseScheme");
                    policy.RequireAuthenticatedUser();
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("EnrollmentScheme", policy =>
                {
                    policy.AddAuthenticationSchemes("EnrollmentScheme");
                    policy.RequireAuthenticatedUser();
                });
            });


            builder.Services.AddControllers();
            builder.Services.AddSingleton<StudentService>();
            builder.Services.AddSingleton<CourseService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Token **_only_**\n\n Access for\n 1. **Student**\n 2. **Course**\n 3. **Enrollment**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { } }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowFromSpecificOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                );
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowFromSpecificOrigin");

            app.MapControllers();

            app.UseStaticFiles();

            app.Run();
        }
    }
}
