using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _WeAreAthomic.SCRIPTS.Player;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCRailGrindSystem : MonoBehaviour
    {
        private CharacterController _cc;
        private Animator _anim;
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private MainCMovement _mainCMove;
        private MainCAnimatorController _mainCAnimator;

        private Coroutine _slideCorutine;

        private Collider[] _railCols;

        [SerializeField] private GameObject meshObj;
        private GameObject _currentPipe;

        [SerializeField] private Transform groundCheck;

        [SerializeField] private LayerMask railLayer;
        [SerializeField] private LayerMask obstacleLayer;

        private Vector3 _currentDestination;
        private Vector3 _directionMove;
        private Vector3 _velocity;
        private Vector3 _posOnAirPlayer;
        private Vector3 _posOnAirTarget;

        [System.NonSerialized] public bool IsSliding;
        [System.NonSerialized] public bool CanJumpOnRail;
        private bool _canSlide;
        private bool _canBoost;
        private bool _isJumping;
        private bool _isFalling;

        private int _childActual = 0;

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
            _mainCAnimator = GetComponent<MainCAnimatorController>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
            //_playerInputActions.Player.Running.started += IncreaseBoost;
            //_playerInputActions.Player.Running.canceled += DrecreaseBoost;
            _playerInputActions.Player.Jump.performed += Jump;
        }

        private void Update()
        {
            if (IsOnRail())
            {
                _railCols = Physics.OverlapSphere(groundCheck.position, .3f, railLayer);
                StartSliding();
            }
            else
            {
                if (IsSliding && !CanJumpOnRail)
                {
                    IsSliding = false;
                    _canSlide = false;
                    directionsList.Clear();
                    _childActual = 0;
                }
            }

            BoostManager();

            if (_mainCMove.IsGrounded())
            {
                if (CanJumpOnRail)
                {
                    CanJumpOnRail = false;
                }
            }

            if (IsOnRail())
            {
                if (_isJumping && _velocity.y < 0)
                {
                    _isJumping = false;
                    //_anim.SetBool(string.Format("isFalling"), true);
                    //_anim.SetBool(string.Format("isJumping"), false);
                    _isFalling = true;
                }

                if (_isFalling)
                {
                    _isFalling = false;
                    //_anim.SetBool(string.Format("isGrounded"), true);
                    //_anim.SetBool(string.Format("isFalling"), false);
                    _mainCLayers.DisableJumpLayer();
                }
            }

            //_cc.Move(_velocity * Time.deltaTime);
        }

        public void StartSliding()
        {
            if (!IsSliding)
            {
                IsSliding = true;
                CanJumpOnRail = true;
                GetAllTransforms();
                _mainCLayers.DisableJumpLayer();
                _mainCAnimator.SetGrounded(true);
                _mainCAnimator.SetFalling(false);
                _mainCAnimator.SetJumping(false);
                _slideCorutine = StartCoroutine(nameof(SlideCoroutine));
            }
        }

        private void FixPosition()
        {
            _cc.enabled = false;
            var desiredPos = new Vector3(transform.position.x, transform.position.y,
                directionsList[_childActual].position.z);

            transform.position = desiredPos;

            _cc.enabled = true;
        }

        private void GetAllTransforms()
        {
            var ray = new Ray(groundCheck.position, -Vector3.up);
            if (_railCols.Length > 0)
            {
                var padre1 = _railCols[0].transform;
                var padre2 = padre1.parent;
                var padre3 = padre2.parent;
                var padre4 = padre3.parent;

                var railContainer = padre4.GetChild(padre4.childCount - 1);

                var allChildren = railContainer.GetComponentsInChildren<Transform>();

                foreach (var child in allChildren)
                {
                    if (child.CompareTag("RailTransform"))
                    {
                        directionsList.Add(child);
                    }
                }


                //SortList();

                foreach (var t in directionsList.ToList())
                {
                    var playerToObjDirection = t.position - transform.position;
                    var dotProduct = Vector3.Dot(playerToObjDirection.normalized, transform.forward.normalized);

                    if (dotProduct < 0)
                    {
                        directionsList.Remove(t);
                    }
                }

                FixPosition();

                _canSlide = true;

                _mainCAnimator.SetSliding(true);
                _mainCLayers.EnableSlideLayer();

            }
        }

        private IEnumerator SlideCoroutine()
        {
            _cc.enabled = false;
            _currentDestination = directionsList[_childActual].position;
            _directionMove = (_currentDestination - transform.position).normalized;
            _posOnAirTarget = new Vector3(directionsList[_childActual].position.x, transform.position.y, directionsList[_childActual].position.z);

            while (Vector3.Distance(transform.position, _posOnAirTarget) > 0.3f && !_isJumping)
            {
                RotateToNextDirectionList();
                MoveToNextDirectionList();
                Debug.Log("hola1");
                yield return new WaitForSeconds(0.01f);
            }
            NextDirectionList();

        }

        private void Jump(InputAction.CallbackContext context)
        {
            if(ThereIsObstacle() && !_isJumping)
            {
                var ray = new Ray(groundCheck.position, groundCheck.transform.forward);
                if(Physics.Raycast(ray, out RaycastHit hit, 4f, obstacleLayer))
                {
                    _currentPipe = hit.collider.gameObject;
                    if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) > 1f)
                    {
                        StopCoroutine(_slideCorutine);
                        FixPositionFrontPipe();
/*                        _isJumping = true;
                        _anim.applyRootMotion = true;
                        _anim.SetTrigger(string.Format("jumpRail"));
                        Debug.Log("funciona");*/
                    }
                }
            }
        }

        private void NextDirectionList()
        {
            if(directionsList.Count > 0)
            {
                _childActual++;
                StartCoroutine(nameof(SlideCoroutine));
            }
        }

        private void MoveToNextDirectionList()
        {
            _posOnAirTarget = new Vector3(directionsList[_childActual].position.x, transform.position.y, directionsList[_childActual].position.z);
            transform.position = Vector3.MoveTowards(transform.position, _currentDestination, railSpeed * Time.deltaTime);
        }

        private void RotateToNextDirectionList()
        {
            Vector3 targetDirection = _currentDestination - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }

        private void FixPositionFrontPipe()
        {
            var ray = new Ray(_currentPipe.transform.position, _currentPipe.transform.forward);
            var rayPos = ray.GetPoint(1f);
            var desiredPos = new Vector3(transform.position.x, transform.position.y, rayPos.z);
            transform.position = desiredPos;
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

        /*private void ApplyGravityRail()
    {
        if (_isJumping || !IsOnRail() && CanJumpOnRail)
        {
            _velocity += transform.up * Time.deltaTime * gravityRail;
        }
    }*/

        private void SortList()
        {
            directionsList.Sort((a, b) =>
            {
                var distanceToA = Vector3.Distance(a.position, transform.position);
                var distanceToB = Vector3.Distance(b.position, transform.position);
                return distanceToA.CompareTo(distanceToB);
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(groundCheck.position, .3f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, .1f);
        }

        public bool IsOnRail()
        {
            return Physics.CheckSphere(groundCheck.position, .1f, railLayer);
        }

        private bool ThereIsObstacle()
        {
            var ray = new Ray(groundCheck.position, groundCheck.transform.forward);
            return Physics.Raycast(ray, 4f, obstacleLayer);
        }
    }
}