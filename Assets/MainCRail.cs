using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.Splines;

public class MainCRail : MonoBehaviour
{
    private SplineAnimate _splineAnim;
    private SplineContainer _splineContainer;
    private PlayerInputActions _inputActions;
    private MainCAnimatorController _mainCAnim;
    private MainCMovement _mainCMove;
    private CharacterController _cc;

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
        _mainCAnim = GetComponent<MainCAnimatorController>();
        _mainCMove = GetComponent<MainCMovement>();
        _cc = GetComponent<CharacterController>();
    }


    private void Update()
    {
        if(IsOnRail() && !IsSliding)
        {
            StartSlide();
        }

        if(IsSliding)
        {
            var currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
            Debug.DrawRay(currentPosition, Vector3.up * 20, Color.yellow);
        }

        if(IsSliding && !_mainCMove.IsJumping)
        {
            _distancePercentage += railSpeed * Time.deltaTime / _splineLength;

            var currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
            //transform.position = Vector3.MoveTowards(transform.position ,currentPosition, railSpeed * Time.deltaTime);
            var posVector = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
            var difference = posVector - transform.position;
            _cc.Move(railSpeed * Time.deltaTime * difference.normalized);
            

            if(_distancePercentage > 1f)
            {
                _distancePercentage = 0f;
            }

            var nextPosition = _splineContainer.EvaluatePosition(_distancePercentage + .001f);
            var direction = nextPosition - currentPosition;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
        }
        else if(IsSliding && _mainCMove.IsJumping)
        {
            _distancePercentage += railSpeed * Time.deltaTime / _splineLength;
            var currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);
            var posVector = new Vector3(currentPosition.x, transform.position.y, currentPosition.z);
            //transform.position = Vector3.MoveTowards(transform.position, currentPosition, railSpeed * Time.deltaTime);
            var difference = posVector - transform.position;
            _cc.Move(railSpeed * Time.deltaTime * difference.normalized);
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

    public bool IsOnRail()
    {
        return Physics.CheckSphere(railCheck.position, .2f, railLayer);
    }


}
