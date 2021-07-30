﻿using System.ComponentModel;

namespace Cooker.Ingredients.Secrets
{
    public record SecretsOutput(string Name) : ICookedIngredient;

    [CookerResource("secrets")]
    public record SecretsData(string Id, string Name, [DefaultValue(new string[] {})] string[] Secrets) : IngredientData(Id, Name)
    {
        public override IIngredient BuildIngredient()
        {
            return new SecretsIngredient(this);
        }
    }
}