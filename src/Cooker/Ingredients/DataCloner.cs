using System;
using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public static class DataCloner
    {
        public static bool Clone<T>(this T input, IDictionary<IIngredient, ICookedIngredient> edibles, out T? clone)
            where T : IngredientData
        {
            if (!IsRecord(input.GetType())) throw new NotSupportedException("All ingredients must be record types");
            var newProps = new Dictionary<string, object?>();
            clone = default(T);
            foreach (var property in input.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    if (!DepedencyHelper.IsSatisfied((string?)property.GetValue(input), edibles, out var newValue))
                    {
                        return false;
                    }
                    newProps.Add(property.Name, newValue);
                }

                if (property.PropertyType.IsAssignableTo(typeof(IDictionary<string, string>)))
                {
                    var newDictionary = new Dictionary<string, string>();
                    var oldDictionary = (IDictionary<string, string>?)property.GetValue(input);
                    if (oldDictionary == null)
                    {
                        newProps.Add(property.Name, newDictionary);
                    }
                    
                    foreach (var entry in oldDictionary!)
                    {
                        if (!DepedencyHelper.IsSatisfied(entry.Value, edibles, out var newValue))
                        {
                            return false;
                        }

                        newDictionary.Add(entry.Key, newValue!);
                    }                
                    newProps.Add(property.Name, newDictionary);
                }
            }

            var cloneMethod = typeof(T).GetMethod("<Clone>$")!;
            clone = (T)cloneMethod.Invoke(input, new object[] { })!;
            foreach (var replacement in newProps)
            {
                clone.GetType().GetProperty(replacement.Key)!.SetValue(clone, replacement.Value);
            }
            return true;
        }

        private static bool IsRecord(Type type) => type.GetMethod("<Clone>$") != null;
    }
}