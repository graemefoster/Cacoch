using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cacoch.Core.Manifest;
using Shouldly;
using Xunit;

namespace Cacoch.Provider.AzureArm.Tests
{
    public class the_storage_twin
    {
        [Theory] 
        [InlineData("aaaa", false)]
        [InlineData("aaaaa", true)]
        [InlineData("abcdabcdabc", true)]
        [InlineData("abcdabcdabcd", false)]
        public async Task allows_between_5_and_11_characters(string name, bool valid)
        {
            var storage = new Cacoch.Provider.AzureArm.Resources.Storage.StorageTwin(new Storage(name));
            var validationResult = await storage.Validate();
            if (valid) validationResult.ShouldBe(ValidationResult.Success);
            else validationResult.ErrorMessage.ShouldNotBeNull();
        }
    }
}