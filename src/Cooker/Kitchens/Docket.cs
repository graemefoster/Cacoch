using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public class Docket
    {
        public string TableName { get; }
        public IIngredient[] LineItems { get; }

        public Docket(string tableName, params IIngredient[] lineItems)
        {
            TableName = tableName;
            LineItems = lineItems;
        }
    }
}