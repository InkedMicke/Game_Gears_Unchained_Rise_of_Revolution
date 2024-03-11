using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GDollyCartNormalized : MonoBehaviour
{
    [SerializeField] CinemachineDollyCart virtualCamera;

    [SerializeField] float dollySpeed = 2f;

    [SerializeField] UnityEvent onFinish;
     
    public void StartDolly()
    {
        StartCoroutine(Dolly());
    }

    IEnumerator Dolly()
    {
        while(virtualCamera.m_Position < 1)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("hola1");
            virtualCamera.m_Position += dollySpeed * Time.deltaTime;
        }

        onFinish?.Invoke();
    }
    
}
