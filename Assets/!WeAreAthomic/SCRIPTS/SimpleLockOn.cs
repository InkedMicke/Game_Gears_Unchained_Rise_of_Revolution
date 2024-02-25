using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLockOn : MonoBehaviour
{
    [SerializeField] private Transform m_target;

    private void OnEnable()
    {
        if (m_target == null) m_target = Camera.main.transform;
        StartCoroutine(LookAtTarget());
    }

    private IEnumerator LookAtTarget()
    {
        while(this.gameObject.activeInHierarchy)
        {
            var _dir = m_target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(_dir);
            yield return null;
        }
    }
}
