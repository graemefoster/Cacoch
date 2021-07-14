﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cooker.Kitchens.Azure
{
    public interface IArmRunner
    {
        Task<object> Execute(string template, Dictionary<string, object> parameters);
    }
}