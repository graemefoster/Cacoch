using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Cooker;
using Cooker.Azure;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Azure.Ingredients.Storage;
using Cooker.Ingredients;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;
using Shouldly;
using Xunit;

namespace CookerTests
{
    public class Restaurant
    {
        [Fact]
        public async Task can_build_an_order()
        {
            var name = "1234";

            var storage = new Storage(name, name, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            var docket = new Docket("Docket", storage);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);
            var edible = (StorageOutput) meal[storage];

            edible.Name.ShouldBe(name);
        }


        [Fact]
        public async Task can_work_out_complex_dependency_chain()
        {
            var storage1 = new Storage("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var secrets1 = new Secrets("secretsone", "[one.Name]-foofoo");
            var storage2 = new Storage("two", "[secretsone.Name]-foofoo", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2, secrets1);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);

            ((StorageOutput) meal[storage2]).Name.ShouldBe("one-foofoo-foofoo");
        }

        [Fact]
        public async Task can_work_out_dependencies()
        {
            var storage1 = new Storage("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new Storage("two", "[one.Name]-foofoo", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);

            ((StorageOutput) meal[storage2]).Name.ShouldBe("one-foofoo");
        }

        [Fact]
        public async Task can_have_multi_steps()
        {
            var secrets1 = new Secrets("one", "one");

            var docket = new Docket("Docket", secrets1);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(docket);

            ((SecretsOutput) meal[secrets1]).Name.ShouldBe("one");
        }

        private static Cooker.Restaurant BuildTestRestaurant()
        {
            var runner = new FakeArmRunner();
            var secretSdk = new FakeSecretSdk();

            var restaurant = new Cooker.Restaurant(
                new Kitchen(new KitchenStation[]
                {
                    new ArmKitchenStation(runner),
                    new AzureSdkKitchenStation(secretSdk)
                }),
                new CookbookLibrary(new Dictionary<Type, Type>
                {
                    {typeof(Secrets), typeof(AzureKeyVaultBuilder)},
                    {typeof(Storage), typeof(AzureStorageBuilder)},
                }),
                new TestContextBuilder());
            return restaurant;
        }

        private class TestContextBuilder : IPlatformContextBuilder
        {
            public Task<IPlatformContext> Build(Docket docket)
            {
                return Task.FromResult((IPlatformContext)new AzurePlatformContext());
            }
        }

        [Fact]
        public void throws_on_unknown_dependency_properties()
        {
            var storage1 = new Storage("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new Storage("two", "[one.DoesNotExist]-foofoo", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant();

            Should.Throw<InvalidOperationException>(async () => await restaurant.PlaceOrder(docket));
        }

        [Fact]
        public void can_detect_dependency_issues()
        {
            var storage1 = new Storage("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new Storage("two", "[storage1.Name]-foofoo", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant();

            Should.Throw<InvalidOperationException>(async () => await restaurant.PlaceOrder(docket));
        }

        [Fact]
        public void cannot_build_unknown_items()
        {
            var unknown = new UnknownItem();
            var docket = new Docket("Docket", unknown);

            var restaurant = BuildTestRestaurant();

            Should.Throw<NotSupportedException>(async () => await restaurant.PlaceOrder(docket));
        }

        class UnknownItem : IIngredient
        {
            public string Id => "Foo";
            public string DisplayName => "Foo";

            public bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
            {
                return true;
            }
        }
    }

    internal class FakeSecretSdk : ISecretSdk
    {
        public Task<ICookedIngredient> Execute<TOutput>(Func<SecretClient, TOutput> action)
            where TOutput : ICookedIngredient
        {
            return Task.FromResult((ICookedIngredient) default(TOutput)!);
        }
    }
}