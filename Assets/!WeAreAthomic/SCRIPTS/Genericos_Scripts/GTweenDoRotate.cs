using DG.Tweening;
using UnityEngine;

namespace Generics.Tween
{
    public class GTweenDoRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 target;
        [SerializeField] private bool loop;
        [SerializeField] private bool WantToRotateStart;
        [SerializeField] private LoopType loopType;
        [SerializeField] private float duration;
        [SerializeField] private RotateMode rotateMode;
        [SerializeField] private Ease ease;



        private void Start()
        {
            if (WantToRotateStart)
            {
                if (loop)
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
        public void StartRotate()
        {
            if (loop)
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

        public void StopRotate()
        {
            transform.DOKill();
        }
    }
}