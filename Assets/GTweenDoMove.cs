using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class GTweenDoMove : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private bool playOnStart;
    [SerializeField] private UnityEvent playOnComplete;
    private void Start()
    {
        if (playOnStart)
        {
            transform.DOMove(transform.position + target, duration).SetEase(ease).OnComplete(() => playOnComplete.Invoke());
        }
    }

    public void Move()
    {
        transform.DOMove(transform.position + target, duration).SetEase(ease).OnComplete(() => playOnComplete.Invoke());
    }
}
