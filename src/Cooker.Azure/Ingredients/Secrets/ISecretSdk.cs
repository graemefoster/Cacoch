using System;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Cooker.Ingredients;

namespace Cooker.Azure.Ingredients.Secrets
{
    public interface ISecretSdk
    {
        Task<ICookedIngredient> Execute<TOutput>(Func<SecretClient,TOutput> action) where TOutput : ICookedIngredient;
    }

    class AzureKeyVaultSecretSdk : ISecretSdk
    {
        private readonly SecretClient _client;

        public AzureKeyVaultSecretSdk(SecretClient client)
        {
            _client = client;
        }
        public async Task<ICookedIngredient> Execute<TOutput>(Func<SecretClient, TOutput> action) where TOutput : ICookedIngredient
        {
            return action(_client);
        }
    }
}