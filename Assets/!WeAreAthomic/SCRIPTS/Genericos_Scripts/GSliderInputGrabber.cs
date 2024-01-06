using UnityEngine;
using UnityEngine.UI;

public class GSliderInputGrabber : MonoBehaviour
{
    private Slider _slider;

    public MonoBehaviour targetScript;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SendDataToTargetScript(string functionName)
    {
        targetScript.SendMessage(functionName, _slider.value);
    }
}
