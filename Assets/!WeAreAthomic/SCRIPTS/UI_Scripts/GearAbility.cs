using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GearAbility : MonoBehaviour
{
    private AbilityCanvasController _abilityController;

    [SerializeField] private Image imageToChange;
    [SerializeField] private Image imageSlider;

    private Color orginalColor;

    [SerializeField] private GameObject container;
    [SerializeField] private GameObject descriptionObj;

    private bool _isMouseDown;
    private bool _isMouseInside;
    private bool _isUnlocked;
    private bool _isShowingDescription;

    [SerializeField] private int cost = 2;

    private void Awake()
    {
        orginalColor = imageToChange.color;
        _abilityController = container.GetComponent<AbilityCanvasController>();
    }

    private void Update()
    {
        if (_isShowingDescription)
        {
            var mouseVector = Mouse.current.delta.ReadValue();
            descriptionObj.transform.position = new Vector3(mouseVector.x, mouseVector.y, descriptionObj.transform.position.z);
        }
    }

    public void PointerEnter()
    {
        _isMouseInside = true;
        imageToChange.color = Color.black;
        StartCoroutine(ShowDescription());
    }

    public void PointerExit()
    {
        _isMouseInside = false;
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

    private IEnumerator ShowDescription()
    {
        yield return new WaitForSeconds(2f);
        descriptionObj.SetActive(true);
        _isShowingDescription = true;
        while (_isMouseInside)
        {
            yield return new WaitForEndOfFrame();
        }
        _isShowingDescription = false;
        descriptionObj.SetActive(false);
    }
}
