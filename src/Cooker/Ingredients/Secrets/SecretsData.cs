using System.Collections.Generic;
using System.ComponentModel;

namespace Cooker.Ingredients.Secrets
{
    [CookerResource("secrets")]
    public record SecretsData(
        string Id,
        string Name,
        [DefaultValue(new string[] { })] string[] Secrets
    ) : IngredientData(Id, Name)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new SecretsIngredient(this);
        }

        public record SecretsLink(string Resource, LinkAccess Link) : CookerLink(Resource, Link);
    }
}