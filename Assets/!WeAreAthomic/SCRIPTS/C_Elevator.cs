using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Elevator : MonoBehaviour
{

    [SerializeField] private Transform elevatorGoal;
    [SerializeField] private float velocityElevator = 0.1f;
    [SerializeField] private float delay = 0.1f;

    private bool isActive;


    private void OnTriggerEnter(Collider other)
    {
       
        if (!isActive)
        {
            StartCoroutine(ElevatorGoUp());
        }
       
        isActive = false;
       
    }

    IEnumerator ElevatorGoUp()
    {
        while(transform.position.y < elevatorGoal.position.y)
         {
            isActive = true;
            transform.position = new Vector3(transform.position.x, transform.position.y + velocityElevator, transform.position.z);

            yield return new WaitForSeconds(delay);
         }
    }
}
