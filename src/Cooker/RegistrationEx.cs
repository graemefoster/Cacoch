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
            Dictionary<Type, Type> recipeBuilders) where TPlatformContext: IPlatformContext
        {

            services.AddSingleton<IRestaurant, Restaurant<TPlatformContext>>();
            services.AddSingleton<Kitchen<TPlatformContext>>();
            services.AddSingleton(sp => new CookbookLibrary<TPlatformContext>(i => 
                (IRecipeBuilder<TPlatformContext>)ActivatorUtilities.CreateInstance(sp, recipeBuilders[i.GetType()], i)));
        }
    }
}