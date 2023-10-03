using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Button_Interactable : MonoBehaviour
{
    private Interactor _interactor;
    
    public GameObject eButtonObj;
    public GameObject circleObj;
    private GameObject _cameraObj;
    
    Transform playerTr;

    private bool isPlayerNear;
    private bool isSeeing;

    public UnityEvent[] eventsToActivate;
    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        _interactor = _cameraObj.GetComponent<Interactor>();
    }

    // Update is called once per frame
    private void Update()
    {
        ShowButton();
    }

    private void ShowButton()
    {

        if (Vector3.Distance(transform.position, playerTr.position) < 10 && !_interactor.isSeeing)
        {
            circleObj.SetActive(true);
        }
        else
        {
            circleObj.SetActive(false);
        }

        if (_interactor.isSeeing)
        {
            if (Vector3.Distance(transform.position, playerTr.position) < 3)
            {
                eButtonObj.SetActive(true);
            }
            else
            {
                eButtonObj.SetActive(false);
            }
        }
        else
        {
            eButtonObj.SetActive(false);
        }


        

    }

    private void OnTriggerStay(Collider other)
    {
        isPlayerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerNear = false;
    }
}
