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
   

    public void GoPosition()
    {
        OnStart.Invoke();
        StartCoroutine(ElevatorGoToPosition());
    }

    IEnumerator ElevatorGoToPosition()
    {
        while(Mathf.Abs(Vector3.Distance(transform.position, elevatorGoal.position)) > 0.1f)
         {
            isActive = true;
            var diference = elevatorGoal.position - transform.position; 
            transform.position += diference.normalized * velocityElevator;

            yield return new WaitForSeconds(delay);
         }
        transform.position = elevatorGoal.position;
        OnFinish.Invoke();
    }
}
