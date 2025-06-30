using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인챈트를 담당하는 클래스
// 내부 정보로 인챈트의 정보를 다루고 있다.
// TODO : 추후 각 인챈트 별로 특수 효과를 구현할 예정이다.
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
