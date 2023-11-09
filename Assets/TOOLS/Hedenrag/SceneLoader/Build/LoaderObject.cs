using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hedenrag
{
    namespace SceneLoader
    {
        public class LoaderObject : MonoBehaviour
        {
            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}