using System;
using System.Security.Cryptography;
using System.Text;
using Cooker.Kitchens;

namespace Cooker.Azure
{
    public class AzurePlatformContext : IPlatformContext
    {
        private readonly string _randomness;

        public AzurePlatformContext(Docket docket, PlatformEnvironment environment)
        {
            _randomness = Convert.ToBase64String(SHA512
                .Create()
                .ComputeHash(Encoding.UTF8.GetBytes(docket.TableName)))[..5];

            ResourceGroupName = string.Format($"{docket.TableName}-{environment.Slug}");
        }
        
        public string ResourceGroupName { get; }
    }
}