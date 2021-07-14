using Cooker.Ingredients;

namespace Cooker.Kitchens
{
    public class Docket
    {
        public string TableName { get; }
        public ILineItem[] LineItems { get; }

        public Docket(string tableName, params ILineItem[] lineItems)
        {
            TableName = tableName;
            LineItems = lineItems;
        }
    }
}