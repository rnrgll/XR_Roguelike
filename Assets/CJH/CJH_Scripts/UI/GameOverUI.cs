using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI message;

    private void Awake()
    {
        // 싱글톤 중복 방지
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("중복된 GameOverUI 인스턴스 발견.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 필드 유효성 검사
        if (panel == null)
        {
            Debug.LogError(" GameOverUI: panel 오브젝트가 할당되지 않았습니다!");
        }

        if (message == null)
        {
            Debug.LogError(" GameOverUI: message(TextMeshProUGUI)가 비어 있습니다!");
        }

        // 시작 시 패널 숨김
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);

        if (GameStateManager.Instance == null)
        {
            GameObject go = new GameObject("GameStateManager");
            go.AddComponent<GameStateManager>();
        }

        GameStateManager.Instance.CalculateReward();

        message.text = $"<size=150%><b>Game Over</size>\n\n" +
                       $"당신은 세상을 구하기에는 아직 부족했습니다.\n\n" +
                       $"제거한 죄악의 수: {GameStateManager.Instance.Wins}\n" +
                       $"획득 재화: {GameStateManager.Instance.ExternalCurrency}";

        Invoke(nameof(ReturnToTitle), 5f); // 5초 뒤 타이틀로
    }

    private void ReturnToTitle()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");

    }
}

