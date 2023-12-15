using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerElevator : MonoBehaviour
{
    [SerializeField] private UnityEvent seActivaCuandoEstaElTiempoNecesarioDentro;

    public bool IsPlayerInside;
    public bool IsActivated;

    private float maxTimer = 1f;
    private float timer;

    private void OnTriggerEnter(Collider other)
    {
        IsPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        timer = maxTimer;
        IsPlayerInside = false;
    }

    private void Update()
    {
        if(IsPlayerInside)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                seActivaCuandoEstaElTiempoNecesarioDentro.Invoke();
                timer = maxTimer;
                IsPlayerInside = false;
                gameObject.SetActive(false);
                IsActivated = true;
            }
        }
    }

    public void SetIsActivated(bool condition)
    {
        IsActivated = condition;
    }
}
