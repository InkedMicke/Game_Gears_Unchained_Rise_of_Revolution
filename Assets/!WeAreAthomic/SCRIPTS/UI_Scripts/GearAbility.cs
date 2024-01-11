using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GearAbility : MonoBehaviour
{
    private AbilityCanvasController _abilityController;

    private MeshRenderer _meshRenderer;

    private RectTransform canvasRect;

    [SerializeField] private Image imageToChange;
    [SerializeField] private Image imageSlider;

    [SerializeField] private Material selectedMat;
    private Material _originalMat;

    private Color orginalColor;

    private Vector3 worldPosition;


    [SerializeField] private Camera cameraAbility;

    [SerializeField] private GameObject container;
    [SerializeField] private GameObject descriptionObj;

    [SerializeField] private Transform middlePos;
    [SerializeField] private Transform topPos;
    [SerializeField] private Transform leftPos;
    [SerializeField] private Transform rightPos;
    [SerializeField] private Transform bottomPos;

    private bool _isMouseDown;
    private bool _isMouseInside;
    private bool _isUnlocked;
    private bool _isShowingDescription;

    [SerializeField] private int cost = 2;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        orginalColor = imageToChange.color;
        _originalMat = _meshRenderer.material;
        _abilityController = container.GetComponent<AbilityCanvasController>();

        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void Update()
    {

        if (_isShowingDescription)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                var mouseVector = Mouse.current.position.ReadValue();
                worldPosition = cameraAbility.ScreenToWorldPoint(mouseVector);
                worldPosition.z = descriptionObj.transform.position.z;
                Debug.Log(worldPosition);
            }
            else
            {
                worldPosition = middlePos.position;
                worldPosition.z = descriptionObj.transform.position.z;
            }

            Vector3 clampedPosition = new (
            Mathf.Clamp(worldPosition.x, leftPos.position.x, rightPos.position.x),
            Mathf.Clamp(worldPosition.y, bottomPos.position.y, topPos.position.y),
            worldPosition.z
    );

            descriptionObj.transform.position = clampedPosition;
        }
    }

    public void PointerEnter()
    {
        _isMouseInside = true;
        imageToChange.color = Color.black;
        _meshRenderer.material = selectedMat;
        StartCoroutine(ShowDescription());
    }

    public void PointerExit()
    {
        _isMouseInside = false;
        imageToChange.color = orginalColor;
        _meshRenderer.material = _originalMat;
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
        _isShowingDescription = true;
        while (_isMouseInside)
        {
            yield return new WaitForEndOfFrame();
        }
        _isShowingDescription = false;
        descriptionObj.SetActive(false);
    }
}
