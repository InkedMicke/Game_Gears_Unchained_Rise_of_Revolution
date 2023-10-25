using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using _WeAreAthomic.SCRIPTS.Player;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

namespace _WeAreAthomic.SCRIPTS.LightKiller
{
    public class CCTVController : MonoBehaviour
    {
        private LightKiller _lKiller;
        private MainCMovement _mainCMovement;

        [SerializeField] private GameObject lKillerObj;
        [SerializeField] private GameObject transforms;
        private GameObject _playerObj;

        private Transform _lightKillerTr;

        private Quaternion _targetRotation;

        public LayerMask groundLayer;
        public LayerMask obstructionLayer;

        private bool _isCameraOff;

        private int _currentRotation;

        [SerializeField] private float rotationSpeed = 0.05f;
        [SerializeField] private float redLightSpeedWhenChasing = 0.01f;

        [SerializeField] private List<Transform> objectsToLookAt;
        public UnityEvent[] seActivanCuandoSeEnciendeLaCamara;

        private void Awake()
        {
            _lKiller = lKillerObj.GetComponent<LightKiller>();
            _lightKillerTr = lKillerObj.transform;
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _mainCMovement = _playerObj.GetComponent<MainCMovement>();
        }

        private void Start()
        {
            var transformsArray = transforms.GetComponentsInChildren<Transform>();
            foreach (Transform t in transformsArray)
            {
                if (t.CompareTag("CameraTransform"))
                {
                    objectsToLookAt.Add(t);
                }
            }
        }

        private void Update()
        {

            if (objectsToLookAt.Count > 0)
            {
                Vector3 lookDirection = objectsToLookAt[_currentRotation].position - transform.position;

                _targetRotation = Quaternion.LookRotation(lookDirection);

                float step = rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, step);
            }

            var angle = Quaternion.Angle(transform.rotation, _targetRotation);

            if (angle < 5f)
            {
                if (_currentRotation == objectsToLookAt.Count - 1)
                {
                    _currentRotation = 0;
                }
                else
                {
                    _currentRotation++;
                }
            }

            switch (_isCameraOff)
            {
                case true:
                    {
                        var offRotation = Quaternion.Euler(80f, 0f, 0f);
                        transform.rotation = Quaternion.Lerp(transform.rotation, offRotation, 5f);
                        break;
                    }
                case false:
                    LookToPlayer();
                    SetLightPosition();
                    break;
            }
        }
        private void LookToPlayer()
        {
            if (_lKiller.IsFocusingPlayer)
            {
                if (_mainCMovement.IsGrounded())
                {
                    transform.LookAt(_playerObj.transform.position);
                }
                else
                {
                    transform.LookAt(_mainCMovement.PositionOnFloorNotGrounded());
                }

                var playerPos = _playerObj.transform.position;
                var desiredPos = new Vector3(playerPos.x, playerPos.y + 1f, playerPos.z);
                var direction = desiredPos - transform.position;

                var raycastLength = direction.magnitude;

                var ray = new Ray(transform.position, direction.normalized);
                if (Physics.Raycast(ray, raycastLength, obstructionLayer))
                {
                    if (_lKiller.IsFocusingPlayer)
                    {
                        _lKiller.WhiteLight();
                    }
                }
            }
        }

        public void PermanentDisabled()
        {
            TurnOffCamera(Mathf.Infinity);
        }

        private void SetLightPosition()
        {
            var ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hitInfo, 50f, groundLayer))
            {
                if (!_lKiller.IsFocusingPlayer)
                {
                    _lightKillerTr.position = hitInfo.point;
                }
                else
                {
                    _lightKillerTr.position = Vector3.Lerp(_lightKillerTr.position, hitInfo.point, redLightSpeedWhenChasing);
                }
            }
        }

        public void TurnOffCamera(float value)
        {
            _isCameraOff = true;
            lKillerObj.SetActive(false);
            Invoke(nameof(TurnOnCamera), value);
        }

        public void TurnOnCamera()
        {
            lKillerObj.SetActive(true);
            _lKiller.WhiteLight();
            _isCameraOff = false;

            foreach (var t in seActivanCuandoSeEnciendeLaCamara)
            {
                t.Invoke();
            }

        }

    }
}
