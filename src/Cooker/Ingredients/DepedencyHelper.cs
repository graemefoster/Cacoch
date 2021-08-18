using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cooker.Ingredients
{
    internal static class DepedencyHelper
    {
        public static bool IsSatisfied(string? value, IDictionary<IIngredient, ICookedIngredient> edibles, out string? prop)
        {
            if (value == null)
            {
                prop = null;
                return true;
            }
            
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
                object? context = edible;
                foreach (var part in expressionParts.Skip(1))
                {
                    if (context == null)
                    {
                        throw new ArgumentException(
                            $"Failed to resolve expression {expression} against output {lineItem.Id}");
                    }
                    
                    if (context is IDictionary dictionary)
                    {
                        context = dictionary[part];
                    }
                    else
                    {
                        var propertyInfo = context.GetType().GetProperty(part);
                        if (propertyInfo == null)
                        {
                            throw new InvalidOperationException(
                                $"Failed to resolve property on expression {expression}");
                        }

                        context = propertyInfo.GetValue(context);
                    }
                }

                prop = value.Replace($"[{expression}]", Convert.ToString(context));
                
                return true;
            }

            prop = value;
            return true;
        }
    }
}