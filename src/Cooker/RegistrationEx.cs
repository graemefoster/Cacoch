using System;
using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Kitchens;
using Microsoft.Extensions.DependencyInjection;

namespace Cooker
{
    public static class RegistrationEx
    {
        public static void RegisterCooker(this IServiceCollection services,
            Dictionary<Type, Func<IIngredient, IIngredientBuilder>> cookbooks)
        {
            services.AddSingleton<Restaurant>();
            services.AddSingleton<Kitchen>();
            services.AddSingleton(new CookbookLibrary(cookbooks));
        }
    }
}