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
                new Bookshelf());

            var meal = await restaurant.PlaceOrder(docket);
            var edible = (StorageOutput) meal[storage];

            edible.Name.ShouldBe(name);
        }


        [Fact]
        public async Task can_work_out_dependencies()
        {
            var storage1 = new Storage("one");
            var storage2 = new Storage("[one.Name]-foofoo");
            
            var docket = new Docket(storage1, storage2);

            var restaurant = new Cooker.Restaurant(
                new[] {(Kitchen) new ArmKitchen()},
                new Bookshelf());

            var meal = await restaurant.PlaceOrder(docket);
            
            meal[storage2].Name.ShouldBe("one-foofoo");

        }

        [Fact]
        public void can_detect_dependency_issues()
        {
            var storage1 = new Storage("one");
            var storage2 = new Storage("[storage1.Name]-foofoo");
            
            var docket = new Docket(storage1, storage2);

            var restaurant = new Cooker.Restaurant(
                new[] {(Kitchen) new ArmKitchen()},
                new Bookshelf());

            Should.Throw<InvalidOperationException>(async () => await restaurant.PlaceOrder(docket));

        }
    }
}