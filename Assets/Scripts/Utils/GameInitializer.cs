using LDH.LDH_Script;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(GameSetting());
    }
    
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
        
    }



    public IEnumerator GameSetting()
    {
        //일단 모든 클릭  이벤트를 막기
        EventSystem.current.enabled = false;
        
        //씬 비동기 로드 시작
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("Intro", LoadSceneMode.Single);
        loadOp.allowSceneActivation = false;
        
        //아래 작업이 끝나야 씬 전환시키기 + 클릭이벤트 다시 활성화
        
        //플레이어 프리팹 생성
        PlayerController playerPrefab = Resources.Load<PlayerController>("Prefabs/Player");
        if (playerPrefab == null)
        {
            Debug.LogError("[GameSetting] Player 프리팹 로드를 실패했습니다");
            yield break;
        }
        PlayerController player = GameObject.Instantiate(playerPrefab);
        player.transform.SetParent(Manager.manager.transform);

        yield return player.StartSetting();

        InitializeGame();
        
        // 씬 전환 허용
        loadOp.allowSceneActivation = true;
        
    }
}
