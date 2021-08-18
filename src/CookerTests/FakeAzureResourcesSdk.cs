using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cooker.Azure;
using Cooker.Azure.Ingredients.Secrets;
using Cooker.Azure.KitchenStations.Sdk;
using Cooker.Ingredients;

namespace CookerTests
{
    internal class FakeAzureResourcesSdk : IAzureResourcesSdk
    {
        private readonly Stack<object> _stack = new Stack<object>();
        
        public void Seed(object obj)
        {
            _stack.Push(obj);
        }
        
        public Task<ICookedIngredient> Execute<TOutput>(AzurePlatformContext platformContext,
            Func<AzurePlatformContext, IAzureSdkProvider, Task<TOutput>> action) where TOutput : ICookedIngredient
        {
            return Task.FromResult((ICookedIngredient)_stack.Pop()!);
        }
    }
}