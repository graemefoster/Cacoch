using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public class Docket
    {
        public string TableName { get; }
        public IngredientData[] LineItems { get; }

        public Docket(string tableName, params IngredientData[] lineItems)
        {
            TableName = tableName;
            LineItems = lineItems;
        }
    }
}