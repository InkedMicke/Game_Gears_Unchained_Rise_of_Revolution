using _WeAreAthomic.SCRIPTS.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MainCHackingSystem : MonoBehaviour
{
    private Animator _anim;
    private MainCLayers _mainCLayers;
    private CharacterController _cc;

    [SerializeField] private Slider hackSlider;

    [SerializeField] private GameObject wrenchObj;
    [SerializeField] private GameObject pistolObj;
    [SerializeField] private GameObject robotObj;
    [SerializeField] private GameObject hackCanvas;

    private GameObject _currentInteract;
    private GameObject _currentWeapon;

    public bool IsHackingAnim;
    public bool IsHacking;

    private float _actualTime;
    private float timeToHack;
    private float actualTime;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _mainCLayers = GetComponent<MainCLayers>();
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdateUI();
    }

    public void StartHacking(float value)
    {
        FixPosition();
        EnableHackAnim();
        SaveWeapon();
        DisableWeapon();
        timeToHack = value;
    }

    public void EndHacking()
    {
        hackCanvas.SetActive(false);
    }   

    private void UpdateUI()
    {
        if(IsHacking)
        {
            hackSlider.value = Time.time;
        }
    }

    public void EnableHackAnim()
    {
        IsHackingAnim = true;
        _anim.SetTrigger(string.Format("hack"));
        _mainCLayers.EnableHackLayer();
    }

    public void SpawnRobot()
    {
        var robot = Instantiate(robotObj, _currentInteract.transform.GetChild(0).position, Quaternion.identity);
    }

    public void FixPosition()
    {
        _cc.enabled = false;

        var interactables = FindObjectsOfType<Button_Interactable>();

        foreach(Button_Interactable t in interactables)
        {
            if(t.IsActive == true)
            {
                _currentInteract = t.gameObject;
            }
        }

        transform.position = new Vector3(_currentInteract.transform.GetChild(0).position.x, transform.position.y, _currentInteract.transform.GetChild(0).position.z + .8f);

        var desiredRot = new Vector3(_currentInteract.transform.GetChild(0).position.x, transform.position.y, _currentInteract.transform.GetChild(0).position.z);
        transform.LookAt(desiredRot);
    }

    private IEnumerator Hack(float value)
    {
        var canEnableLayer = true;

        actualTime = Time.time + value;

        while (canEnableLayer)
        {
            if (Time.time > actualTime)
            {
                canEnableLayer = false;
                EndHacking();
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void SaveWeapon()
    {
        if (pistolObj.activeSelf)
        {
            _currentWeapon = pistolObj;
        }

        if (wrenchObj.activeSelf)
        {
            _currentWeapon = wrenchObj;
        }
    }

    private void DisableWeapon()
    {
        _currentWeapon.SetActive(false);
    }
    private void EnableWeapon()
    {
        _currentWeapon.SetActive(true);
    }

    public void EndAnimHack()
    {
        _mainCLayers.DisableHackLayer();
        EnableWeapon();
        _cc.enabled = true;
        IsHackingAnim = false;
        StartCoroutine(Hack(timeToHack));
        hackCanvas.SetActive(true);
        IsHacking = true;
        hackSlider.minValue = Time.time;
        hackSlider.maxValue = Time.time + timeToHack;
    }
}
