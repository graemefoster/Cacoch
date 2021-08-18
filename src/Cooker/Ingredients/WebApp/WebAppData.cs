namespace Cooker.Ingredients.WebApp
{
    [CookerResource("webapp")]
    public record WebAppData(string Id, string Name, string Classification) : IngredientData(Id, Name)
    {
        public override IIngredient BuildIngredient()
        {
            return new WebAppIngredient(this);
        }
    }
}