using System.Collections;
using UnityEngine;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class GeneratorChecker : MonoBehaviour
    {
        [SerializeField] private BoxCollider _collider;


        private void Start()
        {
            _collider = GetComponent<BoxCollider>();

            StartCoroutine(CheckAllDestroyed());
        }

        IEnumerator CheckAllDestroyed()
        {
            if (GameManagerSingleton.Instance.generatorsDestroyed >= 3)
            {
                _collider.enabled = true;
            }
            yield return new WaitForSeconds(5f);
        }

    }
}
