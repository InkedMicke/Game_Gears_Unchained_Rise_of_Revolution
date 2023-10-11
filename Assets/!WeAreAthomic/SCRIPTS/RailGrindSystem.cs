using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static Autodesk.Fbx.FbxAxisSystem;

public class RailGrindSystem : MonoBehaviour
{
    private CharacterController _cc;
    private Animator _anim;
    private PlayerInputActions _playerInputActions;
    private MainCLayers _mainCLayers;
    private MainCMovement _mainCMove;

    [SerializeField] private Transform groundCheck;

    [SerializeField] private LayerMask railLayer;

    private Vector3 _currentDestination;
    private Vector3 _directionMove;
    private Vector3 _velocity;

    [System.NonSerialized] public bool IsSliding;
    [System.NonSerialized] public bool CanJumpOnRail;
    private bool _canSlide;
    private bool _canBoost;
    private bool _isJumping;
    private bool _isFalling;

    private int childActual = 0;

    [SerializeField] private float railSpeed = .1f;
    [SerializeField] private float railSpeedBoost = 30f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float gravityRail = -15f;
    [SerializeField] private float jumpForceRail = 10f;
    [SerializeField] private float jumpDelay = .5f;
    private float _playbackMultiplier;
    private float jumpTotalDelay;

    public List<Transform> directionsList = new List<Transform>();

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _mainCLayers = GetComponent<MainCLayers>();
        _mainCMove = GetComponent<MainCMovement>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Running.started += IncreaseBoost;
        _playerInputActions.Player.Running.canceled += DrecreaseBoost;
        _playerInputActions.Player.Jump.performed += JumpOnRail;

    }

    private void Update()
    {
        if (IsOnRail())
        {
            StartSliding();
        }
        else
        {
            if (IsSliding && !CanJumpOnRail)
            {
                _anim.SetLayerWeight(_anim.GetLayerIndex("StartSliding"), 0f);
                IsSliding = false;
                _canSlide = false;
                directionsList.Clear();
                childActual = 0;
            }
        }
        BoostManager();
        Slide();
        _anim.SetLayerWeight(_anim.GetLayerIndex("EndSliding"), _playbackMultiplier);

        if(_mainCMove.IsGrounded())
        {
            if(CanJumpOnRail)
            {
                CanJumpOnRail = false;
            }
        }

        if(IsOnRail())
        {
            if(_isJumping && _velocity.y < 0)
            {
                _isJumping = false;
            }
        }

        Debug.Log(_velocity.y);

        ApplyGravityRail();

        _cc.Move(_velocity * Time.deltaTime);
    }

    public void StartSliding()
    {
        if (!IsSliding)
        {
            IsSliding = true;
            CanJumpOnRail = true;
            GetAllTransforms();
            _anim.SetLayerWeight(_anim.GetLayerIndex("StartSliding"), 1);
        }
    }

    void GetAllTransforms()
    {
        var ray = new Ray(groundCheck.position, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f, railLayer))
        {
            var padre1 = hit.collider.gameObject.transform.parent;
            var padre2 = padre1.parent;

            var railContainer = padre2.GetChild(padre2.childCount - 1);

            Transform[] allChildren = railContainer.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("RailTransform"))
                {
                    directionsList.Add(child);
                }
            }


            SortList();

            foreach (Transform t in directionsList.ToList())
            {
                var playerToObjDirection = t.position - transform.position;
                var dotProduct = Vector3.Dot(playerToObjDirection.normalized, transform.forward.normalized);

                if (dotProduct < 0)
                {
                    directionsList.Remove(t);
                }
            }

            _canSlide = true;

            _anim.SetBool(string.Format("isSliding"), true);
            _mainCLayers.EnableSlideLayer();


/*            _cc.enabled = false;

            transform.position = new Vector3(transform.position.x, transform.position.y, directionsList[childActual].position.z);

            _cc.enabled = true;*/
        }
    }

    void Slide()
    {
        if (directionsList.Count > 0 && _canSlide)
        {
            _currentDestination = directionsList[childActual].position;

            _directionMove = (_currentDestination - transform.position).normalized;
            _cc.Move(_directionMove * (railSpeed * Time.deltaTime));

            // Almacenar la rotación deseada en una variable temporal
            var targetRotation = directionsList[childActual].rotation;

            // Aplicar una corrección de 90 grados alrededor del eje Y
            targetRotation *= Quaternion.Euler(0, -90, 0);

            // Interpolar la rotación corregida
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);



            if (Vector3.Distance(transform.position, directionsList[childActual].position) < 0.3f)
            {
                childActual++;
                _currentDestination = directionsList[childActual].position;
            }
        }
    }

    void IncreaseBoost(InputAction.CallbackContext context)
    {
        _canBoost = true;

    }

    void DrecreaseBoost(InputAction.CallbackContext context)
    {
        _canBoost = false;
        _playbackMultiplier = 0;
    }

    void BoostManager()
    {
        if (_playbackMultiplier < 1 && IsSliding && _canBoost)
        {
            _playbackMultiplier += Time.deltaTime * 0.5f;
        }

        if (railSpeed < railSpeedBoost && _canBoost && IsSliding)
        {
            railSpeed += Time.deltaTime * 4;
        }
    }

    public void JumpOnRail(InputAction.CallbackContext context)
    {
        if (IsSliding && !_isJumping && Time.time > jumpTotalDelay)
        {
            _velocity = transform.up * jumpForceRail;
            _isJumping = true;
        }
    }

    private void ApplyGravityRail()
    {
        if (_isJumping || !IsOnRail() && CanJumpOnRail)
        {
            _velocity += transform.up * Time.deltaTime * gravityRail;
        }
    }

    private void SortList()
    {
        directionsList.Sort((a, b) =>
        {
            var distanceToA = Vector3.Distance(a.position, transform.position);
            var distanceToB = Vector3.Distance(b.position, transform.position);
            return distanceToA.CompareTo(distanceToB);
        });
    }

    public bool IsOnRail()
    {
        return Physics.CheckSphere(groundCheck.position, .2f, railLayer);
    }
}