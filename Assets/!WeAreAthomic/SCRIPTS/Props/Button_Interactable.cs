using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Button_Interactable : MonoBehaviour, IInteractable
{
    public GameObject eButtonObj;
    public GameObject circleObj;
    Transform playerTr;
    Transform cameraTr;

    private bool isPlayerNear;
    private bool isSeeing;

    public UnityEvent[] eventsToActivate;
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        cameraTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        ShowButton();
    }

    public void IsSeeing()
    {
        if (!isSeeing)
        {
            isSeeing = true;
        }
    }

    public void IsNotSeeing()
    {
        if (isSeeing)
        {
            isSeeing = false;
        }
    }

    public void ShowButton()
    {

        if (Vector3.Distance(transform.position, playerTr.position) < 10 && Vector3.Distance(transform.position, playerTr.position) > 5)
        {
            circleObj.SetActive(true);
        }
        else
        {
            circleObj.SetActive(false);
        }

        if (!isPlayerNear) return;


        eButtonObj.SetActive(isSeeing);

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
