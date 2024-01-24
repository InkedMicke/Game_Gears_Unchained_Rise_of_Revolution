using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTweenDoRotate : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] private bool loop;
    [SerializeField] private LoopType loopType;
    [SerializeField] private float duration;
    [SerializeField] private RotateMode rotateMode;
    [SerializeField] private Ease ease;

    private void Start()
    {
        if(loop)
        {
            transform.DORotate(target, duration, rotateMode)
                .SetLoops(-1, loopType)
                .SetRelative()
                .SetEase(ease);
        }
        else
        {
            transform.DORotate(target, duration, rotateMode)
                .SetRelative()
                .SetEase(ease);
        }

    }
}
