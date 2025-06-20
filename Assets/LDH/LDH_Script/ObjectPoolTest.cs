using DesignPattern;
using System;
using UnityEngine;

namespace LDH.LDH_Script
{
    public class ObjectPoolTest : MonoBehaviour
    {
        public TestObject _testPrefab;
        private ObjectPool _testPool;
        
        private void Awake() => Init();

        private void Init()
        {

            _testPool = new ObjectPool(transform, _testPrefab, 3);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PooledObject obj = _testPool.PopPool();
                obj.GetComponent<TestObject>().Play();
                obj.transform.SetParent(transform);
                
            }
        }
    }
}