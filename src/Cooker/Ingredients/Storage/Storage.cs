using System.Collections.Generic;

namespace Cooker.Ingredients.Storage
{
    public class Storage : Ingredient
    {
        public Storage(
            string id,
            string displayName,
            string[] tables,
            string[] queues,
            string[] containers) : base(id, displayName)
        {
            Tables = tables;
            Queues = queues;
            Containers = containers;
        }

        public string[] Tables { get; }
        public string[] Queues { get; }
        public string[] Containers { get; }

        public override bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
        {
            var satisfied = false;
            if (DepedencyHelper.IsSatisfied(DisplayName, edibles, out var displayName))
            {
                DisplayName = displayName!;
                satisfied = true;
            }

            return satisfied;
        }
    }
}