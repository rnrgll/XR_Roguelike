using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class RandomManager : Singleton<RandomManager>
{
    private void Awake() => SingletonInit();


    /// <summary>
    /// 메이저 아르카나의 정방향과 역방향을 전환해주는 역할을 한다. 
    /// </summary>
    /// <returns> 방향을 반환한다.</returns>
    public MajorPosition RandomPosition()
    {
        int posDecNum = Random.Range(0, 2);
        if (posDecNum == 0) return MajorPosition.Upright;
        else return MajorPosition.Reversed;
    }

    /// <summary>
    /// 랜덤하게 정수를 반환해준다.
    /// </summary>
    /// <param name="min"> 최솟값, 해당 값 이상의 값이 반환된다</param>
    /// <param name="max"> 최댓값, 해당 값 미만의 값까지 반환된다.</param>
    /// <return> min <= x < max의 값이 리턴된다. </return>
    public int RandInt(int min, int max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// min 이상, max 이하 범위의 랜덤한 float을 반환
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public float RandFloat(float min, float max)
    {
        return Random.Range(min, max);
    }

}