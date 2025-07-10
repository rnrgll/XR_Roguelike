using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameClearUI : MonoBehaviour
{
        public static GameClearUI Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI message;

    private void Awake()
        {
            // 싱글톤 등록
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("중복된 GameClearUI 인스턴스 발견.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // 오브젝트가 꺼지지 않았을 때만 초기화
            if (panel == null)
                Debug.LogError(" GameClearUI: panel이 연결되지 않았습니다!");
            if (message == null)
                Debug.LogError(" GameClearUI: message가 연결되지 않았습니다!");

            // 시작 시 패널 숨김
            if (panel != null)
                panel.SetActive(false);
        }

        public void Show()
        {
            if (panel == null || message == null)
            {
                Debug.LogError(" GameClearUI: Show 호출 시 필드 누락");
                return;
            }

        panel.SetActive(true);

            GameStateManager.Instance.CalculateReward();

            message.text = $"<size=150%><b>Game Clear!</b></size>\n\n" +
                           $"당신은 세상을 구하였습니다.\n\n" +
                           $"<b>제거한 죄악의 수:</b> {GameStateManager.Instance.Wins}\n" +
                           $"<b>획득 재화:</b> {GameStateManager.Instance.ExternalCurrency}";

            Invoke(nameof(ReturnToTitle), 5f);
        }

        private void ReturnToTitle()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
