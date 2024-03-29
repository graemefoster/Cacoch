﻿using System.Threading.Tasks;
using Cooker.Kitchens;

namespace Cooker
{
    public interface IRestaurant
    {
        Task<Meal> PlaceOrder(PlatformEnvironment environment, Docket docket);
    }
}