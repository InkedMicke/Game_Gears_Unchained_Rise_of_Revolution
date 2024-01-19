using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class MainCRail : MonoBehaviour
{
    private SplineAnimate _splineAnim;
    private SplineContainer _splineContainer;
    private PlayerInputActions _inputActions;
    private MainCAnimatorController _mainCAnim;
    private MainCMovement _mainCMove;

    [SerializeField] private LayerMask railLayer;

    [SerializeField] private GameObject splineFollower;
    [SerializeField] private GameObject PP;

    [SerializeField] private Transform railCheck;

    [System.NonSerialized] public bool IsSliding;

    [SerializeField] private float railSpeed = 5f;
    [SerializeField] private float jumpCooldown = 1f;
    private float _splineLength;
    private float _distancePercentage;
    private float _timeGraceJumpPeriod;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.PlayerPC.Jump.performed += JumpOnRail;

        _mainCAnim = GetComponent<MainCAnimatorController>();
        _mainCMove = GetComponent<MainCMovement>();
    }


    private void Update()
    {
        if(IsOnRail() && !IsSliding)
        {
            StartSlide();
        }

        if(IsSliding)
        {
            if(!IsOnRail())
            {
                IsSliding = false;
            }
            _distancePercentage += railSpeed * Time.deltaTime / _splineLength;

            var currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
            transform.position = Vector3.MoveTowards(transform.position ,currentPosition, railSpeed * Time.deltaTime);

            if(_distancePercentage > 1f)
            {
                _distancePercentage = 0f;
            }

            var nextPosition = _splineContainer.EvaluatePosition(_distancePercentage + .005f);
            var direction = nextPosition - currentPosition;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
        }
    }

    private void StartSlide()
    {
        
        _splineContainer = splineFollower.GetComponent<SplineContainer>();
        _splineLength = _splineContainer.CalculateLength();
        IsSliding = true;
        _mainCAnim.SetMoveSpeed(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(railCheck.position,.2f);
    }

    private void JumpOnRail(InputAction.CallbackContext context)
    {
        if(CanJumpRail())
        {
            _timeGraceJumpPeriod = Time.time + jumpCooldown;
            _mainCMove.SetJumping(true);
            Debug.Log("hola5");
            StartCoroutine(JumpPhysics());
        }
    }

    private IEnumerator JumpPhysics()
    {
        var posicionFinal = transform.position + Vector3.up * 5f;
        while (transform.position.y < posicionFinal.y)
        {
            // Calcular el siguiente paso del salto
            float pasoSalto = 5f * Time.deltaTime;

            // Mover hacia arriba
            transform.Translate(Vector3.up * pasoSalto);

            // Esperar un frame antes de la siguiente iteración
            // para dar tiempo a que Unity actualice la posición
            // y evite un bucle infinito
            yield return null;
        }
    }

    public bool IsOnRail()
    {
        return Physics.CheckSphere(railCheck.position, .2f, railLayer);
    }

    private bool CanJumpRail()
    {
        if (GameManagerSingleton.Instance.IsAbilityMenuEnabled)
        {
            return false;
        }

        if (GameManagerSingleton.Instance.IsStopMenuEnabled)
        {
            return false;
        }

        if (GameManagerSingleton.Instance.IsSettingsMenuEnabled)
        {
            return false;
        }

        if (!IsOnRail())
            Debug.Log("hola1");
        {
            return false;
        }

        if (_mainCMove.IsJumping)
        {
            Debug.Log("hola2");
            return false;
        }

        if (Time.time < _timeGraceJumpPeriod)
        {
            Debug.Log("hola3");
            return false;
        }

        return true;
    }
}
