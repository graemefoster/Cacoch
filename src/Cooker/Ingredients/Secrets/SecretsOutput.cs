namespace Cooker.Ingredients.Secrets
{
    public class SecretsOutput : ICookedIngredient
    {
        public SecretsOutput(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}