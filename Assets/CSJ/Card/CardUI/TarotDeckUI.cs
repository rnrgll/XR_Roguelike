using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotDeckUI : UIRequire
{
    private TarotDeck tarotDeck;
    [Header("카드 앞면 UI")]
    [SerializeField] private MajorArcanaUI cardUIPrefab;


    [Header("카드를 표시할 부모 트랜스폼")]
    [SerializeField] private RectTransform cardParent;
    [Header("카드 뒷면 스프라이트")]
    [SerializeField] private Sprite cardBackSprite;

    private MajorArcanaUI currentCardUI;
    private bool isUsed = false;

    public override void InitializeUI(PlayerController pc)
    {
        tarotDeck = pc.tarotDeck;
        DrawAndShow();
        base.InitializeUI(pc);
    }


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
        isUsed = false;

        // 3) UI 인스턴스화
        currentCardUI = Instantiate(cardUIPrefab, cardParent);

        // 4) 그 카드 SO 로 설정
        currentCardUI.Setup(so);

        // 5) 카드 방향(Upright/Reversed) 반영
        currentCardUI.SetCardPosition();

        currentCardUI.OnClick -= OnMajorCardClicked;
        currentCardUI.OnClick += OnMajorCardClicked;

    }

    private void OnMajorCardClicked(MajorArcanaSO so)
    {
        if (isUsed) return;
        isUsed = true;


        Debug.Log("클릭");
        // 클릭하면 SO의 Activate 실행
        so.Activate();

        currentCardUI.OnClick -= OnMajorCardClicked;

        currentCardUI.SetCardImage(cardBackSprite);
    }


    protected override void Subscribe()
    {
        currentCardUI.OnClick += OnMajorCardClicked;
        Manager.UI.OnGlobalUIActive += SetActive;
        playerController.OnTurnStarted += OnTurnStarted;
        playerController.OnTurnEnd += OnTurnEnded;

    }

    protected override void UnSubscribe()
    {
        currentCardUI.OnClick -= OnMajorCardClicked;
        //Manager.UI.OnGlobalUIActive -= SetActive;  //on disable에서 구독해제하면 다시 major deck이 활성화되지 않아서 수정
        if (playerController != null)
            playerController.OnTurnStarted -= OnTurnStarted;
    }

    private void OnDestroy()
    {
        Manager.UI.OnGlobalUIActive -= SetActive;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(!isActive);
    }

    private void OnTurnStarted()
    {
        DrawAndShow();
    }
    private void OnTurnEnded()
    {
        isInteractable = false;
    }

    private void OnBattleEnded()
    {
        Destroy(this);
    }
}
