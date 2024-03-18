using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Generics.Tween
{
    public class GTweenDoMoveTarget : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;
        [SerializeField] private bool playOnStart;
        [SerializeField] private UnityEvent playOnComplete;
        private void Start()
        {
            if (playOnStart)
            {
                transform.DOMove(target.position, duration).SetEase(ease).OnComplete(() => playOnComplete.Invoke());
            }
        }

        public void Move()
        {
            transform.DOMove(target.position, duration).SetEase(ease).OnComplete(() => playOnComplete.Invoke());
        }
    }
}
