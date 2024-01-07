using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearAbility : MonoBehaviour
{
    private AbilityCanvasController _abilityController;

    [SerializeField] private Image imageToChange;
    [SerializeField] private Image imageSlider;

    private Color orginalColor;

    [SerializeField] private GameObject container;

    private bool _isMouseDown;
    private bool _isUnlocked;

    [SerializeField] private int cost = 2;

    private void Awake()
    {
        orginalColor = imageToChange.color;
        _abilityController = container.GetComponent<AbilityCanvasController>();
    }

    public void PointerEnter()
    {
        imageToChange.color = Color.black;
    }

    public void PointerExit()
    {
        imageToChange.color = orginalColor;
    }

    public void PointerDown()
    {
        if (!_isUnlocked && GameManagerSingleton.Instance.gearsItem >= cost)
        {
            _isMouseDown = true;
            StartCoroutine(CircleSlider());
        }
    }

    public void PointerUp()
    {
        _isMouseDown = false;
        imageSlider.fillAmount = 0f;
    }

    private IEnumerator CircleSlider()
    {
        while (_isMouseDown && imageSlider.fillAmount < 1)
        {
            imageSlider.fillAmount += .01f;
            yield return new WaitForEndOfFrame();
        }

        imageSlider.fillAmount = 1;
        _isUnlocked = true;
        _abilityController.SetGearsCount(GameManagerSingleton.Instance.gearsItem - cost);
        imageSlider.fillAmount = 0;

    }
}
