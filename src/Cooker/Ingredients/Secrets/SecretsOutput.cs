namespace Cooker.Ingredients.Secrets
{
    public class SecretsOutput : ILineItemOutput
    {
        public SecretsOutput(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}