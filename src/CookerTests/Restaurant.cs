using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Kitchens;
using Cooker.Kitchens.Azure;
using Cooker.Recipes;
using Cooker.Recipes.Secrets;
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

            var storage = new Storage(name, name);
            var docket = new Docket(storage);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);
            var edible = (StorageOutput) meal[storage];

            edible.Name.ShouldBe(name);
        }


        [Fact]
        public async Task can_work_out_complex_dependency_chain()
        {
            var storage1 = new Storage("one", "one");
            var secrets1 = new Secrets("secretsone", "[one.Name]-foofoo");
            var storage2 = new Storage("two", "[secretsone.Name]-foofoo");

            var docket = new Docket(storage1, storage2, secrets1);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);

            ((StorageOutput)meal[storage2]).Name.ShouldBe("one-foofoo-foofoo");
        }
        [Fact]
        public async Task can_work_out_dependencies()
        {
            var storage1 = new Storage("one", "one");
            var storage2 = new Storage("two", "[one.Name]-foofoo");

            var docket = new Docket(storage1, storage2);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);

            ((StorageOutput)meal[storage2]).Name.ShouldBe("one-foofoo");
        }

        [Fact]
        public async Task can_have_multi_steps()
        {
            var secrets1 = new Secrets("one", "one");

            var docket = new Docket(secrets1);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);

            ((SecretsOutput)meal[secrets1]).Name.ShouldBe("one");
        }

        private static Cooker.Restaurant BuildTestRestaurant()
        {
            var runner = new FakeArmRunner();
            var restaurant = new Cooker.Restaurant(
                new Kitchen(new[] {new ArmKitchenStation(runner)}),
                new CookbookLibrary());
            return restaurant;
        }

        [Fact]
        public void throws_on_unknown_dependency_properties()
        {
            var storage1 = new Storage("one", "one");
            var storage2 = new Storage("two", "[one.DoesNotExist]-foofoo");

            var docket = new Docket(storage1, storage2);

            var restaurant = BuildTestRestaurant();

            Should.Throw<InvalidOperationException>(async () => await restaurant.PlaceOrder(docket));
        }

        [Fact]
        public void can_detect_dependency_issues()
        {
            var storage1 = new Storage("one", "one");
            var storage2 = new Storage("two", "[storage1.Name]-foofoo");

            var docket = new Docket(storage1, storage2);

            var restaurant = BuildTestRestaurant();

            Should.Throw<InvalidOperationException>(async () => await restaurant.PlaceOrder(docket));
        }

        [Fact]
        public void cannot_build_unknown_items()
        {
            var unknown = new UnknownItem();
            var docket = new Docket(unknown);

            var restaurant = BuildTestRestaurant();

            Should.Throw<NotSupportedException>(async () => await restaurant.PlaceOrder(docket));
        }

        class UnknownItem : ILineItem
        {
            public string Id => "Foo";
            public string DisplayName => "Foo";
        }
    }

    internal class FakeArmRunner : IArmRunner
    {
        public Task<object> Execute(string template, Dictionary<string, object> parameters)
        {
            return Task.FromResult(new object());
        }
    }
}