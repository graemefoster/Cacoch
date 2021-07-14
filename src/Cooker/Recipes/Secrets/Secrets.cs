namespace Cooker.Recipes.Secrets
{
    public class Secrets : LineItem<SecretsOutput>
    {
        public Secrets(string id, string displayName) : base(id, displayName)
        {
        }
    }
}