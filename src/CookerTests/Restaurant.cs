using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker;
using Cooker.Azure;
using Cooker.Azure.Ingredients.OAuthClient;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Azure.Ingredients.Storage;
using Cooker.Azure.KitchenStations.AzureResourceManager;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Ingredients;
using Cooker.Ingredients.OAuth2;
using Cooker.Ingredients.Secrets;
using Cooker.Ingredients.Storage;
using Cooker.Kitchens;
using Microsoft.Extensions.Logging.Abstractions;
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

            var storage = new StorageData(name, Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var docket = new Docket(Guid.NewGuid(), "Docket", storage);

            var restaurant = BuildTestRestaurant(out _, out var armRunner);
            armRunner.Seed(new { resourceId = "" });

            var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);
            var edible = (StorageOutput)meal[storage];

            edible.Data.Id.ShouldBe(name);
        }

        private static PlatformEnvironment GetTestEnvironment()
        {
            return new PlatformEnvironment("dev", "Development");
        }

        
        // TODO
        // [Fact]
        // public async Task can_work_out_complex_dependency_chain()
        // {
        //     var oauth1 = new OAuthClientData("one", "one", Array.Empty<string>());
        //     var oauth2 = new OAuthClientData("two", "[one.Data.DisplayName]-foofoo", Array.Empty<string>());
        //     var oauth3 = new OAuthClientData("three", "[two.Data.DisplayName]-foofoo", Array.Empty<string>());
        //
        //     var docket = new Docket(Guid.NewGuid(), "Docket", oauth1, oauth2, oauth3);
        //
        //     var restaurant = BuildTestRestaurant(out var sdk, out var armRunner);
        //
        //     sdk.Seed(new OAuthClientOutput(oauth3, "3", null));
        //     sdk.Seed(new OAuthClientOutput(oauth2, "2", null));
        //     sdk.Seed(new OAuthClientOutput(oauth1, "1", null));
        //
        //     var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);
        //
        //     ((OAuthClientOutput)meal[oauth2]).Data.DisplayName.ShouldBe("one-foofoo");
        //     ((OAuthClientOutput)meal[oauth3]).Data.DisplayName.ShouldBe("one-foofoo-foofoo");
        // }

        [Fact]
        public async Task can_work_out_dependencies()
        {
            var storage1 = new StorageData("one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new StorageData("[one.Data.Id]-foofoo", Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket(Guid.NewGuid(), "Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant(out _, out var armRunner);
            armRunner.Seed(new { resourceId = "" });
            armRunner.Seed(new { resourceId = "" });

            var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);

            ((StorageOutput)meal[storage2]).Data.Id.ShouldBe("one-foofoo");
        }

        [Fact]
        public async Task can_have_multi_steps()
        {
            var secrets1 = new SecretsData("one", new[] { "secret-one" }, Array.Empty<SecretsData.KnownSecret>());

            var docket = new Docket(Guid.NewGuid(), "Docket", secrets1);

            var restaurant = BuildTestRestaurant(out var sdk, out var armRunner);
            armRunner.Seed(new { resourceId = "", vaultUrl = "https://somewhere.com/" });
            sdk.Seed(new AzureSecretsBuilder.ExistingSecretsOutput(Array.Empty<string>()));

            var meal = await restaurant.PlaceOrder(GetTestEnvironment(), docket);

            ((SecretsOutput)meal[secrets1]).Data.Id.ShouldBe("one");
        }

        private static Restaurant<AzurePlatformContext> BuildTestRestaurant(
            out FakeAzureResourcesSdk sdk,
            out FakeArmRunner armRunner)
        {
            armRunner = new FakeArmRunner();
            sdk = new FakeAzureResourcesSdk();

            var restaurant = new Restaurant<AzurePlatformContext>(
                new Kitchen<AzurePlatformContext>(
                    new KitchenStation<AzurePlatformContext>[]
                    {
                        new ArmKitchenStation(armRunner),
                        new AzureSdkKitchenStation(sdk)
                    }),
                new CookbookLibrary<AzurePlatformContext>(
                    x =>
                    {
                        if (x is SecretsIngredient ingredient) return new AzureSecretsBuilder(ingredient);
                        if (x is StorageIngredient storageIngredient) return new AzureStorageBuilder(storageIngredient);
                        if (x is OAuthClientIngredient oauthIngredient) return new OAuthClientBuilder(oauthIngredient);
                        throw new NotSupportedException();
                    }),
                new TestContextBuilder(),
                new NullLogger<Restaurant<AzurePlatformContext>>());

            return restaurant;
        }

        private class TestContextBuilder : IPlatformContextBuilder<AzurePlatformContext>
        {
            public Task<AzurePlatformContext> Build(Docket docket, PlatformEnvironment platformEnvironment)
            {
                return Task.FromResult(
                    new AzurePlatformContext(
                        docket,
                        new AzureCookerSettings(),
                        platformEnvironment));
            }
        }

        [Fact]
        public void throws_on_unknown_dependency_properties()
        {
            var storage1 = new StorageData("one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new StorageData("[one.DoesNotExist]-foofoo", Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket(Guid.NewGuid(), "Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant(out _, out _);

            Should.Throw<InvalidOperationException>(async () =>
                await restaurant.PlaceOrder(GetTestEnvironment(), docket));
        }

        [Fact]
        public void can_detect_dependency_issues()
        {
            var storage1 = new StorageData("one", Array.Empty<string>(), Array.Empty<string>(),
                Array.Empty<string>());
            var storage2 = new StorageData("[storage1.Name]-foofoo", Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>());

            var docket = new Docket(Guid.NewGuid(), "Docket", storage1, storage2);

            var restaurant = BuildTestRestaurant(out _, out _);

            Should.Throw<InvalidOperationException>(async () =>
                await restaurant.PlaceOrder(GetTestEnvironment(), docket));
        }

        [Fact]
        public void cannot_build_unknown_items()
        {
            var unknown = new UnknownItem("1");
            var docket = new Docket(Guid.NewGuid(), "Docket", unknown);

            var restaurant = BuildTestRestaurant(out _, out _);

            Should.Throw<NotSupportedException>(async () => await restaurant.PlaceOrder(GetTestEnvironment(), docket));
        }

        record UnknownItem(string Id) : IngredientData(Id)
        {
            public override IEnumerable<IIngredient> GatherIngredients()
            {
                yield return new UnknownIngredient(this);
            }

            class UnknownIngredient : IIngredient
            {
                public UnknownIngredient(UnknownItem item)
                {
                    OriginalIngredientData = item;
                    Id = item.Id;
                }

                public IngredientData OriginalIngredientData { get; }
                public string Id { get; }

                public bool PrepareForCook(IDictionary<IIngredient, ICookedIngredient> edibles)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}