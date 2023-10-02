using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollower : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInputActions _playerInputActions;

    public Transform cameraFollow;

    [SerializeField] private float controllerSensitivityX = 100;
    [SerializeField] private float controllerSensitivityY = 100;
    [SerializeField] private float mouseSensitivityX = 100;
    [SerializeField] private float mouseSensitivityY = 100;
    [SerializeField] private float clampAngle = 80.0f;
    private float _rotY;
    private float _rotX;
    private float _finalInputX;
    private float _finalInputY;

    private void Awake()
    {

    }

    private void Start()
    {
        cameraFollow = GameObject.FindGameObjectWithTag("Camera_Follow").transform;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Camera.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        var controllerVector = _playerInputActions.Camera.Look.ReadValue<Vector2>();
        // We setup the rotation of the sticks here

        var mouseVector = Mouse.current.delta.ReadValue() * Time.smoothDeltaTime;

        _finalInputX = (controllerVector.x * controllerSensitivityX) + (mouseVector.x * mouseSensitivityX);
        _finalInputY = (-(controllerVector.y) * controllerSensitivityY) + -(mouseVector.y * mouseSensitivityY);

       _rotY += _finalInputX * Time.deltaTime;
        _rotX += _finalInputY * Time.deltaTime;

        _rotX = Mathf.Clamp(_rotX, -clampAngle, clampAngle);

        var localRotation = Quaternion.Euler(_rotX, _rotY, 0.0f);
        transform.rotation = localRotation;

    }

    private void LateUpdate()
    {
        var target = cameraFollow.transform;

        //move towards the game object that is the target
        var step = 20 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

    }
}
