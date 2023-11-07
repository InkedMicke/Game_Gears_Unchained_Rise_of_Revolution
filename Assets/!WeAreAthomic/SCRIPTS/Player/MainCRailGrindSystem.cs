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

        private Collider[] _railCols;

        [SerializeField] private GameObject meshObj;
        private GameObject _currentPipe;
        private GameObject _currentRail;

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
        private bool _haveFirstTransfrom;

        public int _childActual = 0;

        [SerializeField] private float railSpeed = .1f;
        [SerializeField] private float railSpeedBoost = 30f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float gravityRail = -15f;
        [SerializeField] private float jumpForceRail = 10f;
        [SerializeField] private float jumpDelay = .5f;
        private float _playbackMultiplier;
        private float jumpTotalDelay;
        private float _playerOffset = 0.0f;

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

            Slide();
            //_cc.Move(_velocity * Time.deltaTime);
        }

        public void StartSliding()
        {
            if (!IsSliding)
            {
                _cc.enabled = false;
                IsSliding = true;
                CanJumpOnRail = true;
                GetRailGameObject();
                GetAllTransforms();
                _mainCLayers.DisableJumpLayer();
                _mainCAnimator.SetGrounded(true);
                _mainCAnimator.SetFalling(false);
                _mainCAnimator.SetJumping(false);
            }
        }

        private void GetRailGameObject()
        {
            var ray = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(ray, out var hit, 1f, railLayer))
            {
                _currentRail = hit.collider.gameObject;
            }
        }

        private void FixPosition()
        {
/*            _currentDestination = directionsList[_childActual].position;
            var desiredPos = new Vector3();

            if (_currentDestination.x - directionsList[_childActual - 1].transform.position.x < 1f)
            {
                desiredPos = new Vector3(_currentDestination.x, transform.position.y, transform.position.z);


            }

            if (_currentDestination.z - directionsList[_childActual - 1].transform.position.z < 1f)
            {
                desiredPos = new Vector3(transform.position.x, transform.position.y, _currentDestination.z);
            }

            transform.position = desiredPos;*/

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

                    if (dotProduct > 0 && !_haveFirstTransfrom)
                    {
                        _haveFirstTransfrom = true;
                        break;
                    }
                    else
                    {
                        _childActual++;

                    }
                }

                FixPosition();

                _canSlide = true;

                _mainCAnimator.SetSliding(true);
                _mainCLayers.EnableSlideLayer();

            }
        }

        private void Slide()
        {
            if (_canSlide)
            {
                _currentDestination = directionsList[_childActual].position;
                _directionMove = (_currentDestination - transform.position).normalized;
                _posOnAirTarget = new Vector3(directionsList[_childActual].position.x, transform.position.y,
                    directionsList[_childActual].position.z);
                RotateToNextDirectionList();
                MoveToNextDirectionList();
                if (Vector3.Distance(transform.position, _posOnAirTarget) < 0.3f)
                {
                    if (directionsList.Count > 0)
                    {
                        _childActual++;
                    }
                }
            }
        }

        private void StartSlideCoroutine()
        {
            _mainCAnimator.SetSliding(true);
            _isJumping = false;
            Invoke(nameof(FixPosition), 0.1f);
            _canSlide = true;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (ThereIsObstacle() && !_isJumping)
            {
                var ray = new Ray(groundCheck.position, groundCheck.transform.forward);
                if (Physics.Raycast(ray, out var hit, 4f, obstacleLayer))
                {
                    _currentPipe = hit.collider.gameObject;
                    if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) > 1.5f)
                    {
                        _mainCAnimator.SetSliding(false);
                        _canSlide = false;
                        FixPositionFrontPipe();
                        _isJumping = true;
                        _anim.applyRootMotion = true;
                        _anim.SetTrigger(string.Format("jumpRail"));
                    }
                }
            }
        }

        private void MoveToNextDirectionList()
        {
            var position = transform.position;
            _posOnAirTarget = new Vector3(directionsList[_childActual].position.x, position.y, directionsList[_childActual].position.z);
            position = Vector3.MoveTowards(position, _currentDestination, railSpeed * Time.deltaTime);
            transform.position = position;
        }

        private void RotateToNextDirectionList()
        {
            var targetDirection = _currentDestination - transform.position;
            var targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }

        private void FixPositionFrontPipe()
        {
            var childPipe = _currentPipe.transform.GetChild(0);
            var childPipePos = childPipe.transform.position;
            var desiredPos = new Vector3(childPipePos.x, transform.position.y, childPipePos.z);

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
            return Physics.Raycast(ray, 3f, obstacleLayer);
        }
    }
}