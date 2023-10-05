using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Button_Interactable : MonoBehaviour, IInteractable
{

    public GameObject eButtonObj;
    public GameObject circleObj;
    private GameObject _cameraObj;

    Transform playerTr;

    private bool _isSeeing;

    public UnityEvent[] eventsToActivate;

    private void Awake()
    {

    }

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    private void Update()
    {
        ShowCircle();
    }

    private void ShowCircle()
    {
        if (Vector3.Distance(transform.position, playerTr.position) < 10)
        {
            circleObj.SetActive(true);
        }
        else
        {
            circleObj.SetActive(false);
        }
    }

    public void Interact()
    {
        foreach (UnityEvent i in eventsToActivate)
        {
            i.Invoke();
        }
    }

    public void ShowButton()
    {
        eButtonObj.SetActive(true);
    }

    public void HideButton()
    {
        eButtonObj.SetActive(false);
    }

}