using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class G_ButtonDoScale : MonoBehaviour
{
    [SerializeField] Vector3 target = Vector3.one;
    [SerializeField] float time = 1f;
    [SerializeField] Ease easing;
    [SerializeField] bool timeScaleIndependent = true ;
   
    public void SetObjScale()
    {
        
        transform.DOKill();
        transform.DOScale(target, time).SetEase(easing).SetUpdate(timeScaleIndependent);
    }
  

}
