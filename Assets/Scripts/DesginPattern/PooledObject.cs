using System.Collections;
using System.Collections.Generic;
using UnityEditor.VisionOS;
using UnityEngine;

namespace DesignPattern
{
    public abstract class PooledObject : MonoBehaviour
    {
        public ObjectPool ObjPool { get; private set; }

        public void PooledInit(ObjectPool objPool)
        {
            ObjPool = objPool;
        }

        public void ReturnPool()
        {
            if (ObjPool == null)
            {
                Destroy(this.gameObject);
                return;
            }
            ObjPool.PushPool(this);
        }
    }
}