﻿using Cooker.Kitchens;

namespace Cooker.Recipes.Storage
{
    public class StorageOutput : ILineItemOutput
    {
        public StorageOutput(ILineItem input, string name)
        {
            Input = input;
            Name = name;
        }

        public ILineItem Input { get; }
        public string Name { get; }
    }
}