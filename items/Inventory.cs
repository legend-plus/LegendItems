﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp.items
{
    public class Inventory
    {
        //Pretty Simple, For Now.
        public List<Item> items = new List<Item>();
        
        public void AddItem(Item item)
        {
            items.Add(item);
        }
    }
}