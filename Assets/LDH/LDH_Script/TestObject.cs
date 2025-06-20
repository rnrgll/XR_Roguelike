using DesignPattern;
using System;
using System.Collections;
using UnityEngine;

namespace LDH.LDH_Script
{
    public class TestObject : PooledObject
    {
        public void Play()
        {
            StartCoroutine(ReturnAuto());
        }

        private IEnumerator ReturnAuto()
        {
            yield return new WaitForSeconds(2f);
            
            ReturnPool();
        }
    }
}