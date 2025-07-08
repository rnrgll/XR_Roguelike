using System.Collections.Generic;
using UnityEngine;
using DesignPattern;  // Singleton<T> 정의된 네임스페이스
using UnityEngine.SceneManagement;

public class BackgroundManager : Singleton<BackgroundManager>
{
    [Header("배경으로 사용할 프리팹들")]
    [Tooltip("씬에 배치할 배경 Prefab 등럭")]
    [SerializeField] private List<GameObject> backgroundPrefabs;

    [Header("배경을 담을 부모 Transform")]
    [Tooltip("빈 GameObject를 만들어 할당하면, 그 자식으로 배경이 생성됩니다.")]
    [SerializeField] private Transform backgroundContainer;

    private Queue<GameObject> bgQueue;
    private GameObject currentBackground;

    private void Awake()
    {
        // Singleton 초기화
        SingletonInit();

        // 모든 씬에서 BackgroundManager가 살아 있게
        DontDestroyOnLoad(gameObject);       

        // 배경 순서 셔플 후 Queue에 담기
        InitBackgroundQueue();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitBackgroundQueue()
    {
        var list = new List<GameObject>(backgroundPrefabs);
        Shuffle(list);
        bgQueue = new Queue<GameObject>(list);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            // 활성화하고 다음 배경 보여주기
            gameObject.SetActive(true);
            ShowNextBackground();
        }
        else
        {
            // 그 외 씬에선 숨기고, 남은 배경도 클리어
            gameObject.SetActive(false);
            if (currentBackground != null)
                Destroy(currentBackground);
        }
    }


    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    /// <summary>
    /// 남은 배경이 있으면 하나 꺼내어 Instantiate하고, 
    /// 기존 배경이 있다면 파괴한다.
    /// </summary>
    public void ShowNextBackground()
    {
        if (bgQueue == null || bgQueue.Count == 0)
        {
            return;
        }

        // 이전 배경 제거
        if (currentBackground != null)
            Destroy(currentBackground);

        // 다음 배경 생성
        var prefab = bgQueue.Dequeue();
        currentBackground = Instantiate(prefab, backgroundContainer);

        // 위치·회전·스케일 초기화 (필요시)
        currentBackground.transform.localPosition = Vector3.zero;
        currentBackground.transform.localRotation = Quaternion.identity;
        currentBackground.transform.localScale = Vector3.one;

        Debug.Log($"[BackgroundManager] 배경 교체 → {prefab.name}");
    }
}