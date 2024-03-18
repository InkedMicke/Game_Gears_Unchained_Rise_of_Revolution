using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generics
{
    public class GDestroyObject : MonoBehaviour
    {
        public void DestroyThisObject(float value)
        {
            Invoke(nameof(DestroyThis), value);
        }

        private void DestroyThis()
        {
            Destroy(gameObject);
        }
    }
}