using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GearAbility : MonoBehaviour
{
    private AbilityCanvasController _abilityController;

    private RectTransform rectTransform;
    private RectTransform canvasRect;

    [SerializeField] private Image imageToChange;
    [SerializeField] private Image imageSlider;

    private Color orginalColor;

    private Vector3 worldPosition;

    private Vector2 minPosition;
    private Vector2 maxPosition;


    [SerializeField] private Camera cameraAbility;

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

        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void Update()
    {

        if (_isShowingDescription)
        {

            var mouseVector = Mouse.current.position.ReadValue();
            worldPosition = cameraAbility.ScreenToWorldPoint(mouseVector);
            worldPosition.z = descriptionObj.transform.position.z;


            Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(worldPosition.x, -300f, 122f),
            Mathf.Clamp(worldPosition.y, 236, 415f)
    );

            descriptionObj.transform.position = clampedPosition;
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
        yield return new WaitForSecondsRealtime(1f);

        descriptionObj.SetActive(true);

        rectTransform = descriptionObj.transform.GetChild(0).GetComponent<RectTransform>();

        // Calcula los límites de la pantalla (canvas)
        Vector2 minCanvasPos = canvasRect.rect.min;
        Vector2 maxCanvasPos = canvasRect.rect.max;

        minPosition = minCanvasPos;
        maxPosition = maxCanvasPos;
        _isShowingDescription = true;
        while (_isMouseInside)
        {
            yield return new WaitForEndOfFrame();
        }
        _isShowingDescription = false;
        descriptionObj.SetActive(false);
    }
}
