using System.ComponentModel.DataAnnotations.Schema;

namespace Cooker.Recipes.Storage
{
    public class Storage : LineItem<StorageOutput>
    {
        public Storage(string name, params ILineItem[] dependsOn) : base(name, dependsOn)
        {
        }
    }
}