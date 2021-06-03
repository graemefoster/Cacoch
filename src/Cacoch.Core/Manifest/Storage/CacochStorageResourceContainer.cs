using Cacoch.Core.Manifest.Abstractions;

namespace Cacoch.Core.Manifest.Storage
{
    public record CacochStorageResourceContainer(CacochStorageResourceContainerType Type, string Name);


    [CacochResource("storageLink")]
    public record StorageLink(string Name, LinkAccess Access) : CacochResourceLinkMetadata(Name);

    public enum LinkAccess
    {
        Read,
        ReadWrite
    }
}