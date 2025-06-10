using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Repository;
using Restaurant_WebAPI.Services;
using Unity;
using Unity.WebApi;

namespace Restaurant_WebAPI.App_Start
{
    public class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Registering types to inject
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<ITokenRepository, TokenRepository>();
            container.RegisterType<IAuthServiceAPI, AuthServiceAPI>();

            // Restaurant types
            container.RegisterType<IRestaurantRepository, RestaurantRepository>();
            container.RegisterType<IRestaurantService, RestaurantService>();

            // Order types
            container.RegisterType<IOrderRepositoryAPI, OrderRepositoryAPI>();
            container.RegisterType<IOrderServiceAPI, OrderServiceAPI>();

            // Account Types
            container.RegisterType<IAccountService, AccountService>();

            // Set the dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
