using System;
using System.Threading.Tasks;
using Cooker;
using Cooker.Kitchens;
using Cooker.Kitchens.AzureArm;
using Cooker.Recipes;
using Cooker.Recipes.Storage;
using Shouldly;
using Xunit;

namespace CookerTests
{
    public class Restaurant
    {
        [Fact]
        public async Task can_build_recipe()
        {
            var name = "1234";

            var storage = new Storage(name);
            var docket = new Docket(storage);

            var restaurant = new Cooker.Restaurant(
                new[] {(Kitchen) new ArmKitchen()},
                new RecipeBook());

            var meal = await restaurant.PlaceOrder(docket);
            var edible = (StorageOutput) meal[storage];

            edible.Name.ShouldBe(name);
        }


        [Fact]
        public void throws_exception_on_unknown_recipes()
        {
            var storage1 = new Storage("one");
            var storage2 = new Storage("[storage1.Name]", storage1);
            
            var docket = new Docket(storage1, storage2);

            var restaurant = new Cooker.Restaurant(
                new[] {(Kitchen) new ArmKitchen()},
                new RecipeBook());

            Should.ThrowAsync<NotSupportedException>(async () => await restaurant.PlaceOrder(docket));
        }
    }
}