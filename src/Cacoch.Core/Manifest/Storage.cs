using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Cacoch.Core.Manifest
{
    public class Storage: IResource
    {
        public Storage(string name, string[] containers)
        {
            Name = name;
            Containers = containers;
        }

        public string Name { get; }
        public string[] Containers { get; }
    }
}