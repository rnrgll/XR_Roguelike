using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchant
{
    public CardEnchant enchantInfo { get; private set; }

    public Enchant()
    {
        enchantInfo = CardEnchant.none;
    }

    public void EnchantToCard(CardEnchant _enchant)
    {
        enchantInfo = _enchant;
    }
}
