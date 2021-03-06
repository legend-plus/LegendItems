﻿using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegendItems
{
    public class Item
    {
        String itemSprite;
        String itemName;
        String itemDescription;
        String itemType;
        int? maxStack;
        int quantity;

        public BaseItem baseItem;
        public Item(BaseItem baseItem, String itemSprite = null, String itemName = null, String itemDescription = null, String itemType = null, int? maxStack = null, int quantity = 1)
        {
            this.baseItem = baseItem;
            this.itemSprite = itemSprite;
            this.itemName = itemName;
            this.itemDescription = itemDescription;
            this.itemType = itemType;
            this.maxStack = maxStack;
            if (quantity <= GetMaxStack())
            {
                this.quantity = quantity;
            }
            else
            {
                this.quantity = GetMaxStack();
            }
        }

        public Item(Item item)
        {
            baseItem = item.baseItem;
            itemSprite = item.itemSprite;
            itemName = item.itemName;
            itemDescription = item.itemDescription;
            itemType = item.itemType;
            maxStack = item.maxStack;
            quantity = item.quantity;
        }

        public String GetSprite()
        {
            return itemSprite ?? baseItem.sprite;
        }

        public String GetName()
        {
            return itemName ?? baseItem.name;
        }

        public String GetDescription()
        {
            return itemDescription ?? baseItem.description;
        }

        public String GetItemType()
        {
            return itemType ?? baseItem.type;
        }

        public int GetMaxStack()
        {
            return maxStack ?? baseItem.maxStack;
        }

        public int GetQuantity()
        {
            return quantity;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= GetMaxStack())
            {
                this.quantity = quantity;
            }
        }

        public void AddQuantity(int quantity)
        {
            SetQuantity(GetQuantity() + quantity);
        }
        

        public bool HasSprite()
        {
            return itemSprite != null;
        }

        public bool HasName()
        {
            return itemName != null;
        }

        public bool HasDescription()
        {
            return itemDescription != null;
        }

        public bool HasItemType()
        {
            return itemType != null;
        }

        public bool HasMaxStack()
        {
            return maxStack != null;
        }

        public override string ToString()
        {
            return GetName();
        }

        public virtual Item GetStatic()
        {
            return new Item(null, GetSprite(), GetName(), GetDescription(), GetItemType(), GetMaxStack(), quantity);
        }

        public virtual byte[] Encode(BigEndianBitConverter converter)
        {
            byte[] spriteData = System.Text.Encoding.UTF8.GetBytes(GetSprite());
            byte[] nameData = System.Text.Encoding.UTF8.GetBytes(GetName());
            byte[] descriptionData = System.Text.Encoding.UTF8.GetBytes(GetDescription());
            byte[] typeData = System.Text.Encoding.UTF8.GetBytes(GetItemType());
            byte[] spriteLength = converter.GetBytes(spriteData.Length);
            byte[] nameLength = converter.GetBytes(nameData.Length);
            byte[] descriptionLength = converter.GetBytes(descriptionData.Length);
            byte[] typeLength = converter.GetBytes(typeData.Length);
            byte[] maxStackData = converter.GetBytes(GetMaxStack());
            byte[] quantityData = converter.GetBytes(GetQuantity());


            byte[] output = new byte[24 + spriteData.Length + nameData.Length + descriptionData.Length + typeData.Length];

            System.Buffer.BlockCopy(spriteLength, 0, output, 0, 4);
            System.Buffer.BlockCopy(spriteData, 0, output, 4, spriteData.Length);

            System.Buffer.BlockCopy(nameLength, 0, output, 4 + spriteData.Length, 4);
            System.Buffer.BlockCopy(nameData, 0, output, 8 + spriteData.Length, nameData.Length);

            System.Buffer.BlockCopy(descriptionLength, 0, output, 8 + spriteData.Length + nameData.Length, 4);
            System.Buffer.BlockCopy(descriptionData, 0, output, 12 + spriteData.Length + nameData.Length, descriptionData.Length);

            System.Buffer.BlockCopy(typeLength, 0, output, 12 + spriteData.Length + nameData.Length + descriptionData.Length, 4);
            System.Buffer.BlockCopy(typeData, 0, output, 16 + spriteData.Length + nameData.Length + descriptionData.Length, typeData.Length);

            System.Buffer.BlockCopy(maxStackData, 0, output, 16 + spriteData.Length + nameData.Length + descriptionData.Length + typeData.Length, 4);
            System.Buffer.BlockCopy(quantityData, 0, output, 20 + spriteData.Length + nameData.Length + descriptionData.Length + typeData.Length, 4);


            return output;
        }

        public static Item DecodeItem(byte[] data, BigEndianBitConverter converter, int offset = 0)
        {
            int spriteLength = converter.ToInt32(data, offset);
            string sprite = System.Text.Encoding.UTF8.GetString(data, 4 + offset, spriteLength);

            int nameLength = converter.ToInt32(data, 4 + spriteLength + offset);
            string name = System.Text.Encoding.UTF8.GetString(data, 8 + spriteLength + offset, nameLength);

            int descriptionLength = converter.ToInt32(data, 8 + spriteLength + nameLength + offset);
            string description = System.Text.Encoding.UTF8.GetString(data, 12 + spriteLength + nameLength + offset, descriptionLength);

            int typeLength = converter.ToInt32(data, 12 + spriteLength + nameLength + descriptionLength + offset);
            string type = System.Text.Encoding.UTF8.GetString(data, 16 + spriteLength + nameLength + descriptionLength + offset, typeLength);

            int maxStack = converter.ToInt32(data, 16 + spriteLength + nameLength + descriptionLength + typeLength + offset);
            int quantity = converter.ToInt32(data, 20 + spriteLength + nameLength + descriptionLength + typeLength + offset);

            if (type == "weapon")
            {
                int weaponClassLength = converter.ToInt32(data, 24 + spriteLength + nameLength + descriptionLength + typeLength + offset);
                string weaponClass = System.Text.Encoding.UTF8.GetString(data, 28 + spriteLength + nameLength + descriptionLength + typeLength + offset, weaponClassLength);

                double damage = converter.ToDouble(data, 28 + spriteLength + nameLength + descriptionLength + typeLength + weaponClassLength + offset);

                int damageTypeLength = converter.ToInt32(data, 36 + spriteLength + nameLength + descriptionLength + typeLength + weaponClassLength + offset);
                string damageType = System.Text.Encoding.UTF8.GetString(data, 40 + spriteLength + nameLength + descriptionLength + typeLength + weaponClassLength + offset, damageTypeLength);

                return new Weapon(null, sprite, name, description, type, weaponClass, damage, damageType);
            }
            else
            {
                return new Item(null, sprite, name, description, type, maxStack, quantity);
            }
        }

        public virtual bool Stackable(Item item)
        {
            if (
                item.baseItem == baseItem
                && item.GetSprite() == GetSprite()
                && item.GetName() == GetName()
                && item.GetDescription() == GetDescription()
                && item.GetItemType() == GetItemType()
                && item.GetMaxStack() == GetMaxStack()
                )
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -47655084;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(itemSprite);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(itemName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(itemDescription);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(itemType);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(maxStack);
            hashCode = hashCode * -1521134295 + quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseItem>.Default.GetHashCode(baseItem);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Item;
            return item != null &&
                   itemSprite == item.itemSprite &&
                   itemName == item.itemName &&
                   itemDescription == item.itemDescription &&
                   itemType == item.itemType &&
                   EqualityComparer<int?>.Default.Equals(maxStack, item.maxStack) &&
                   quantity == item.quantity &&
                   EqualityComparer<BaseItem>.Default.Equals(baseItem, item.baseItem);
        }
    }
}
