namespace Cooker.Ingredients.Functions
{
    public record FunctionOutput(FunctionDataInternal Original, string PlatformName, string HostName, string Identity) : CookedIngredient<FunctionDataInternal>(Original), IHaveRuntimeIdentity;
}