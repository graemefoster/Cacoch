using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooker.Ingredients.WebApp
{
    [CookerResource("webapp")]
    public record WebAppData(
            string Id,
            string Name,
            string Classification,
            Dictionary<string, string> Configuration,
            IEnumerable<CookerLink>? Links)
        : IngredientData(Id, Name), ICanAccessOtherResources
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            yield return new WebAppIngredient(this);
            foreach (var link in this.Gather())
            {
                yield return link;
            }
        }
    }

    internal static class IngredientsEx
    {
        public static IEnumerable<IIngredient> Gather(this ICanAccessOtherResources resource)
        {
            if (resource.Links == null) return Enumerable.Empty<IIngredient>();
            return resource.Links.Select(x => new LinkIngredient(
                new LinkData(
                    $"link-{resource.Id}-{x.Resource}-{x.Link}",
                    "link",
                    resource.Id,
                    x.Resource,
                    x.Link)));
        }
    }

    public class LinkIngredient : Ingredient<LinkData>
    {
        public LinkIngredient(LinkData ingredientData) : base(ingredientData)
        {
        }

        public override bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            var fromIngredient = edibles.Keys.SingleOrDefault(x => x.Id == TypedIngredientData.FromResource);
            var toIngredient = edibles.Keys.SingleOrDefault(x => x.Id == TypedIngredientData.ToResource);
            if (fromIngredient == null || toIngredient == null) return false;
            From = edibles[fromIngredient];
            To = edibles[toIngredient];
            return true;
        }

        public ICookedIngredient? To { get; private set; }

        public ICookedIngredient? From { get; private set; }
    }

    public record LinkData(string Id, string DisplayName, string FromResource, string ToResource, LinkAccess Access) :
        IngredientData(Id,
            DisplayName)
    {
        public override IEnumerable<IIngredient> GatherIngredients()
        {
            throw new NotSupportedException("This is internal use only");
        }
    }
}