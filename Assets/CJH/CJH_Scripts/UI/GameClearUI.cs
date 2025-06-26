using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameClearUI : MonoBehaviour
{
    public static GameClearUI Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI message;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
        GameStateManager.Instance.CalculateReward();

        message.text = $"Game Clear!\n\n" +
                       $"당신은 세상을 구할 수 있었습니다.\n" +
                       $"제거한 죄악의 수 : {GameStateManager.Instance.Wins} (보스 포함)\n" +
                       $"획득 재화 : {GameStateManager.Instance.ExternalCurrency}";

        Invoke(nameof(ReturnToTitle), 5f);
    }

    private void ReturnToTitle()
    {
        //메인 메뉴 씬이랑 연결
        // 추후 메인 메뉴 씬 이름이 바뀌게 된다면 수정.
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}