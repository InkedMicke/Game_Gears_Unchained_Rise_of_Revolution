using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using System;
using _WeAreAthomic.SCRIPTS.PP_Scripts;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public enum TypeOfLightKillerHurtBox
    {
        tutorial,
        normal
    }

    public class CctvController : MonoBehaviour
    {
        [NonSerialized] public LightKiller _lKiller;
        [NonSerialized] public MainCMovement MainCMove;
        [NonSerialized] public MainCAnimatorController MainCAnim;
        [NonSerialized] public MainCHealthManager MainCHealth;

        private MainCHackingSystem _mainCHacking;
        private MainCCrouch m_mainCCrouch;

        [NonSerialized] public GroupCctvController GroupCCtvController;
        [NonSerialized] public PP PP;

        public TypeOfLightKillerHurtBox TypeOfLightHurtBox;

        private AudioSource _cameraAudio;

        [SerializeField] private GameObject lKillerObj;
        [SerializeField] private GameObject transforms;
        public GameObject _playerObj;
        private GameObject _volumeCatch;

        private Transform _lightKillerTr;

        private Quaternion _targetRotation;

        public LayerMask groundLayer;
        public LayerMask obstructionLayer;

        private bool _isCameraOff;
        [NonSerialized] public bool HasGroupCamera;

        private int _currentRotation;

        [SerializeField] private float rotationSpeed = 0.05f;
        [SerializeField] private float redLightSpeedWhenChasing = 0.01f;

        [SerializeField] private List<Transform> objectsToLookAt;
        public UnityEvent[] seActivanCuandoSeEnciendeLaCamara;
        public UnityEvent[] seActivaEncuentraUnObstaculoMientrasFocuseaAlPlayer;

        private void Awake()
        {
            _lKiller = lKillerObj.GetComponent<LightKiller>();
            _lightKillerTr = lKillerObj.transform;
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            MainCMove = _playerObj.GetComponent<MainCMovement>();
            _mainCHacking = _playerObj.GetComponent<MainCHackingSystem>();
            m_mainCCrouch = _playerObj.GetComponent<MainCCrouch>();
            MainCAnim = _playerObj.GetComponent<MainCAnimatorController>();
            MainCHealth = _playerObj.GetComponentInChildren<MainCHealthManager>();
            _cameraAudio = GetComponent<AudioSource>();
            GroupCCtvController = GetComponentInParent<GroupCctvController>();
            PP = _playerObj.transform.parent.GetComponent<PP>();

            _volumeCatch = _playerObj.transform.GetChild(_playerObj.transform.childCount - 2).gameObject;
        }
        public void ActivateVolume()
        {
            _volumeCatch.SetActive(true);
        }
        public void DesactivateVolume()
        {
            _volumeCatch.SetActive(false);
        }
        private void Start()
        {
            var transformsArray = transforms.GetComponentsInChildren<Transform>();
            foreach (var t in transformsArray)
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
                        transform.localEulerAngles = new Vector3(55f, 0f, 0f);
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
                if (MainCMove.IsGrounded())
                {
                    transform.LookAt(_playerObj.transform.position);
                }
                else
                {
                    transform.LookAt(MainCMove.PositionOnFloorNotGrounded());
                }

                var playerPos = _playerObj.transform.position;
                var desiredPos = new Vector3(playerPos.x, m_mainCCrouch.IsCrouch ? playerPos.y + 1.5f :playerPos.y + 2f, playerPos.z);
                var direction = desiredPos - transform.position;

                var raycastLength = direction.magnitude;
                var ray = new Ray(transform.position, direction.normalized);
                Debug.DrawRay(transform.position, direction.normalized * raycastLength);
                if (Physics.Raycast(ray, raycastLength, obstructionLayer))
                {
                    if (_lKiller.IsFocusingPlayer)
                    {
                        _lKiller.WhiteLight();
                        _mainCHacking.SetGotCached(false);
                        foreach (var t in seActivaEncuentraUnObstaculoMientrasFocuseaAlPlayer)
                        {
                            t.Invoke();
                        }
                    }
                }
            }
        }

        public void PermanentDisabled()
        {
            TurnOffCamera(Mathf.Infinity);
            CancelInvoke(nameof(TurnOnCamera));
            _cameraAudio.Stop();
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
            _cameraAudio.Stop();
        }

        public void SetHasGroupCamera(bool hasGroup)
        {
            HasGroupCamera = hasGroup;
        }

        public void TurnOnCamera()
        {
            CancelInvoke(nameof(TurnOnCamera));
            lKillerObj.SetActive(true);
            _lKiller.WhiteLight();
            _isCameraOff = false;
            _cameraAudio.Play();
            foreach (var t in seActivanCuandoSeEnciendeLaCamara)
            {
                t.Invoke();
            }

        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position + (Vector3.up * 0.15f), transform.forward * 20f, Color.red);
        }
#endif
    }
}