using System;
using System.Collections.Generic;
using Cooker.Ingredients;
using Cooker.Ingredients.OAuth2;
using Cooker.Ingredients.Secrets;
using Shouldly;
using Xunit;

namespace CookerTests
{
    public class TheDependencyHelper
    {
        [Fact]
        public void can_navigate_property_trees_with_dictionaries()
        {
            var secretUrl = "https://somewhere.com/secrets/secret-one";
            var edible = new SecretsOutput(
                new SecretsData("", Array.Empty<string>(), Array.Empty<SecretsData.KnownSecret>()),
                "",
                new Dictionary<string, string>()
                {
                    ["secret-one"] = secretUrl
                });

            var satisfied = DepedencyHelper.IsSatisfied(
                "[secretstore.SecretUrls.secret-one]",
                new Dictionary<IIngredient, ICookedIngredient>()
                {
                    [new SecretsIngredient(
                        new SecretsData("secretstore", Array.Empty<string>(),
                            Array.Empty<SecretsData.KnownSecret>()))] = edible
                }, out var value);

            satisfied.ShouldBeTrue();
            value.ShouldBe(secretUrl);
        }

        [Fact]
        public void can_navigate_property_trees_with_lists()
        {
            var edibleApp = new OAuthClientOutput(
                new OAuthClientData(
                    "test",
                    "test",
                    Array.Empty<string>()),
                "00000",
                "12345",
                "secret");

            var oauthIngredient = new OAuthClientIngredient(edibleApp.Data);

            var edible = new SecretsData("", Array.Empty<string>(),
                new[] { new SecretsData.KnownSecret("test-secret", "[test.ClientSecret]") });

            var satisfied = edible.Clone(new Dictionary<IIngredient, ICookedIngredient>()
            {
                [oauthIngredient] = edibleApp
            }, out var secrets);

            satisfied.ShouldBeTrue();
            secrets!.KnownSecrets![0].Value.ShouldBe("secret");
        }

        [Fact]
        public void supports_embedded_expressions()
        {
            var secretUrl = "https://somewhere.com/secrets/secret-one";
            var edible = new SecretsOutput(
                new SecretsData("secretstore", Array.Empty<string>(), Array.Empty<SecretsData.KnownSecret>()),
                "",
                new Dictionary<string, string>()
                {
                    ["secret-one"] = secretUrl
                });

            var satisfied = DepedencyHelper.IsSatisfied(
                "foo-[secretstore.SecretUrls.secret-one]-bar",
                new Dictionary<IIngredient, ICookedIngredient>()
                {
                    [new SecretsIngredient(edible.Data)] = edible
                }, out var value);

            satisfied.ShouldBeTrue();
            value.ShouldBe($"foo-{secretUrl}-bar");
        }

        [Fact]
        public void can_navigate_property_trees()
        {
            var name = "secretstore";
            var edible = new SecretsOutput(
                new SecretsData(
                    name,
                    Array.Empty<string>(),
                    Array.Empty<SecretsData.KnownSecret>()
                ),
                "",
                new Dictionary<string, string>()
                {
                });

            var satisfied = DepedencyHelper.IsSatisfied(
                "[secretstore.Data.Id]",
                new Dictionary<IIngredient, ICookedIngredient>()
                {
                    [new SecretsIngredient(edible.Data)] = edible
                }, out var value);

            satisfied.ShouldBeTrue();
            value.ShouldBe(name);
        }
    }
}