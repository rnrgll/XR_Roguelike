using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotDeckUI : UIRequire
{
    private TarotDeck tarotDeck;
    [SerializeField] private MajorArcanaUI cardUIPrefab;


    [Header("카드를 표시할 부모 트랜스폼")]
    [SerializeField] private RectTransform cardParent;

    private MajorArcanaUI currentCardUI;



    /// <summary>
    /// 덱에서 한 장 뽑아서 화면에 인스턴스화 + 초기화
    /// </summary>
    public void DrawAndShow()
    {
        // 1) 기존에 떠 있던 카드가 있으면 제거
        if (currentCardUI != null)
        {
            Destroy(currentCardUI.gameObject);
            currentCardUI = null;
        }

        // 2) 덱에서 뽑기
        var so = tarotDeck.Draw();

        // 3) UI 인스턴스화
        currentCardUI = Instantiate(cardUIPrefab, cardParent);

        // 4) 그 카드 SO 로 설정
        currentCardUI.Setup(so);

        // 5) 카드 방향(Upright/Reversed) 반영
        currentCardUI.SetCardPosition();

    }

    private void OnMajorCardClicked(MajorArcanaSO so)
    {
        Debug.Log("클릭");
        // 클릭하면 SO의 Activate 실행
        so.Activate();
    }

    public override void InitializeUI(PlayerController pc)
    {
        tarotDeck = pc.tarotDeck;
        DrawAndShow();
        base.InitializeUI(pc);
    }

    protected override void Subscribe()
    {
        currentCardUI.OnClick += OnMajorCardClicked;
        Manager.UI.OnToggleClicked += SetActive;
    }

    protected override void UnSubscribe()
    {
        currentCardUI.OnClick -= OnMajorCardClicked;
        Manager.UI.OnToggleClicked -= SetActive;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
