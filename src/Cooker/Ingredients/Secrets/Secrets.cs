﻿using System.Collections.Generic;

namespace Cooker.Ingredients.Secrets
{
    public class Secrets : Ingredient
    {
        internal Secrets(SecretsData secretsData) : base(secretsData)
        {
        }

        public override bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            var satisfied = false;
            if (DepedencyHelper.IsSatisfied(DisplayName, edibles, out var displayName))
            {
                DisplayName = displayName!;
                satisfied = true;
            }

            return satisfied;
        }
    }
}