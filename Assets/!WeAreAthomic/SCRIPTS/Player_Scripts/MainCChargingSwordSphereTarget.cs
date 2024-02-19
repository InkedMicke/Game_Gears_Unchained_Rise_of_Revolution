using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Dummie;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCChargingSwordSphereTarget : MainCMouseController
    {
        private CharacterController _cc;
        private Animator _anim;
        private CapsuleCollider _cC;
        private MainCAttack _mainCAttack;
        private MainCLayers _mainCLayers;
        private MainCSwitchWeapon _mainCSwitch;
        private PlayerInputActions _playerInputActions;
        private MainCAnimatorController _mainCAnimator;
        private MainCMovement _mainCMove;

        [SerializeField] private GameObject sphereDetectorObj;
        [SerializeField] private GameObject groundTr;
        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject quickCanvas;
        [SerializeField] private GameObject floatingTextPrefab;
        [SerializeField] private GameObject orientation;
        [SerializeField] private GameObject wrenchObj;
        private GameObject _mainCameraObj;

        private Camera _mainCamera;

        [SerializeField] private Image circleImage;
        [SerializeField] private Image filledCircle;

        [SerializeField] private float minFOV = 40f;
        [SerializeField] private float cooldown = 5f;

        private GameObject _sphereDetectorInst;
        public GameObject[] arrowDisplayer;
        private GameObject _closestGameObject = null;
        private GameObject _currentGameOBject = null;
        private GameObject _floatingTextInst;

        private Vector3 _currentPosition;

        [SerializeField] private LayerMask enemyHurtBoxLayer;

        private Color _startColorFilledCircle;


        [SerializeField] private float damageMultiplier = 1.05f;
        [SerializeField] private float minCircleShrinkSpeed = 2f;
        [SerializeField] private float maxCircleShrinkSpeed = 4f;
        [SerializeField] private float minCircleSize = 1f;
        [SerializeField] private float maxCircleSize = 4f;
        [SerializeField] private float maxSphereSize = 500f;
        [SerializeField] private float circleSizeGrowSpeed = 1f;
        private float _currentCircleShrinkSpeed;
        private float _layerMultiplier;
        private float _currentAnimLength;
        private float _currentFOV;
        private float _startFOV;
        private float _closestDistance = Mathf.Infinity;
        private float _currentCircleSize;
        private float _accuracy;
        private float _damage;
        private float _originalTimeScale = 1.0f;
        private float _totalCooldown;

        public int attackCountCharging;

        [System.NonSerialized] public bool IsChargingSword;
        [System.NonSerialized] public bool IsSlidingOnEnemies;
        [System.NonSerialized] public bool HasUnlockedAbility;
        private bool _isEventActive;
        private bool _canQuickTimeEvent;
        private bool _isMousePressed;
        private bool _isSphereDetectorSpawned;
        private bool _isSphereDetectorInstNotNull;

        protected override void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _anim = GetComponent<Animator>();
            _cC = GetComponent<CapsuleCollider>();
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCLayers = GetComponent<MainCLayers>(); 
            _mainCSwitch = GetComponent<MainCSwitchWeapon>();
            _mainCameraObj = GameObject.FindGameObjectWithTag(string.Format("MainCamera"));
            _mainCamera = _mainCameraObj.GetComponent<Camera>();
            _mainCAnimator = GetComponent<MainCAnimatorController>();
            _mainCMove = GetComponent<MainCMovement>();

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.Attack.performed += MouseDown;
            _playerInputActions.PlayerPC.Attack.canceled += MouseUp;
            
            _mainCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            _currentFOV = _mainCamera.fieldOfView;
            _startFOV = _currentFOV;
            _originalTimeScale = Time.timeScale;
            _startColorFilledCircle = filledCircle.color;
            base.Awake();
        }

        private protected void Update()
        {
            StartChargingSword();
            _mainCamera.fieldOfView = _currentFOV;
            OnQuickTimeEvent();
            LookAtFloatingText();
            if (HasUnlockedAbility && _isMousePressed)
            {
                if (_typeOfAttack == TypeOfAttack.ChargedAttack && !IsChargingSword)
                {
                    IsChargingSword = true;
                    SpawnSphereDetector();
                }
            }
        }

        private void MouseDown(InputAction.CallbackContext context)
        {
            _isMousePressed = true;
        }

        private void MouseUp(InputAction.CallbackContext context)
        {
            _isMousePressed = false;
            CheckGameObjectsOnList();
            DestroyArrowsDisplayer();
            DisableTriggerSphere();
            DisableTriggerSphere();
            _totalCooldown = Time.time + 4f;
        }

        private void StartChargingSword()
        {
            if (_typeOfAttack == TypeOfAttack.ChargedAttack && !IsSlidingOnEnemies && Time.time > _totalCooldown && !_mainCSwitch.isUsingPistol && HasUnlockedAbility && _mainCMove.IsGrounded())
            {
                arrowDisplayer = GameObject.FindGameObjectsWithTag("ArrowDisplayer");

                foreach (var gameObj in arrowDisplayer)
                {
                    if (gameObj)
                    {
                        var arrowDisTrs = gameObj.transform;

                        arrowDisTrs.transform.LookAt(_mainCameraObj.transform.position);

                        arrowDisTrs.Rotate(90, 0, 0);
                    }
                }

                if (_sphereDetectorInst)
                {
                    var sphereTr = _sphereDetectorInst.transform;

                    if (sphereTr.localScale.x >= maxSphereSize) return;

                    sphereTr.localScale += new Vector3(circleSizeGrowSpeed, circleSizeGrowSpeed, circleSizeGrowSpeed);
                }
            }
        }

        private void SpawnSphereDetector()
        {
            if (!_isSphereDetectorSpawned && IsChargingSword)
            {
                GameManagerSingleton.Instance.ClearClosestsGameObject();
                GameManagerSingleton.Instance.ClearGameObject();

                _sphereDetectorInst =
                    Instantiate(sphereDetectorObj, groundTr.transform.position, groundTr.transform.rotation);
                _isSphereDetectorSpawned = true;
            }
        }

        private void CheckGameObjectsOnList()
        {
            if (IsChargingSword)
            {
                if (GameManagerSingleton.Instance.gameObjectsList.Count > 0)
                {
                    foreach (var gameObjects in GameManagerSingleton.Instance.gameObjectsList)
                    {
                        var actualTrs = gameObjects.transform;

                        var distance = Vector3.Distance(transform.position, actualTrs.transform.position);

                        if (distance < _closestDistance)
                        {
                            _closestDistance = distance;
                            _closestGameObject = gameObjects;
                        }

                        GameManagerSingleton.Instance.AddClosestGameObject(gameObjects);
                    }

                    StartCoroutine(nameof(SneakThroughEnemies));
                }
                else
                {
                    if (_sphereDetectorInst)
                    {
                        Destroy(_sphereDetectorInst);
                        IsChargingSword = false;
                        IsSlidingOnEnemies = false;
                        _totalCooldown = Time.time + cooldown;
                        _isSphereDetectorSpawned = false;
                    }
                }
            }
        }

        private void DestroyArrowsDisplayer()
        {
            arrowDisplayer = GameObject.FindGameObjectsWithTag("ArrowDisplayer");

            foreach (var gameObjectArrow in arrowDisplayer)
            {
                if (gameObjectArrow != null)
                {
                    Destroy(gameObjectArrow);
                }
            }
        }

        private IEnumerator SneakThroughEnemies()
        {
            //mesh.SetActive(false);F
            _cc.enabled = false;
            IsSlidingOnEnemies = true;
            yield return new WaitForSeconds(0.1f);
            var gameObjectsCopy = new List<GameObject>(GameManagerSingleton.Instance.closestGameObjectsList);

            if (GameManagerSingleton.Instance.closestGameObjectsList.Count == 0)
            {
                if (_sphereDetectorInst)
                {
                    Destroy(_sphereDetectorInst);
                    _isSphereDetectorSpawned = false;
                    IsChargingSword = false;
                }
            }

            foreach (var gameObj in gameObjectsCopy.Where(gameObj => !_isEventActive))
            {
                mesh.SetActive(false);
                var closestTr = gameObj.transform;
                var targetPosition = closestTr.position;

                var position = transform.position;

                if (Physics.Raycast(closestTr.transform.position, -closestTr.transform.forward, 1.5f, enemyHurtBoxLayer))
                {
                    if (Physics.Raycast(closestTr.transform.position, closestTr.transform.forward, 1.5f, enemyHurtBoxLayer))
                    {
                        if (Physics.Raycast(closestTr.transform.position, closestTr.transform.right, 1.5f,
                                enemyHurtBoxLayer))
                        {
                            if (Physics.Raycast(closestTr.transform.position, -closestTr.transform.right, 1.5f,
                                    enemyHurtBoxLayer))
                            {
                            }
                            else
                            {
                                var desiredPos = new Vector3(closestTr.position.x, transform.position.y,
                                    closestTr.position.z);
                                transform.position = desiredPos + -closestTr.transform.right;
                            }
                        }
                        else
                        {
                            var desiredPos = new Vector3(closestTr.position.x, transform.position.y, closestTr.position.z);
                            transform.position = desiredPos + closestTr.transform.right;
                        }
                    }
                    else
                    {
                        var desiredPos = new Vector3(closestTr.position.x, transform.position.y, closestTr.position.z);
                        transform.position = desiredPos + closestTr.transform.forward;
                    }
                }
                else
                {
                    var desiredPos = new Vector3(closestTr.position.x, transform.position.y, closestTr.position.z);
                    transform.position = desiredPos + -closestTr.transform.forward;
                }

                var desiredRot = new Vector3(closestTr.position.x, position.y, closestTr.position.z);
                transform.LookAt(desiredRot);

                GameManagerSingleton.Instance.RemoveClosestsGameObject(gameObj);
                mesh.SetActive(true);

                _mainCLayers.EnableAttackLayer();
                _mainCAttack.SetAttackCount(1);
                _mainCAttack.ShowWeapon();
                filledCircle.color = _startColorFilledCircle;
                quickCanvas.SetActive(true);
                _currentGameOBject = gameObj;
                StartQuickTimeEvent();


                yield return new WaitForSecondsRealtime(3.5f);
            }

            //swordTrail.SetActive(false);
            IsSlidingOnEnemies = false;
            _cc.enabled = true;
            IsChargingSword = false;
        }

        private void StartQuickTimeEvent()
        {
            _isEventActive = true;
            Time.timeScale = 0.08f;
            _currentCircleSize = maxCircleSize;

            _currentCircleShrinkSpeed = Random.Range(minCircleShrinkSpeed, maxCircleShrinkSpeed);

            circleImage.gameObject.SetActive(true);
            circleImage.transform.localScale = Vector3.one * _currentCircleSize;
            _canQuickTimeEvent = true;
        }

        private void OnQuickTimeEvent()
        {
            if (_canQuickTimeEvent)
            {
                var shrinkAmount = _currentCircleShrinkSpeed * Time.unscaledDeltaTime;
                _currentCircleSize -= shrinkAmount;
                _currentCircleSize = Mathf.Clamp(_currentCircleSize, minCircleSize, maxCircleSize);

                var newScale = Vector3.one * _currentCircleSize;
                circleImage.transform.localScale = newScale;

                if (_currentCircleSize <= minCircleSize)
                {
                    EndQuickTimeEvent(false);
                }


                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    // El jugador presionó la barra espaciadora
                    EndQuickTimeEvent(true);
                }
            }
        }

        private void EndQuickTimeEvent(bool success)
        {
            // Finalizar el QTE y calcular el daño basado en la precisión
            _isEventActive = false;
            _canQuickTimeEvent = false;
            if (success)
            {
                var clampedCircleSize = Mathf.Clamp(_currentCircleSize, minCircleSize, maxCircleSize);
                _accuracy = 100f * (maxCircleSize - clampedCircleSize) / (maxCircleSize - minCircleSize);

                if (_accuracy >= 90f)
                {
                    filledCircle.color = Color.green;
                    _damage = 100f; // Daño máximo cuando la precisión está entre 90-100%
                    if (floatingTextPrefab)
                    {
                        ShowFloatingTextDamage(_damage);
                    }

                    DummieHurtBox dummieHurtBox;

                    dummieHurtBox = _currentGameOBject.GetComponentInChildren<DummieHurtBox>();
                    dummieHurtBox.TakeDamage(_damage);
                }
                else
                {
                    filledCircle.color = Color.red;
                    damageMultiplier =
                        Mathf.Lerp(1f, 0f,
                            (90f - _accuracy) / 90f); // Ajustar el multiplicador de daño en función de la precisión
                    _damage = _accuracy * damageMultiplier;
                    if (floatingTextPrefab)
                    {
                        ShowFloatingTextDamage(_damage);
                    }

                    DummieHurtBox dummieHurtBox;

                    dummieHurtBox = _currentGameOBject.GetComponentInChildren<DummieHurtBox>();
                    dummieHurtBox.TakeDamage(_damage);
                }
            }
            else
            {
                filledCircle.color = Color.red;
                if (floatingTextPrefab)
                {
                    ShowFloatingText("Miss!");
                }
            }

            Time.timeScale = _originalTimeScale;
        
            if (GameManagerSingleton.Instance.closestGameObjectsList.Count == 0)
            {
                if (_sphereDetectorInst)
                {
                    Destroy(_sphereDetectorInst);
                    _isSphereDetectorSpawned = false;
                    IsChargingSword = false;
                }
            }

            Invoke(nameof(DisableQrteCanvas), 0.1f);
        }

        private bool CheckObstacle()
        {
            var ray = new Ray(transform.position, orientation.transform.forward);

            var hits = Physics.RaycastAll(ray, 4f);

            if (hits.Length > 1)
            {
                var firstHit = hits[0];
                var hitObject = firstHit.collider.gameObject;

                if (!(hitObject == _closestGameObject))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            else
            {
                return true;
            }
        }

        private void ShowFloatingTextDamage(float damage)
        {
            _floatingTextInst = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            _floatingTextInst.GetComponent<TextMeshPro>().text = damage.ToString(("0") + "!");
            _floatingTextInst.transform.Rotate(0, 0, 180);
        }

        private void ShowFloatingText(string text)
        {
            _floatingTextInst = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            _floatingTextInst.GetComponent<TextMeshPro>().text = text;
            _floatingTextInst.transform.Rotate(0, 0, 180);
        }

        private void LookAtFloatingText()
        {
            if (_floatingTextInst)
            {
                //floatingTextInst.transform.LookAt(cameraObj.position);
                var rotation = _mainCameraObj.transform.rotation;
                var targetRotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0f);
                _floatingTextInst.transform.rotation = Quaternion.Slerp(_floatingTextInst.transform.rotation,
                    targetRotation, 200f * Time.deltaTime);
            }
        }

        private void DisableQrteCanvas()
        {
            quickCanvas.SetActive(false);
        }

        public void EnableHasUnlockedAbility()
        {
            HasUnlockedAbility = true;
        }


        private void DisableTriggerSphere()
        {
            if (_sphereDetectorInst)
            {
                var scSphere = _sphereDetectorInst.GetComponent<SphereCollider>();

                if (scSphere.isTrigger == true)
                {
                    scSphere.isTrigger = false;
                }
            }
        }
    }
}