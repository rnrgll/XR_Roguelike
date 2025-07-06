using Event.Dialogue;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Prologue : MonoBehaviour
{
    void Start()
    {
        //맵 배경에 보이게
        Manager.Map.ShowMap();
        
        //다이얼로그는 additive로 처리
        Manager.Dialogue.PlayDialogue(DialogueType.Prologue);
        
        //다이얼로그가 끝나면 맵 선택이 이루어진다.
        
    }

}
