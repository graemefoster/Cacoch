using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources;
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

            var storage = new StorageData(name, name, Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var docket = new Docket("Docket", storage);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);
            var edible = (StorageOutput) meal[storage];

            edible.Name.ShouldBe(name);
        }

        private static PlatformEnvironment GetTestEnvironment()
        {
            return new PlatformEnvironment("dev", "Development");
        }


        [Fact]
        public async Task can_work_out_complex_dependency_chain()
        {
            var storage1 = new StorageData("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var secrets1 = new SecretsData("secretsone", "[one.Name]-foofoo");
            var storage2 = new StorageData("two", "[secretsone.Name]-foofoo", Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2, secrets1);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);

            ((StorageOutput) meal[storage2]).Name.ShouldBe("one-foofoo-foofoo");
        }

        [Fact]
        public async Task can_work_out_dependencies()
        {
            var storage1 = new StorageData("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new StorageData("two", "[one.Name]-foofoo", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);

            ((StorageOutput) meal[storage2]).Name.ShouldBe("one-foofoo");
        }

        [Fact]
        public async Task can_have_multi_steps()
        {
            var secrets1 = new SecretsData("one", "one");

            var docket = new Docket("Docket", secrets1);

            var restaurant = BuildTestRestaurant();

            var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);

            ((SecretsOutput) meal[secrets1]).Name.ShouldBe("one");
        }

        private static Restaurant<AzurePlatformContext> BuildTestRestaurant()
        {
            var runner = new FakeArmRunner();
            var secretSdk = new FakeAzureResourcesSdk();

            var restaurant = new Restaurant<AzurePlatformContext>(
                new Kitchen<AzurePlatformContext>(
                    new KitchenStation<AzurePlatformContext>[]
                    {
                        new ArmKitchenStation(runner),
                        new AzureSdkKitchenStation(secretSdk)
                    }),
                new CookbookLibrary<AzurePlatformContext>(new Dictionary<Type, Type>
                {
                    {typeof(SecretsIngredient), typeof(AzureKeyVaultBuilder)},
                    {typeof(StorageIngredient), typeof(AzureStorageBuilder)},
                }),
                new TestContextBuilder());
            return restaurant;
        }

        private class TestContextBuilder : IPlatformContextBuilder<AzurePlatformContext>
        {
            public Task<AzurePlatformContext> Build(Docket docket, PlatformEnvironment platformEnvironment)
            {
                return Task.FromResult(new AzurePlatformContext(docket, platformEnvironment));
            }
        }

        [Fact]
        public void throws_on_unknown_dependency_properties()
        {
            var storage1 = new StorageData("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new StorageData("two", "[one.DoesNotExist]-foofoo", Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant();

            Should.Throw<InvalidOperationException>(async () =>
                await restaurant.PlaceOrder(GetTestEnvironment(), docket));
        }

        [Fact]
        public void can_detect_dependency_issues()
        {
            var storage1 = new StorageData("one", "one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new StorageData("two", "[storage1.Name]-foofoo", Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket("Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant();

            Should.Throw<InvalidOperationException>(async () =>
                await restaurant.PlaceOrder(GetTestEnvironment(), docket));
        }

        [Fact]
        public void cannot_build_unknown_items()
        {
            var unknown = new UnknownItem("1", "Name");
            var docket = new Docket("Docket", unknown);

            var restaurant = BuildTestRestaurant();

            Should.Throw<NotSupportedException>(async () => await restaurant.PlaceOrder(GetTestEnvironment(), docket));
        }

        record UnknownItem(string Id, string DisplayName) : IngredientData(Id, DisplayName)
        {
            public override IIngredient BuildIngredient()
            {
                return new UnknownIngredient(this);
            }

            class UnknownIngredient : IIngredient
            {
                public UnknownIngredient(UnknownItem item)
                {
                    OriginalIngredientData = item;
                    Id = item.Id;
                    DisplayName = item.DisplayName;
                }

                public IngredientData OriginalIngredientData { get; }
                public string Id { get; }
                public string DisplayName { get; }

                public bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }

    internal class FakeAzureResourcesSdk : IAzureResourcesSdk
    {
        public Task<ICookedIngredient> Execute<TOutput>(AzurePlatformContext platformContext,
            Func<AzurePlatformContext, ResourcesManagementClient, TOutput> action) where TOutput : ICookedIngredient
        {
            return Task.FromResult((ICookedIngredient) default(TOutput)!);
        }
    }
}