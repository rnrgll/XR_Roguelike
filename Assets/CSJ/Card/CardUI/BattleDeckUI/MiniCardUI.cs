using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MiniCardUI : MonoBehaviour
{
    [SerializeField] private Image CardImage;
    [SerializeField] private GameObject UsedBorder;

    public MinorArcana CardData { get; private set; }
    public void Setup(MinorArcana card)
    {
        CardData = card;
        string cName;
        var folder = (card.CardNum == 14) ? "page" : "normal";
        string path = $"ArcanaTest/SuitMini/{folder}";

        Sprite[] sheet = Resources.LoadAll<Sprite>(path);
        cName = $"{card.CardSuit}_{card.CardNum}";

        var sprite = System.Array.Find(sheet, s => s.name == cName);

        if (sprite != null)
        {
            CardImage.sprite = sprite;
        }

        if (UsedBorder != null)
            UsedBorder.SetActive(false);
    }

    public void MarkUsed(bool used)
    {
        if (UsedBorder != null)
            UsedBorder.SetActive(used);
    }

}