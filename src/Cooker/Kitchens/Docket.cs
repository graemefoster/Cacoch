using System.Collections.Generic;
using System.Linq;
using Cooker.Recipes;

namespace Cooker.Kitchens
{
    public class Docket
    {
        public ILineItem[] LineItems { get; }

        public Docket(params ILineItem[] lineItems)
        {
            LineItems = lineItems;
        }
    }
}