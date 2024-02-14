using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using _WeAreAthomic.SCRIPTS.PP_Scripts;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCHackingSystem : MonoBehaviour
    {
        [SerializeField] private PP m_PP;
        private MainCLayers _mainCLayers;
        private CharacterController _cc;
        private BastetController _bastetController;
        private MainCSwitchWeapon _mainSwitchWeapon;
        private MainCSounds _mainCSounds;
        private MainCAnimatorController _mainCAnim;
        private MainCAttack _mainCAttack;

        private Coroutine _hackCoroutine;

        [SerializeField] private Slider hackSlider;

        public GameObject wrenchObj;
        [SerializeField] private GameObject hackCanvas;
        [SerializeField] private GameObject robotObj;
        private GameObject _currentInteract;

        public bool isHackingAnim;
        public bool IsHacking;
        public bool GotCached;
        private bool _isHackingSoldier;

        private float _timeToHack;
        private float _actualTime;

        private void Awake()
        {
            _mainCLayers = GetComponent<MainCLayers>();
            _cc = GetComponent<CharacterController>();
            _bastetController = robotObj.GetComponent<BastetController>();
            _mainSwitchWeapon = GetComponent<MainCSwitchWeapon>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCAnim = GetComponent<MainCAnimatorController>();
            _mainCAttack = GetComponent<MainCAttack>();
        }

        private void Update()
        {
            UpdateUI();
            RobotPos();

            if(_isHackingSoldier)
            {
                if(Vector3.Distance(transform.position, _currentInteract.transform.position) > 6)
                {
                    _isHackingSoldier = false;
                    EndHacking();
                }
            }
        }

        public void StartHacking(float timeHack, TypeOfHacked typeOfHacked)
        {
            if (!IsHacking && !GotCached)
            {
                _timeToHack = timeHack;

                _mainCAnim.SetMoveSpeed(0);

                var interactables = FindObjectsOfType<ButtonInteractable>();

                foreach (var t in interactables)
                {
                    if (t.isActive == true)
                    {
                        _currentInteract = t.gameObject;
                    }
                }

                if (typeOfHacked == TypeOfHacked.botonCamara || typeOfHacked == TypeOfHacked.botonPuerta)
                {
                    FixPosition();
                    EnableHackAnim();
                    _mainCAttack.HideWeapon();
                    _mainCAttack.SetIsSheathed(false);
                }
                else
                {
                    SpawnRobot();
                    EndAnimHack();
                    _isHackingSoldier = true;
                }

            }
        }

        public void EndHacking()
        {
            hackCanvas.SetActive(false);
            m_PP.RemoveObjToCurrentUIGameObjectList(hackCanvas);
            var button = _currentInteract.GetComponent<ButtonInteractable>();
            button.EndHackInvoke();

            var interactables = FindObjectsOfType<ButtonInteractable>();
            foreach (var t in interactables)
            {
                if (t.isActive)
                {
                    t.isActive = false;
                }
            }
            _bastetController.HideScanner();
            _bastetController.GoToRightHandPosUntilReachedPos(true, () => robotObj.SetActive(false), .6f, .1f);
            _mainCSounds.StopHackInProcessSound();
            IsHacking = false;
        }

        private void UpdateUI()
        {
            if (IsHacking)
            {
                hackSlider.value = Time.time;
            }
        }

        public void EnableHackAnim()
        {
            isHackingAnim = true;
            _mainCAnim.TriggerHack();
            _mainCLayers.EnableHackLayer();
        }

        public void SpawnRobot()
        {
            robotObj.SetActive(true);
            _bastetController.ShowScanner();
            IsHacking = true;
        }

        public void RobotPos()
        {
            if (IsHacking && robotObj.activeSelf)
            {
                var position = _currentInteract.transform.position;
                var desiredPosRobot = new Vector3(position.x, position.y - .05f, position.z);
                robotObj.transform.position = desiredPosRobot + _currentInteract.transform.forward * 0.3f;
                var desiredPos = new Vector3(position.x, robotObj.transform.position.y, position.z);
                robotObj.transform.LookAt(desiredPos);
            }
        }

        private void FixPosition()
        {
            _cc.enabled = false;

            var r = new Ray(_currentInteract.transform.position, _currentInteract.transform.forward);
            var rayPos = r.GetPoint(1f);
            var desiredPos = new Vector3(rayPos.x, transform.position.y, rayPos.z);
            transform.position = desiredPos;

            var desiredRot = new Vector3(_currentInteract.transform.GetChild(0).position.x, transform.position.y,
                _currentInteract.transform.GetChild(0).position.z);
            transform.LookAt(desiredRot);
        }

        private IEnumerator Hack(float value)
        {

            _actualTime = Time.time + value;

            while (true)
            {
                if (Time.time > _actualTime)
                {
                    EndHacking();
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        public void SetGotCached(bool gotCahed)
        {
            GotCached = gotCahed;
        }

        public void StopHack()
        {
            if (IsHacking)
            {
                StopCoroutine(_hackCoroutine);
                IsHacking = false;
                isHackingAnim = false;
                hackCanvas.SetActive(false);
                m_PP.RemoveObjToCurrentUIGameObjectList(hackCanvas);
                _mainCSounds.StopHackInProcessSound();
                _cc.enabled = true;
                _mainCLayers.DisableHackLayer();
                _bastetController.HideScanner();
                _bastetController.GoToRightHandPosUntilReachedPos(true, () => robotObj.SetActive(false), .6f, .1f);
                _mainCSounds.PlayCannotHackSound();

                var interactables = FindObjectsOfType<ButtonInteractable>();

                foreach (var t in interactables)
                {
                    if (t.isActive)
                    {
                        t.isActive = false;
                    }
                }
            }
        }

        public void EndAnimHack()
        {
            if (IsHacking)
            {
                _mainCLayers.DisableHackLayer();
                isHackingAnim = false;
                _hackCoroutine = StartCoroutine(Hack(_timeToHack));
                hackCanvas.SetActive(true);
                m_PP.AddObjToCurrentUIGameObjectList(hackCanvas);
                hackSlider.minValue = Time.time;
                hackSlider.maxValue = Time.time + _timeToHack;
                _cc.enabled = true;
                _mainCSounds.PlayHackInProcessSound();
            }
        }
    }
}