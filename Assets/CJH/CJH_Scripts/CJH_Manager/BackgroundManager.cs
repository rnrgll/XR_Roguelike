using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesignPattern;

public class BackgroundManager : Singleton<BackgroundManager>
{
    [Header("배경")]
    [SerializeField] private Sprite bgStage1;
    [SerializeField] private Sprite bgStage2;
    [SerializeField] private Sprite bgStage3;

    [Header("UI Image")]
    [SerializeField] private Image backgroundImage;

    private Queue<Sprite> bgQueue;

    private void Awake()
    {
        // Singleton<T> 내부 Awake 대신 이곳에서 초기화
        SingletonInit();

        // 필수 할당 검사
        if (backgroundImage == null)
            Debug.LogError("[BackgroundManager] backgroundImage가 할당되지 않았습니다.");

        InitBackgroundQueue();
    }

    /// <summary>
    /// 배경 목록을 셔플하여 Queue에 담는다.
    /// </summary>
    private void InitBackgroundQueue()
    {
        var list = new List<Sprite> { bgStage1, bgStage2, bgStage3 };
        Shuffle(list);
        bgQueue = new Queue<Sprite>(list);
    }

    /// <summary>
    /// Fisher–Yates 셔플
    /// </summary>
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    /// <summary>
    /// 다음 배경을 꺼내어 Image에 할당한다.
    /// 호출할 때마다 중복 없이 3번까지 지원.
    /// </summary>
    public void ShowNextBackground()
    {
        if (backgroundImage == null)
        {

            return;
        }

        if (bgQueue.Count == 0)
        {

            return;
        }

        var nextBg = bgQueue.Dequeue();
        backgroundImage.sprite = nextBg;
    }
}