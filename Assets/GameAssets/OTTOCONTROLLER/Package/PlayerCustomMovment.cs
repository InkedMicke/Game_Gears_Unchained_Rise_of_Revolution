using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundDetection))]
[RequireComponent(typeof(CharacterControllerV2))]
public class PlayerCustomMovment : MonoBehaviour
{
    [SerializeField, HideInInspector] GroundDetection _groundDetect;

    //Variables 
    [SerializeField] float _acceleration = 12f;
    [SerializeField] float _targetVelocity = 10f;

    [SerializeField, Range(0f, 1f)] float _turnaroundStrength;

    private void OnValidate()
    {
        _groundDetect = GetComponent<GroundDetection>();
    }

    /// <summary>
    /// Logic for the horizontal movment
    /// </summary>
    /// <param name="currentVelocity">Velocity from the player before beeing modified</param>
    /// <param name="input">Target direction for the player</param>
    /// <returns>new Velocity for the player</returns>
    public Vector2 Movment(Vector2 currentVelocity, Vector2 input)
    {


        //slide off
        if (_groundDetect.Sliding)
        {
            //TODO sliding off logic
        }

        return currentVelocity;
    }
}
