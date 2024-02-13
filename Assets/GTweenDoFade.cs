using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GTweenDoFade : MonoBehaviour
{
    private CanvasGroup m_canvasGroup;

    [SerializeField] private float m_endValue;
    [SerializeField] private float m_duration;

    [SerializeField] private UnityEvent m_onFinish;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Fade()
    {
        m_canvasGroup.DOFade(m_endValue, m_duration).OnComplete(() => m_onFinish.Invoke());
    }
}
