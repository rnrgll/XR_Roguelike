using CardEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class NumUpdateUI : UIRequire
{
    [Header("NumPanel")]
    [SerializeField] private RectTransform NumPanel;
    [Header("SuitPanel")]
    [SerializeField] private RectTransform SuitPanel;

    private Dictionary<string, TMP_Text> numCounts;
    private Dictionary<string, TMP_Text> suitCounts;

    public override void InitializeUI(PlayerController pc)
    {
        DictionaryInit();
        Debug.Log("[NumUI] Init 호출");
        base.InitializeUI(pc);

    }

    protected override void Subscribe()
    {
        Debug.Log("[NumUI] Subsctibe 호출");
        cardController.OnCardDrawn += UpdateCountFor;
        cardController.OnCardSwapped += OnSwapCard;
        cardController.OnChangedHands += RefreshAllCounts;
    }

    protected override void UnSubscribe()
    {
        cardController.OnCardDrawn -= UpdateCountFor;
        cardController.OnCardSwapped -= OnSwapCard;
        cardController.OnChangedHands -= RefreshAllCounts;
    }
    private void DictionaryInit()
    {
        numCounts = new();
        foreach (Transform child in NumPanel)
        {
            var ctPanel = child.Find("Count")?.GetComponent<TMP_Text>();
            if (ctPanel != null)
            {
                numCounts[child.name] = ctPanel;
            }
        }

        suitCounts = new();
        foreach (Transform child in SuitPanel)
        {
            var ctPanel = child.Find("Count")?.GetComponent<TMP_Text>();
            if (ctPanel != null)
            {
                suitCounts[child.name] = ctPanel;
            }
        }
    }

    private void RefreshAllCounts()
    {
        Debug.Log("[NumUI] RefreshAllCounts 시작");
        Debug.Log($"[NumUI] controller.numbersList = {string.Join(",", cardController.numbersList)}");
        Debug.Log($"[NumUI] controller.SuitsList   = {string.Join(",", cardController.SuitsList)}");
        foreach (var keyValue in numCounts)
        {
            int idx = GetNumIndex(keyValue.Key);
            keyValue.Value.text = cardController.numbersList[idx].ToString();
        }
        foreach (var KeyValue in suitCounts)
        {
            int sidx = System.Enum.Parse<MinorSuit>(KeyValue.Key)
                is MinorSuit ms ? (int)ms : -1;
            if (sidx >= 0)
                KeyValue.Value.text = cardController.SuitsList[sidx].ToString();
        }
    }

    private void UpdateCountFor(MinorArcana card)
    {
        string numKey = card.CardNum.ToString() switch
        {
            "1" => "A",
            "11" => "J",
            "12" => "Q",
            "13" => "K",
            "14" => "H",
            var s => s
        };
        if (numCounts.TryGetValue(numKey, out var nt))
            nt.text = cardController.numbersList[card.CardNum].ToString();

        string suitKey = card.CardSuit.ToString();
        if (suitCounts.TryGetValue(suitKey, out var st))
            nt.text = cardController.SuitsList[(int)card.CardSuit].ToString();
    }

    private int GetNumIndex(string key)
    {
        return key switch
        {
            "A" => 1,
            "2" => 2,
            "3" => 3,
            "4" => 4,
            "5" => 5,
            "6" => 6,
            "7" => 7,
            "8" => 8,
            "9" => 9,
            "10" => 10,
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            "H" => 14,
            _ => 0
        };
    }


    public void OnSwapCard(MinorArcana deckCard, MinorArcana handCard)
    {
        UpdateCountFor(deckCard);

        UpdateCountFor(handCard);
    }
}
