using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUI : MonoBehaviour
{
    [SerializeField] private CardEnchant CardEnchant;
    public CardEnchant cardEnchant => CardEnchant;
    [SerializeField] private CardDebuff CardDebuff;
    public CardDebuff cardDebuff => CardDebuff;

}
