using LegendSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendItems
{
    public class Inventory
    {
        //Pretty Simple, For Now.
        public List<Item> items = new List<Item>();

        public List<Game> openedBy = new List<Game>();

        public Guid guid;

        public Inventory(Guid guid)
        {
            this.guid = guid;
        }

        public Inventory()
        {
            guid = Guid.NewGuid();
        }
        
        public void AddItem(Item item, bool update = true)
        {
            items.Add(item);
            if (update)
            {
                foreach (Game game in openedBy)
                {
                    game.AddToInventory(guid, item, items.Count - 1);
                }
            }
        }
    }
}
