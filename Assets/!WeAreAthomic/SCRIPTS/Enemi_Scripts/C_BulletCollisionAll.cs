using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{

    public class C_BulletCollisionAll : MonoBehaviour
    {
        [SerializeField] private GameObject hitChispas;
        
        

        private void OnCollisionEnter(Collision collision)
        {
            Instantiate(hitChispas, transform.position, Quaternion.identity);

            Destroy(gameObject.transform.parent);
        }

    }
}
