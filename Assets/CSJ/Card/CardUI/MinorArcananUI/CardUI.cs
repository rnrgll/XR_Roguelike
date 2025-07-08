using CardEnum;
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
    [SerializeField] private List<LayerUI> EnchantLayers;
    [SerializeField] private List<LayerUI> DebuffLayers;
    private Dictionary<CardEnchant, LayerUI> EnchantLayerDic;
    private Dictionary<CardDebuff, LayerUI> DebuffDic;

    public bool _isSelected { get; private set; }
    public MinorArcana CardData { get; private set; }
    public Action<MinorArcana> OnClick = delegate { };

    void Awake()
    {
        EnchantLayerDic = new();
        foreach (var layer in EnchantLayers)
        {
            EnchantLayerDic[layer.cardEnchant] = layer;
        }

        DebuffDic = new();
        foreach (var layer in DebuffLayers)
        {
            DebuffDic[layer.cardDebuff] = layer;
        }
    }

    public void Setup(MinorArcana card)
    {
        CardData = card;
        Sprite sprite;
        if (card.sprite == null)
        {
            string cNum = card.CardNum.ToString();
            string cName = $"MinorArcana/{card.CardSuit}/{card.CardSuit}_{cNum}";
            sprite = Resources.Load<Sprite>(cName);
        }
        else sprite = card.sprite;

        if (sprite != null)
        {
            CardImage.sprite = sprite;
        }
        _isSelected = false;
        border.SetActive(false);
        SetLayer(card);
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

    private void SetLayer(MinorArcana card)
    {
        foreach (var EnchatLayerKV in EnchantLayerDic)
        {
            EnchatLayerKV.Value.gameObject.
            SetActive(EnchatLayerKV.Key == card.Enchant.enchantInfo);
        }
        foreach (var DebuffLayerKV in DebuffDic)
        {
            DebuffLayerKV.Value.gameObject.
            SetActive(DebuffLayerKV.Key == card.debuff.debuffinfo);
        }
    }

}
