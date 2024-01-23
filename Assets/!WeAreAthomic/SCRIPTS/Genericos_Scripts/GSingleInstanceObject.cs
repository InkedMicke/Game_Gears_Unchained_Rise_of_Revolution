using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Genericos_Scripts
{
    public class GSingleInstanceObject : MonoBehaviour
    {
        [SerializeField] private string id;
        static Dictionary<string, GameObject> instances = new Dictionary<string, GameObject>();
        private void Awake()
        {
            if (instances.ContainsKey(id))
            {
                Destroy(gameObject);
                return;
            }

            instances.Add(id, gameObject);

            DontDestroyOnLoad(gameObject);
        }
    }
}