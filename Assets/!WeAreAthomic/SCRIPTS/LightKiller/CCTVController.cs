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
        private GameObject _playerObj;
    
        private Transform _lightKillerTr;

        private Quaternion _targetRotation;
    
        public LayerMask groundLayer;
        public LayerMask obstructionLayer;

        [System.NonSerialized] public bool RayGotObstruction;
        private bool _isCameraOff;
    
        private int _currentRotation;
    
        [SerializeField] private float lerpSpeed = 0.05f;
        [SerializeField] private float redLightSpeedWhenChasing = 0.01f;
    
        public List<Vector3> rotations;
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
            if (rotations.Count > 0)
            {
                _currentRotation = Random.Range(0, rotations.Count);
            }
        }

        private void Update()
        {
            if (rotations.Count > 0)
            {
                _targetRotation = Quaternion.Euler(rotations[_currentRotation]);
            }
        
            if (!(transform.rotation == _targetRotation) && !_isCameraOff && !_lKiller.IsFocusingPlayer)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, lerpSpeed);
                RayGotObstruction = false;
            }
            else
            {
                _currentRotation = Random.Range(0, rotations.Count);
            }
        
            switch (_isCameraOff)
            {
                case true:
                {
                    var offRotation = Quaternion.Euler(80f, 0f, 0f);
                    transform.rotation = Quaternion.Lerp(transform.rotation, offRotation, lerpSpeed);
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

                var ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out var hitInfo, 50f, obstructionLayer))
                {
                    if (_lKiller.IsFocusingPlayer)
                    {
                        _lKiller.WhiteLight();
                        RayGotObstruction = true;
                    }
                }
            }
        }
    
        private void SetLightPosition()
        {
            var ray = new Ray(transform.position, transform.forward);
            if(Physics.Raycast(ray, out var hitInfo, 50f, groundLayer))
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

            foreach(var t in seActivanCuandoSeEnciendeLaCamara)
            {
                t.Invoke();
            }

        }
    
    }
}
