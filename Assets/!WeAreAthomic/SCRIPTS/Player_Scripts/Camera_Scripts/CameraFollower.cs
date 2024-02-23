using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts
{
    public class CameraFollower : MonoBehaviour
    {
        protected PlayerInputActions _playerInputActions;

        protected Quaternion LocalRotation;

        [SerializeField] private LayerMask _enemyLayer;

        private SceneLoaderStart m_currentScene;

        protected Vector2 controllerVector;
        protected Vector2 mouseVector;

        public Transform cameraFollow;
        [SerializeField] private Transform playerTr;
        private Transform _closestEnemyTr;

        private bool _isFocusingEnemy;

        [SerializeField] protected float controllerSensitivityX = 100;
        [SerializeField] protected float controllerSensitivityY = 100;
        [SerializeField] protected float mouseSensitivityX = 100;
        [SerializeField] protected float mouseSensitivityY = 100;
        [SerializeField] protected float clampAngle = 80.0f;
        [SerializeField] private float cameraSpeedFollow = 40f;
        protected float _rotY;
        protected float _rotX;
        protected float _finalInputX;
        protected float _finalInputY;
        private float focusRadius = 10f;

        private void Awake()
        {

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.PlayerPC.FocusEnemy.performed += ToggleFocus;
        }

        private void Start()
        {
            if (!("N2_LAB" == SceneManager.GetActiveScene().name))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            cameraFollow = GameObject.FindGameObjectWithTag("Camera_Follow").transform;
        }

        private protected virtual void Update()
        {
            if (!GameManagerSingleton.Instance.IsGamePaused && !GameManagerSingleton.Instance.IsOnDialogue)
            {
                LookCamera();
            }
            UpdateData();

        }

        private void UpdateData()
        {
            mouseSensitivityX = GameManagerSingleton.Instance.sensivityX;
            mouseSensitivityY = GameManagerSingleton.Instance.sensivityY;
        }

        private void ToggleFocus(InputAction.CallbackContext x)
        {
            _isFocusingEnemy = !_isFocusingEnemy;

            if (_isFocusingEnemy)
            {
                var cols = new Collider[20];
                var colliders = Physics.OverlapSphereNonAlloc(playerTr.position, focusRadius, cols, _enemyLayer);

                if (colliders > 0)
                {
                    Collider enemigoCercano = null;
                    float distanciaMinima = float.MaxValue;

                    foreach (var enemigo in cols)
                    {
                        float distancia = Vector3.Distance(playerTr.position, enemigo.transform.position);
                        if (distancia < distanciaMinima)
                        {
                            distanciaMinima = distancia;
                            enemigoCercano = enemigo;
                        }
                    }

                    _closestEnemyTr = enemigoCercano.transform;
                }
                else
                {
                    _closestEnemyTr = null;
                    _isFocusingEnemy = false;
                }
            }
        }

        private void LookCamera()
        {

            // We setup the rotation of the sticks here

            controllerVector = _playerInputActions.Camera.Look.ReadValue<Vector2>();


            mouseVector = Mouse.current.delta.ReadValue() * Time.smoothDeltaTime;

            _finalInputX = mouseVector.x * mouseSensitivityX + (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad ? controllerVector.x * controllerSensitivityX : 0);
            _finalInputY = -(mouseVector.y * mouseSensitivityY) + (GameManagerSingleton.Instance.typeOfInput == TypeOfInput.gamepad ? -controllerVector.y * controllerSensitivityY : 0);

            _rotY += _finalInputX * Time.deltaTime;
            _rotX += _finalInputY * Time.deltaTime;

            _rotX = Mathf.Clamp(_rotX, -clampAngle, clampAngle);

            LocalRotation = Quaternion.Euler(_rotX, _rotY, 0.0f);
            if (_isFocusingEnemy)
            {
                // Si estás enfocando un enemigo, rota hacia la dirección del enemigo más cercano
                Vector3 direction = _closestEnemyTr.position - transform.position;
                direction.y = 0; // Opcional: Establecer y en 0 para evitar inclinaciones hacia arriba o hacia abajo
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Combina la rotación hacia el enemigo con la rotación local del ratón
                Quaternion combinedRotation = toRotation * Quaternion.Euler(_rotX, _rotY, 0.0f);

                // Aplica la rotación combinada
                transform.rotation = Quaternion.RotateTowards(transform.rotation, combinedRotation, 60f * Time.deltaTime);
            }
            else
            {
                // Si no estás enfocando un enemigo, aplica la rotación local normal
                transform.rotation = LocalRotation;
                //transform.rotation = LocalRotation;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTr.position, focusRadius);
        }

        public void SetCameraFollow(Transform pos)
        {
            cameraFollow = pos;
        }

        private void LateUpdate()
        {
            var target = cameraFollow.transform;

            //move towards the game object that is the target
            transform.position = Vector3.MoveTowards(transform.position, target.position, cameraSpeedFollow * Time.deltaTime);
        }
    }
}