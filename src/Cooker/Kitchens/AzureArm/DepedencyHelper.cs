using System;
using System.Collections.Generic;
using System.Linq;
using Cooker.Recipes;

namespace Cooker.Kitchens.AzureArm
{
    static class DepedencyHelper
    {
        public static bool IsSatisfied(string value, IDictionary<ILineItem, IEdible> edibles, out string? prop)
        {
            if (value.Contains("["))
            {
                var expression = value[(value.IndexOf("[", StringComparison.Ordinal) + 1)..value.LastIndexOf("]", StringComparison.Ordinal)]!;
                var expressionParts = expression.Split(".");
                
                var dependencyLineItem = expressionParts[0];
                var edible = edibles.Keys.SingleOrDefault(x => x.Name == dependencyLineItem);
                if (edible == null)
                {
                    prop = null;
                    return false;
                }

                var propertyInfo = edible.GetType().GetProperty(expressionParts[1]);
                if (propertyInfo == null)
                {
                    throw new InvalidOperationException($"Failed to resolve property on expression {expression}");
                }

                prop = value.Replace($"[{expression}]", Convert.ToString(propertyInfo.GetValue(edible!)));
                return true;
            }

            prop = value;
            return true;
        }
    }
}