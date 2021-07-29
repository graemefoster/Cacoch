namespace Cooker.Ingredients.Secrets
{
    public record SecretsData(string Id, string Name) : IngredientData(Id, Name)
    {
        public override IIngredient BuildIngredient()
        {
            return new Secrets(this);
        }
    }
}