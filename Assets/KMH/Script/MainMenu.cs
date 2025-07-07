using Managers;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour

{
    [SerializeField] private GameInitializer gameInitializer;
    [SerializeField] private GameObject optionCanvas;

    //AudioManager audioManager;

    private void Awake()
    {
        // audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Intro");
        gameInitializer.InitializeGame();
        
    }

    public void OnClickShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void OnClickOption()
    {
       // optionCanvas.SetActive(true);
       Manager.UI.SetUIActive(GlobalUI.Setting,true);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RetrunToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToGame()
    {
        
    }
   
}
