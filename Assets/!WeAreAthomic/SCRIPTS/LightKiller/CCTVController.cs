using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _WeAreAthomic.SCRIPTS.LightKiller
{
    public class CCTVController : MonoBehaviour
    {
        private LightKiller _lKiller;

        [SerializeField] private GameObject lKillerObj;
    
        private Transform _lightKillerTr;
        private Transform _playerTr;

        private Quaternion _targetRotation;
    
        public LayerMask groundLayer;
        public LayerMask obstructionLayer;

        [System.NonSerialized] public bool rayGotObstruction;
        private bool _isCameraOff;
    
        private int _currentRotation;
    
        [SerializeField] private float lerpSpeed = 0.05f;
        [SerializeField] private float redLightSpeedWhenChasing = 0.01f;
    
        public List<Vector3> rotations;

        private void Awake()
        {
            _lKiller = lKillerObj.GetComponent<global::_WeAreAthomic.SCRIPTS.LightKiller.LightKiller>();
            _lightKillerTr = lKillerObj.transform;
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;

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
        
            if (!(transform.rotation == _targetRotation) && !_isCameraOff && !_lKiller.isFocusingPlayer)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, lerpSpeed);
                rayGotObstruction = false;
            }
            else
            {
                _currentRotation = Random.Range(0, rotations.Count);
            }
        
            if(_isCameraOff)
            {
                Quaternion offRotation = Quaternion.Euler(80f, 0f, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, offRotation, lerpSpeed);
            }

            if(!_isCameraOff)
            {
                LookToPlayer();
                SetLightPosition();
            }
        }
        void LookToPlayer()
        {
            if (_lKiller.isFocusingPlayer)
            {
                transform.LookAt(_playerTr.position);

                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 50f, obstructionLayer))
                {
                    if (_lKiller.isFocusingPlayer)
                    {
                        _lKiller.WhiteLight();
                        rayGotObstruction = true;
                    }
                }
            }
        }
    
        public void SetLightPosition()
        {
            var ray = new Ray(transform.position, transform.forward);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, 50f, groundLayer))
            {
                if (!_lKiller.isFocusingPlayer)
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
        }
    
    }
}
