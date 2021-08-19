using System.Collections.Generic;
using System.Linq;
using Cooker.Ingredients.Link;

namespace Cooker.Ingredients
{
    internal static class IngredientsEx
    {
        public static IEnumerable<IIngredient> Gather(this ICanAccessOtherResources resource)
        {
            if (resource.Links == null) return Enumerable.Empty<IIngredient>();
            return resource.Links.Select(x => new LinkIngredient(
                new LinkData(
                    $"link-{resource.Id}-{x.Resource}-{x.Link}",
                    resource.Id,
                    x.Resource,
                    x.Link)));
        }
    }
}