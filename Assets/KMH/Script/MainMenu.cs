using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickStart()
    {
        Debug.Log("게임시작");
    }

    public void OnClickShop()
    {
        Debug.Log("상점");
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
   
}
