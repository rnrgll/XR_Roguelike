using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MajorArcanaUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image CardImage;


    public MajorArcanaSO CardData { get; private set; }
    public Action<MajorArcanaSO> OnClick = delegate { };

    public void Setup(MajorArcanaSO card)
    {
        CardData = card;
        string cName = $"MajorArcana/{card.cardName}";
        Debug.Log(cName);
        var sprite = Resources.Load<Sprite>(cName);
        Debug.Log(sprite);
        if (sprite != null)
        {
            CardImage.sprite = sprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(CardData);
    }

    public void SetCardPosition()
    {
        if (CardData.cardPos == CardEnum.MajorPosition.Upright)
        {
            CardImage.transform.Rotate(new Vector2(0, 180));
        }
        else CardImage.transform.Rotate(new Vector2(0, 0));
    }
}
