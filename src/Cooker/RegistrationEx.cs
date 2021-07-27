using System;
using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Kitchens;
using Microsoft.Extensions.DependencyInjection;

namespace Cooker
{
    public static class RegistrationEx
    {
        public static void RegisterCooker<TPlatformContext>(
            this IServiceCollection services,
            Dictionary<Type, Type> cookbooks) where TPlatformContext: IPlatformContext
        {
            services.AddSingleton<IRestaurant, Restaurant<TPlatformContext>>();
            services.AddSingleton<Kitchen>();
            services.AddSingleton(new CookbookLibrary<TPlatformContext>(cookbooks));
        }
    }
}