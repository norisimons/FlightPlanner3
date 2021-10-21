using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Core.Servives;
using FlightPlanner.Data;
using FlightPlanner.Services;
using FlightPlanner.Services.Validators;
using FlightPlanner.Web2.Authentication;
using FlightPlanner.Web2.Mappings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FlightPlanner.Web2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FlightPlanner.Web2", Version = "v1" });
            });

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions,
                BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddDbContext<FlightPlannerDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("flight-planner")));

            services.AddScoped<IFlightPlannerDbContext, FlightPlannerDbContext>();
            services.AddScoped<IDbService, DbService>(); ///// Scoped - dzîvos pieprasîjuma ciklu, ne ilgâk
            services.AddScoped<IEntityService<Flight>, EntityService<Flight>>();
            services.AddScoped<IEntityService<Airport>, EntityService<Airport>>();
            services.AddScoped<IDbServiceExtended, DbServiceExtended>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IValidator, AirportCodesEqualityValidator>();
            services.AddScoped<IValidator, AirportCodeValidator>();
            services.AddScoped<IValidator, ArrivalTimeValidator>();
            services.AddScoped<IValidator, CarrierValidator>();
            services.AddScoped<IValidator, CityValidator>();
            services.AddScoped<IValidator, CountryValidator>();
            services.AddScoped<IValidator, DepartureTimeValidator>();
            services.AddScoped<IValidator, TimeFrameValidator>();
            services.AddScoped<ISearchValidator, SearchValidator>();
            services.AddScoped<IAirportService, AirportService>();

            var cfg = AutoMapperConfiguration.GetConfig();
            services.AddSingleton(typeof(IMapper), cfg);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightPlanner.Web2 v1"));
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
