using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishAnim : MonoBehaviour
{
    [SerializeField] GameObject exclamacion;
    public void AnimExclamacionFinish()
    {
        exclamacion.SetActive(false);
    }
}
