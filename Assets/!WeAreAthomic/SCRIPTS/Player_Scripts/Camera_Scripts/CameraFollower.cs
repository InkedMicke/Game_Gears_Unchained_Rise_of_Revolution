using UnityEngine;
using UnityEngine.InputSystem;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts.Camera_Scripts
{
    public class CameraFollower : MonoBehaviour
    {
        protected PlayerInputActions _playerInputActions;

        protected Quaternion LocalRotation;

        protected Vector2 controllerVector;
        protected Vector2 mouseVector;

        public Transform cameraFollow;

        [SerializeField] protected float controllerSensitivityX = 100;
        [SerializeField] protected float controllerSensitivityY = 100;
        [SerializeField] protected float mouseSensitivityX = 100;
        [SerializeField] protected float mouseSensitivityY = 100;
        [SerializeField] protected float clampAngle = 80.0f;
        protected float _rotY;
        protected float _rotX;
        protected float _finalInputX;
        protected float _finalInputY;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start()
        {
            cameraFollow = GameObject.FindGameObjectWithTag("Camera_Follow").transform;

            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Camera.Enable();
        }

        private protected virtual void Update()
        {

            if (!GameManagerSingleton.Instance.IsGamePaused)
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
            transform.rotation = LocalRotation;
        }

        private void LateUpdate()
        {
            var target = cameraFollow.transform;

            //move towards the game object that is the target
            var step = 20 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }
}