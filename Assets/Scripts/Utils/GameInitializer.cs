using LDH.LDH_Script;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    /// <summary>
    /// 메인 메뉴의 "게임 시작" 버튼에 연결하거나,
    /// 씬 로드 직후 자동 호출하도록 세팅하시면됩니다.
    /// </summary>
    public void InitializeGame()
    {
        Debug.Log("[Init] 게임 초기화 시작");

        // 1) GameStateManager 상태 및 인벤토리 초기화
        var gsm = GameStateManager.Instance;
        gsm.ResetState();   // Wins, BossDefeated, 보상 등 리셋
        gsm.Init();         // _itemInventory 초기화
        Debug.Log("[Init] GameStateManager 초기화 완료");

        // 2) TurnManager 초기화
        var tm = TurnManager.Instance;
        tm.ResetState();    // 플레이어·적·플래그 전부 초기화
        Debug.Log("[Init] TurnManager 초기화 완료");

        // 3) 맵 생성
        var mapMgr = MapManager.Instance;
        mapMgr.GenerateMap();
        Debug.Log("[Init] MapManager.GenerateMap() 호출 완료");

        // 4) UI 상단바 활성화
        Manager.UI.TopBarUI.SetActive(true);
        Debug.Log("[Init] TopBarUI 활성화 완료");
       
        // 5) PlayerController 초기화 (HP, 카드 등)
        var pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            pc.ResetState();  // (앞서 추가한 ResetState() 메서드)
            Debug.Log("[Init] PlayerController 초기화 완료");
        }
        else
        {
            Debug.LogWarning("[Init] PlayerController를 찾을 수 없습니다");
        }
    

        // 6) 씬 전환: 맵 씬 혹은 전투 씬
        //    프롤로그(인트로)씬으로 전환합니다.
        SceneManager.LoadScene("Intro");
        
    }

}
