using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameInitializer gameInitializer;
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
        SceneManager.LoadScene("Option");
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
        SceneManager.LoadScene(1);
    }
   
}
