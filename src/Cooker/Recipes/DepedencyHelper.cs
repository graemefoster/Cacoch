using System;
using System.Collections.Generic;
using System.Linq;

namespace Cooker.Recipes
{
    static class DepedencyHelper
    {
        public static bool IsSatisfied(string value, IDictionary<ILineItem, ILineItemOutput> edibles, out string? prop)
        {
            if (value.Contains("["))
            {
                var expression = value[(value.IndexOf("[", StringComparison.Ordinal) + 1)..value.LastIndexOf("]", StringComparison.Ordinal)]!;
                var expressionParts = expression.Split(".");
                
                var dependencyLineItem = expressionParts[0];
                var lineItem = edibles.Keys.SingleOrDefault(x => x.Id == dependencyLineItem);
                if (lineItem == null)
                {
                    prop = null;
                    return false;
                }

                var edible = edibles[lineItem];
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