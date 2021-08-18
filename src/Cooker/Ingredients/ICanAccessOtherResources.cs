using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public interface ICanAccessOtherResources
    {
        string Id { get; }
        IEnumerable<CookerLink>? Links { get; }
    }
}