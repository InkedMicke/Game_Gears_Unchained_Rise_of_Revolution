using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class C_Elevator : MonoBehaviour
{

    [SerializeField] private Transform elevatorGoal;
    [SerializeField] private float velocityElevator = 0.1f;
    [SerializeField] private float delay = 0.1f;

    private bool isActive;

    [SerializeField] private UnityEvent OnStart;
    [SerializeField] private UnityEvent OnFinish;
    private void Start()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (!isActive)
        {
            OnStart.Invoke();
            StartCoroutine(ElevatorGoToPosition());
        }
       
        isActive = false;
       
    }

    IEnumerator ElevatorGoToPosition()
    {
        while(Mathf.Abs(Vector3.Distance(transform.position, elevatorGoal.position)) <= 0.01f)
         {
            isActive = true;
            var diference = elevatorGoal.position - transform.position; 
            transform.position += diference.normalized * velocityElevator;

            yield return new WaitForSeconds(delay);
         }
        OnFinish.Invoke();
    }
}
