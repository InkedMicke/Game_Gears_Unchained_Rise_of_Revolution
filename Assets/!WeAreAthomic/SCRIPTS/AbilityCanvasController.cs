using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class AbilityCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_gearCount;

    [SerializeField] private VideoPlayer m_videoPlayer;

    public CurrentAbility currentAbility;

    private void Awake()
    {
        m_gearCount.text = GameManagerSingleton.Instance.gearsItem.ToString();
    }

    public void SetGearsCount(int value)
    {
        m_gearCount.text = value.ToString();
        GameManagerSingleton.Instance.gearsItem = value;
    }

    public void SetVideoPlayerClip(VideoClip clip)
    {
        m_videoPlayer.clip = clip;
    }
}
