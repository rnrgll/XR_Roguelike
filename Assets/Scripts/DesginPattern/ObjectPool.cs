using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DesignPattern
{
    public class ObjectPool
    {
        private Stack<PooledObject> _stack;
        private PooledObject _targetPrefab;
        private GameObject _poolObject;
        private int _poolSize;

        public ObjectPool(Transform parent, PooledObject targetPrefab, int poolSize) => Init(parent, targetPrefab, poolSize);

        private void Init(Transform parent, PooledObject targetPrefab, int poolSize)
        {
            _stack = new Stack<PooledObject>(poolSize);
            _targetPrefab = targetPrefab;
            _poolObject = new GameObject($"{targetPrefab.name} Pool");
            _poolObject.transform.parent = parent;

            for (int i = 0; i < poolSize; i++)
            {
                CreatePooledObject();
            }
        }

        public PooledObject PopPool()
        {
            if (_stack.Count == 0)
            {
               PooledObject newObj = MonoBehaviour.Instantiate(_targetPrefab);
               //풀 초기화하지 않고 반환
                
               return newObj;
            }

            PooledObject pooledObject = _stack.Pop();
            pooledObject.gameObject.SetActive(true);
            return pooledObject;
        }

        public void PushPool(PooledObject target)
        {
            target.transform.SetParent(_poolObject.transform);
            target.gameObject.SetActive(false);
            _stack.Push(target);
        }

        private void CreatePooledObject()
        {
            PooledObject obj = MonoBehaviour.Instantiate(_targetPrefab);
            obj.PooledInit(this);
            PushPool(obj);
        }
    }
}