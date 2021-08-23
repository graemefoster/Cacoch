using System.Collections.Generic;

namespace Cooker.Ingredients
{
    public interface ICanAccessOtherResources
    {
        string Id { get; }
        CookerLink[]? Links { get; }
    }
}