using System;
using Cooker.Kitchens;

namespace Cooker.Ingredients
{
    public class CookbookLibrary<TPlatformContext> where TPlatformContext: IPlatformContext
    {
        private readonly Func<IIngredient, IRecipeBuilder<TPlatformContext>> _ingredientBuilders;

        public CookbookLibrary(Func<IIngredient, IRecipeBuilder<TPlatformContext>> ingredientBuilders)
        {
            _ingredientBuilders = ingredientBuilders;
        }

        public IRecipeBuilder<TPlatformContext> GetCookbookFor<T>(T ingredient) where T : IIngredient
        {
            return _ingredientBuilders(ingredient);
        }
    }
}