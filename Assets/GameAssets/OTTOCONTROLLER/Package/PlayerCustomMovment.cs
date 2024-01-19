using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundDetection))]
[RequireComponent(typeof(CharacterControllerV2))]
public class PlayerCustomMovment : MonoBehaviour
{
    [SerializeField, HideInInspector] Rigidbody _rb;
    [SerializeField, HideInInspector] GroundDetection _groundDetect;

    //Variables 
    private Vector2 velocityRef = Vector2.zero;

    [SerializeField] float _acceleration = 12f;
    [SerializeField] float _targetVelocity = 10f;
    [SerializeField] float smoothTime = 10f;
    [SerializeField] float rotationSpeed = 10f;

    [SerializeField, Range(0f, 1f)] float _turnaroundStrength;

    private void OnValidate()
    {
        _rb = GetComponent<Rigidbody>();
        _groundDetect = GetComponent<GroundDetection>();
    }

    private void FixedUpdate()
    {
    }

    /// <summary>
    /// Logic for the horizontal movment
    /// </summary>
    /// <param name="currentVelocity">Velocity from the player before beeing modified</param>
    /// <param name="input">Target direction for the player</param>
    /// <returns>new Velocity for the player</returns>
    public Vector2 Movment(Vector2 currentVelocity, Vector2 input)
    {

        Vector2 targetVelocity = input * _targetVelocity;
        currentVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref velocityRef, smoothTime);
        _rb.velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.y);

        if(input.magnitude > 0.1f)
        {
            RotateToCameraDirection();
        }

        //slide off
        if (_groundDetect.Sliding)
        {
            //TODO sliding off logic
        }

        return currentVelocity;
    }

    private void RotateToCameraDirection()
    {
        // Obtener la dirección de la cámara
        Vector3 direccionCamara = Camera.main.transform.forward;
        direccionCamara.y = 0; // No queremos rotación en el eje Y

        // Calcular la rotación basada en la dirección de la cámara
        Quaternion rotacionDeseada = Quaternion.LookRotation(direccionCamara);

        // Aplicar la rotación al personaje suavemente
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, rotationSpeed * Time.deltaTime);
    }
}
