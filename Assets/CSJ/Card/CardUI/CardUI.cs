using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image CardImage;
    [SerializeField] private GameObject border;

    public bool _isSelected { get; private set; }
    public MinorArcana CardData { get; private set; }
    public Action<MinorArcana> OnClick = delegate { };

    public void Setup(MinorArcana card)
    {
        CardData = card;
        string cNum = card.CardNum.ToString();
        string cName = $"MinorArcana/{card.CardSuit}/{card.CardSuit}_{cNum}";
        var sprite = Resources.Load<Sprite>(cName);
        if (sprite != null)
        {
            CardImage.sprite = sprite;
        }
        _isSelected = false;
        border.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(CardData);
    }

    public bool ToggleSelect()
    {
        _isSelected = !_isSelected;
        border.SetActive(_isSelected);
        return _isSelected;
    }

    public void SyncSelected(bool on)
    {
        _isSelected = on;
        border.SetActive(on);
    }



}
