using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button_Interactable : MonoBehaviour, IInteractable
{
    public GameObject eButtonObj;
    public GameObject circleObj;
    Transform playerTr;
    Transform cameraTr;

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

    public void Interact()
    {
        for (int i = 0; i < eventsToActivate.Length; i++)
        {
            eventsToActivate[i].Invoke();
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

        if(Vector3.Distance(transform.position, playerTr.position) < 5)
        {
            eButtonObj.SetActive(true);
        }
        else
        {
            eButtonObj.SetActive(false);
        }
    }
}
