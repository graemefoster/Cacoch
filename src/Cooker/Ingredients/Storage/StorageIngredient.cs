using System.Collections.Generic;

namespace Cooker.Ingredients.Storage
{
    public class StorageIngredient : Ingredient
    {
        internal StorageIngredient(StorageData data) : base(data)
        {
            Tables = data.Tables;
            Queues = data.Queues;
            Containers = data.Containers;
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