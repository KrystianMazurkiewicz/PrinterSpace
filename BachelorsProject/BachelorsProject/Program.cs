using BachelorsProject.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Castle.Core.Logging;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using Microsoft.Data.Sqlite;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;

public class Program //changed from internal to public for testing
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //Moh_Logs
        builder.Logging.AddFile("Logs/Log-{Date}.txt");
        //Moh


        // Add services to the container.
        //These are dependency injection - service that gets triggered at startup


        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers();

        //ADD MY OWN REDIRECIT HEADDDERRR STREAM



        builder.Services.AddDbContext<SystemContext>(options => options.UseSqlite("Data Source = System.db"));

        //TEST
        builder.Services.AddScoped<ISystemRepository, SystemRepository>();



        builder.Services.AddHostedService<RepeatingService>();
        builder.Services.AddCors(x => x.AddPolicy("corspolicy", build =>
        {
            build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        }));

        //JWS TOEKN
        builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true; //includes error report
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false

                };
                /*
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                    TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes)
                    ValidateIssuer = false,
                    ValidateAudience=false,
                    ClockSkew = TimeSpan.FromMinutes(5)
                }; */
            });



        //role identitiy
        //builder.Services.AddDefaultIdentity<IdentityUser>
        //builder.Services.Add

        //swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });


        // MOHA ADD

        //MOHA END

        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = ".AdventureWorks.Session";
            options.IdleTimeout = TimeSpan.FromDays(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddDistributedMemoryCache();

        //file size limit
        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 268435456; // 250 MB in bytes
        });

        //swagger - think of integrating it

        /*
        var dbPath = "./SystemDB.db"; //path of DB
        var connection = new SqliteConnection($"Data Source={dbPath}");
        builder.Services.AddDbContext<SystemContext>(options => options.UseSqlite(connection));
        */
        //both approches work
        /*
        var connection = new SqliteConnection("Data Source=Systemdb.db");
        builder.Services.AddDbContext<SystemContext>(options => options.UseSqlite(connection));
        */


        /*
        builder.Services.AddDbContext<DbContext>(options =>
            options.UseSqlite("Data Source=User.db"));
        */
        // this has been added to connect the IKundeRepo
        //builder.Services.AddScoped<ISystemRepository, SystemRepository>();

        /*
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SystemContext>();
            // use context
        }
        */
        /*
        var connectionString = builder.Configuration.GetConnectionString("todos") ?? "Data Source=todos.db";
        builder.Services.AddSqlite<DbContext>(connectionString);
        */

        //error test --- done- webpage lunches
        //page taken from https://stackoverflow.com/questions/31886779/asp-net-mvc-6-aspnet-session-errors-unable-to-resolve-service-for-type
        //builder.Services.AddDistributedMemoryCache();

        /*
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        */

        /*
         * builder.Services.AddDbContext<DBContext>(options => options.UseSqlite("Data source=User.db"));


        builder.Services.AddDbContext<DBContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));*/

        var app = builder.Build();



        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //app.UseHsts();
            app.UseDeveloperExceptionPage();

            /*
            app.UseSwagger(); 
            app.UseSwaggerUI()
            */
        }
        //swagger in development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseSwaggerUI();
            DBInit.Initialize(app);

            //logger

        }




        //DB
        /*
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<DBContext>();
            // Note: if you're having trouble with EF, database schema, etc.,
            // uncomment the line below to re-create the database upon each run.
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            DBInitializer.Initialize(context);
        }*/



        //app.UseHttpsRedirection();
        app.UseCors("corspolicy");
        app.UseRouting();
        //app.UseStaticFiles();
        //FOR JWT, has to be above Authorization
        app.UseAuthentication();

        //AUthorization
        app.UseAuthorization();

        //sesions
        app.UseSession();
        //



        //test
        /*
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            //taken from
            //https://github.com/serilog/serilog-extensions-logging-file
            //Check if works
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHost =>
            {
                webHost.UseStartup<Program>();
            })
            .ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.AddFile("Logs/myapp-{Date}.txt");
            })
            .Build();
        */


        //

        app.UseEndpoints(endpoints =>
        {
            //change to controllers to map them
            endpoints.MapControllers();
        });

        //TEST 3.05
        app.MapControllers();



        /*app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");*/


        app.Run();
    }
}