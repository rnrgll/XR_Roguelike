using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEffect/Debuff/Rust")]
public class RustSO : CardDebuffSO
{
    [SerializeField] private int panelty = 50;
    public override void OnCardPlayed(MinorArcana card, CardController controller)
    {
        controller.AddPanelty(panelty);
    }

}
