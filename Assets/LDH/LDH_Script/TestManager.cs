using DesignPattern;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


//Manager 테스트를 위한 임시 Test Manager 생성
public class TestManager : Singleton<TestManager>
{
    private void Awake()
    {
        SingletonInit(); //초기화
    }

    //테스트를 위한 메서드
    public void Test()
    {
        Debug.Log("Test Manager : Test() 호출");
    }
}
