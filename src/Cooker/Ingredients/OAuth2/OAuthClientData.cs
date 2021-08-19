using System.Collections.Generic;
using System.ComponentModel;

namespace Cooker.Ingredients.OAuth2
{
    [CookerResource("oauthClient")]
    public record OAuthClientData(
        string Id,
        string DisplayName,
        [DefaultValue(new string[] { })] string[] RedirectUrls
    ) : IngredientData(Id)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new OAuthClientIngredient(this);
        }
    }
}