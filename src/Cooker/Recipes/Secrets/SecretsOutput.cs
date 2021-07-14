using Cooker.Kitchens;

namespace Cooker.Recipes.Secrets
{
    public class SecretsOutput : ILineItemOutput
    {
        public SecretsOutput(ILineItem input, string name)
        {
            Input = input;
            Name = name;
        }

        public ILineItem Input { get; }
        public string Name { get; }
    }
}