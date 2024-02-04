using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCRailGrindSystem : MonoBehaviour
    {

        private CharacterController _cc;
        private Animator _anim;
        private PlayerInputActions _playerInputActions;
        private MainCLayers _mainCLayers;
        private MainCMovement _mainCMove;
        private MainCAnimatorController _mainCAnimator;
        private MainCPistol _mainCPistol;
        private MainCStopMenu _gStopMenu;
        private MainCVFX _mainVFXCharacter;
        private MainCSounds _mainCSounds;

        private Scene _currentScene;

        private Collider[] _railCols;

        [SerializeField] private GameObject spaceTutCanvas;
        [SerializeField] private GameObject railSparks;
        [SerializeField] private GameObject splineFollower;
        private GameObject _currentPipe;
        private GameObject _currentRail;

        [SerializeField] private Transform railCheck;
        [SerializeField] private Transform orientation;

        [SerializeField] private LayerMask railLayer;
        [SerializeField] private LayerMask obstacleLayer;

        private Vector3 _currentDestination;
        private Vector3 _posOnAirTarget;

        [SerializeField] private string labScene;
        [SerializeField] private string sewerScene;

        private  bool IsSliding;
        [System.NonSerialized] public bool CanJumpOnRail;
        private bool _canSlide;
        private bool _canBoost;
        private bool _isJumping;
        private bool _isFalling;
        private bool _haveFirstTransfrom;
        private bool _isFirstJump = true;
        private bool _isCanvasJump;

        public int _childActual = 0;

        [SerializeField] private float railSpeed = .1f;
        [SerializeField] private float rotationSpeed = 5f;

        public List<Transform> directionsList = new List<Transform>();



        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _anim = GetComponentInChildren<Animator>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCMove = GetComponent<MainCMovement>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCPistol = GetComponent<MainCPistol>();
            _gStopMenu = GetComponent<MainCStopMenu>();
            _mainVFXCharacter = GetComponent<MainCVFX>();
            _mainCSounds = GetComponent<MainCSounds>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.PlayerPC.Enable();
            _playerInputActions.PlayerPC.Jump.performed += InputPC;
            _playerInputActions.PlayerGamepad.Jump.performed += InputGamepad;
        }

        private void Update()
        {
            _currentScene = SceneManager.GetActiveScene();
            if (IsOnRail() && !_isJumping)
            {
                _railCols = Physics.OverlapSphere(railCheck.position, .3f, railLayer);
                StartSliding();
                if (!_isCanvasJump && ThereIsObstacle())
                {
                    spaceTutCanvas.SetActive(true);
                    GameManagerSingleton.Instance.FreezeTime(true);
                    _isCanvasJump = true;
                }
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

            if (_mainCMove.IsGrounded())
            {
                if (CanJumpOnRail)
                {
                    CanJumpOnRail = false;
                }
            }

            Slide();
        }

        private void InputPC(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.pc)
            {
                Jump();
            }
        }

        private void InputGamepad(InputAction.CallbackContext context)
        {
            if (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad)
            {
                Jump();
            }
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
                Debug.Log("hola1");
                _mainCAnimator.SetGrounded(true);
                _mainCAnimator.SetFalling(false);
                _mainCAnimator.SetJumping(false);
                
            }
        }

        public void SetCanSlide(bool canSlide)
        {
            _canSlide = canSlide;
        }

        private void GetRailGameObject()
        {
            var ray = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(ray, out var hit, 1f, railLayer))
            {
                _currentRail = hit.collider.gameObject;
            }
        }

        private IEnumerator FixPosition(float value)
        {
            yield return new WaitForSeconds(value);
            var desiredPos = new Vector3(orientation.position.x, transform.position.y, directionsList[_childActual].position.z);
            transform.position = desiredPos;
        }

        private void GetAllTransforms()
        {
            if (_railCols.Length > 0)
            {
                var padre1 = _railCols[0].transform;
                var padre2 = padre1.parent;
                var padre3 = padre2.parent;
                var padre4 = padre3.parent;

                var railContainer = padre4.GetChild(padre4.childCount - 1);

                var allChildren = railContainer.GetComponentsInChildren<Transform>();
                Debug.Log(railContainer);

                foreach (var child in allChildren)
                {
                    if (child.CompareTag("RailTransform"))
                    {
                        directionsList.Add(child);
                    }
                }

                foreach (var t in directionsList.ToList())
                {
                    var playerToObjDirection = t.position - transform.position;
                    var dotProduct = Vector3.Dot(playerToObjDirection.normalized, transform.forward.normalized);

                    if (dotProduct > 0)
                    {
                        break;
                    }
                    else
                    {
                        _childActual++;
                    }
                }

                StartCoroutine(FixPosition(0));
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
                _posOnAirTarget = new Vector3(directionsList[_childActual].position.x, transform.position.y,
                    directionsList[_childActual].position.z);
                    RotateToNextDirectionList();


                MoveToNextDirectionList();
                if (Vector3.Distance(transform.position, _posOnAirTarget) < 0.5f)
                {
                    if (directionsList.Count > 0)
                    {
                        _childActual++;
                    }
                }
            }
        }

        public void StartSlideCoroutine()
        {
            _mainCAnimator.SetSliding(true);
            _isJumping = false;
            railSparks.SetActive(true);
            //StartCoroutine(FixPosition(0f));
            _canSlide = true;
        }

        private void Jump()
        {
            if (ThereIsObstacle() && !_isJumping && IsSliding)
            {
                var ray = new Ray(railCheck.position, railCheck.transform.forward);
                if (Physics.Raycast(ray, out var hit, 4f, obstacleLayer))
                {
                    _currentPipe = hit.collider.gameObject;
                    if (_isFirstJump)
                    {
                        GameManagerSingleton.Instance.FreezeTime(false);
                        spaceTutCanvas.SetActive(false);
                        _isFirstJump = false;
                    }

                    if (Vector3.Distance(hit.collider.gameObject.transform.position, transform.position) > 1.5f)
                    {
                        _mainCAnimator.SetSliding(false);
                        _canSlide = false;
                        FixPositionFrontPipe();
                        _isJumping = true;
                        railSparks.SetActive(false);
                        _anim.applyRootMotion = true;
                        _anim.SetTrigger(string.Format("jumpRail"));
                        _mainCSounds.PlayJumpSound();

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
            return Physics.CheckSphere(railCheck.position, .1f, railLayer);
        }

        private bool ThereIsObstacle()
        {
            var ray = new Ray(railCheck.position, railCheck.transform.forward);
            return Physics.Raycast(ray, 2f, obstacleLayer);
        }
    }
}