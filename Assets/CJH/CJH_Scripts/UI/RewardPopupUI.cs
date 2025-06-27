using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopupUI : MonoBehaviour
{
    public static RewardPopupUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private RewardCardUI[] itemCards; // 3개

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panel.SetActive(false);
    }
    
    public void Show()
    {
        panel.SetActive(true);

        // 골드 지급
        int gold = RewardDB.Instance.GetFixedGold();
        GameStateManager.Instance.AddGold(gold);
        goldText.text = $"골드 +{gold}";

        // 아이템 보상 뽑기
        var items = RewardDB.Instance.GetRandomItemRewards(3);

        for (int i = 0; i < itemCards.Length; i++)
        {
            itemCards[i].Set(items[i], () =>
            {
                SelectItemReward(items[i]);
            });
        }
    }
    
    private void SelectItemReward(ItemReward reward)
    {
        Debug.Log($"아이템 획득: {reward.name}");

        // TODO: 인벤토리에 추가
        //InventoryManager.Instance.AddItem(reward);

        // 선택 후 창 닫기
        //panel.SetActive(false);

        // 다음 진행
        TurnManager.Instance.ContinueAfterReward();
    }
}