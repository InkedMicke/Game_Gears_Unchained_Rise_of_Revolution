using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class C_GeneratorChecker : MonoBehaviour
{
    [SerializeField] private BoxCollider _collider;


    private void Start()
    {
        _collider = GetComponent<BoxCollider>();

       StartCoroutine(CheckAllDestroyed());
    }

    IEnumerator CheckAllDestroyed()
    {
        if (GameManagerSingleton.Instance.generatorsDestroyed == 3)
        {
            _collider.enabled = true;
        }
        yield return new WaitForSeconds(5f);
    }

}
