using TMPro;
using UnityEngine;

public class AbilityCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_gearCount;

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
}
