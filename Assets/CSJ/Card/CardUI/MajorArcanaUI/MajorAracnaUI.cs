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
        var sprite = Resources.Load<Sprite>(cName);
        if (sprite != null)
        {
            CardImage.sprite = sprite;
        }
    }

    public void SetCardImage(Sprite sprite)
    {
        CardImage.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(CardData);
    }

    public void SetCardPosition()
    {
        if (CardData.cardPos == CardEnum.MajorPosition.Upright)
        {
            CardImage.rectTransform.Rotate(new Vector3(0, 0, 180));
        }
        else CardImage.rectTransform.Rotate(new Vector3(0, 0, 0));
    }
}
