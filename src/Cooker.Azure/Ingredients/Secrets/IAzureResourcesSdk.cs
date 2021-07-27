﻿using System;
using System.Threading.Tasks;
using Azure.ResourceManager.Resources;
using Azure.Security.KeyVault.Secrets;
using Cooker.Ingredients;

namespace Cooker.Azure.Ingredients.Secrets
{
    public interface IAzureResourcesSdk
    {
        Task<ICookedIngredient> Execute<TOutput>(Func<ResourcesManagementClient,TOutput> action) where TOutput : ICookedIngredient;
    }

    class AzureResourcesSdk : IAzureResourcesSdk
    {
        private readonly ResourcesManagementClient _sdk;

        public AzureResourcesSdk(ResourcesManagementClient sdk)
        {
            _sdk = sdk;
        }
        public async Task<ICookedIngredient> Execute<TOutput>(Func<ResourcesManagementClient, TOutput> action) where TOutput : ICookedIngredient
        {
            return action(_sdk);
        }
    }
}