using System;
using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public class Docket
    {
        public string TableName { get; }
        public IngredientData[] LineItems { get; }
        public Guid Id { get; set; }

        public Docket(Guid id, string tableName, params IngredientData[] lineItems)
        {
            Id = id;
            TableName = tableName;
            LineItems = lineItems;
        }
    }
}