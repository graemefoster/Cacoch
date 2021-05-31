using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    public class Storage: IResource
    {
        private readonly IPlatformTwin<Storage> _twin;

        public Storage(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}