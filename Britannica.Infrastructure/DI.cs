using Autofac;
using Britannica.Application.Contracts;
using Britannica.Infrastructure.Repositories;
using Britannica.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Britannica.Infrastructure
{
    public static class DI
    {
        public static Assembly Assembly => Assembly.GetExecutingAssembly();

        public static ContainerBuilder RegisterInfrastructure(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterContext<ApplicationDbContext>(configuration["DbPath"]);

            builder.RegisterType<DateTimeService>().As<IDateTime>().InstancePerLifetimeScope();
            builder.RegisterType<FlightRepository>().As<IFlightRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PassengerRepository>().As<IPassengerRepository>().InstancePerLifetimeScope();
            
            return builder;
        }

        /// <summary>
        /// AutoFac Register DBcontext 
        /// mimics https://github.com/dotnet/efcore/blob/40c8dbe9d45aa161a912345539675e77d114979d/src/EFCore/Extensions/EntityFrameworkServiceCollectionExtensions.cs#L476-L500
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        private static void RegisterContext<TContext>(this ContainerBuilder builder, string dataSource) where TContext : DbContext
        {
            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=BritannicaDb.db"));
            builder.Register(componentContext =>
            {
                var serviceProvider = componentContext.Resolve<IServiceProvider>();
                var configuration = componentContext.Resolve<IConfiguration>();
                var dbContextOptions = new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>());
                var optionsBuilder = new DbContextOptionsBuilder<TContext>(dbContextOptions)
                    .UseApplicationServiceProvider(serviceProvider)
                    .UseSqlite($"Data Source={dataSource}");;

                return optionsBuilder.Options;
            }).As<DbContextOptions<TContext>>().InstancePerLifetimeScope();

            builder.Register(context => context.Resolve<DbContextOptions<TContext>>())
                .As<DbContextOptions>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TContext>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
