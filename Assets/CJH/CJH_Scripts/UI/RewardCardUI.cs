using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button button;

    public void Set(ItemReward reward, UnityAction onClick)
    {
        icon.sprite = reward.icon;
        nameText.text = reward.name;
        descText.text = reward.description;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }
}