using System;
using System.Collections.Generic;

namespace Cacoch.Core.Manifest
{
    public record Manifest(Guid Id, string ApiVersion, string Slug, string DisplayName, List<IResource> Resources);
}