using System;
using System.Collections.Generic;
using Cooker.Ingredients;
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
            var edible = new SecretsOutput("", "", new Dictionary<string, string>()
            {
                ["secret-one"] = secretUrl
            });

            var satisfied = DepedencyHelper.IsSatisfied(
                "[secretstore.SecretUrls.secret-one]",
                new Dictionary<IIngredient, ICookedIngredient>()
                {
                    [new SecretsIngredient(new SecretsData("secretstore", "", Array.Empty<string>()))] = edible
                }, out var value);

            satisfied.ShouldBeTrue();
            value.ShouldBe(secretUrl);
        }

        [Fact]
        public void supports_embedded_expressions()
        {
            var secretUrl = "https://somewhere.com/secrets/secret-one";
            var edible = new SecretsOutput("", "", new Dictionary<string, string>()
            {
                ["secret-one"] = secretUrl
            });

            var satisfied = DepedencyHelper.IsSatisfied(
                "foo-[secretstore.SecretUrls.secret-one]-bar",
                new Dictionary<IIngredient, ICookedIngredient>()
                {
                    [new SecretsIngredient(new SecretsData("secretstore", "", Array.Empty<string>()))] = edible
                }, out var value);

            satisfied.ShouldBeTrue();
            value.ShouldBe($"foo-{secretUrl}-bar");
        }

        [Fact]
        public void can_navigate_property_trees()
        {
            var name = "secretstore";
            var edible = new SecretsOutput("", name, new Dictionary<string, string>()
            {
            });

            var satisfied = DepedencyHelper.IsSatisfied(
                "[secretstore.Name]",
                new Dictionary<IIngredient, ICookedIngredient>()
                {
                    [new SecretsIngredient(new SecretsData("secretstore", name, Array.Empty<string>()))] = edible
                }, out var value);

            satisfied.ShouldBeTrue();
            value.ShouldBe(name);
        }
    }
}