using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff
{
    public CardDebuff debuffinfo { get; private set; }

    public Debuff()
    {
        debuffinfo = CardDebuff.none;
    }

    public void DebuffToCard(CardDebuff _debuff)
    {
        debuffinfo = _debuff;
    }
}

