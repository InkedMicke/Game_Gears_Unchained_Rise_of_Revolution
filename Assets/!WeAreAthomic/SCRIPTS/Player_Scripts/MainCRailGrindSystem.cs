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
        private GStopMenu _gStopMenu;
        private MainVFXCharacter _mainVFXCharacter;
        private MainCSounds _mainCSounds;

        private Scene _currentScene;

        private Collider[] _railCols;

        [SerializeField] private GameObject spaceTutCanvas;
        private GameObject _currentPipe;
        private GameObject _currentRail;

        [SerializeField] private Transform groundCheck;

        [SerializeField] private LayerMask railLayer;
        [SerializeField] private LayerMask obstacleLayer;

        private Vector3 _currentDestination;
        private Vector3 _posOnAirTarget;

        [SerializeField] private string labScene;
        [SerializeField] private string sewerScene;

        [System.NonSerialized] public bool IsSliding;
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
            _anim = GetComponent<Animator>();
            _mainCLayers = GetComponent<MainCLayers>();
            _mainCMove = GetComponent<MainCMovement>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCPistol = GetComponent<MainCPistol>();
            _gStopMenu = GetComponent<GStopMenu>();
            _mainVFXCharacter = GetComponent<MainVFXCharacter>();
            _mainCSounds = GetComponent<MainCSounds>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Jump.performed += Jump;
        }

        private void Update()
        {
            _currentScene = SceneManager.GetActiveScene();
            if (IsOnRail() && !_isJumping)
            {
                _railCols = Physics.OverlapSphere(groundCheck.position, .3f, railLayer);
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

        public void StartSliding()
        {

            if (!IsSliding)
            {
                _cc.enabled = false;
                IsSliding = true;
                CanJumpOnRail = true;
                GetRailGameObject();
                GetAllTransforms();
                _mainCAnimator.SetGrounded(true);
                _mainCAnimator.SetFalling(false);
                _mainCAnimator.SetJumping(false);
                _mainVFXCharacter.ToggleRail();
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

        private IEnumerator FixPosition(float value)
        {
            yield return new WaitForSeconds(value);
            var desiredPos = new Vector3(directionsList[_childActual].position.x, transform.position.y, transform.position.z);
            transform.position = desiredPos;
            Debug.Log("hola1");
        }

        private void GetAllTransforms()
        {
            if (_railCols.Length > 0)
            {
                if (_currentScene.name == sewerScene)
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
                }

                if (_currentScene.name == labScene)
                {
                    var padre1 = _railCols[0].transform.parent;
                    var padre2 = padre1.parent;

                    var railContainer = padre2.GetChild(padre2.childCount - 1);

                    var allChildren = railContainer.GetComponentsInChildren<Transform>();

                    foreach (var child in allChildren)
                    {
                        if (child.CompareTag("RailTransform"))
                        {
                            directionsList.Add(child);
                        }
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

                FixPosition(0);

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
                if (!_mainCPistol.IsAiming)
                {
                    RotateToNextDirectionList();
                }

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

        public void StartSlideCoroutine()
        {
            _mainCAnimator.SetSliding(true);
            _isJumping = false;
            FixPosition(.1f);
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
            return Physics.CheckSphere(groundCheck.position, .1f, railLayer);
        }

        private bool ThereIsObstacle()
        {
            var ray = new Ray(groundCheck.position, groundCheck.transform.forward);
            return Physics.Raycast(ray, 2f, obstacleLayer);
        }
    }
}