﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LegendSharp.items
{
    public class BaseWeapon : BaseItem
    {
        public String weaponClass;
        public double damage;
        public String damageType;

        public BaseWeapon(String sprite, String name, String description, String itemId, String weaponClass, double damage, String damageType) : base(sprite, name, description, itemId, "weapon")
        {
            this.weaponClass = weaponClass;
            this.damage = damage;
            this.damageType = damageType;
        }
    }
}
