using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts.Robot_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCHackingSystem : MonoBehaviour
    {
        private Animator _anim;
        private MainCLayers _mainCLayers;
        private CharacterController _cc;
        private BastetController _bastetController;
        private MainCSwitchWeapon _mainSwitchWeapon;
        private MainCSounds _mainCSounds;

        private Coroutine _hackCoroutine;

        [SerializeField] private Slider hackSlider;

        [SerializeField] private GameObject wrenchObj;
        [SerializeField] private GameObject pistolObj;
        [SerializeField] private GameObject hackCanvas;
        [SerializeField] private GameObject robotObj;
        private GameObject _currentInteract;
        private GameObject _currentWeapon;

        public bool isHackingAnim;
        public bool isHacking;

        private float _timeToHack;
        private float _actualTime;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _mainCLayers = GetComponent<MainCLayers>();
            _cc = GetComponent<CharacterController>();
            _bastetController = robotObj.GetComponent<BastetController>();
            _mainSwitchWeapon = GetComponent<MainCSwitchWeapon>();
            _mainCSounds = GetComponent<MainCSounds>();
        }

        private void Update()
        {
            UpdateUI();
        }

        public void StartHacking(float value)
        {
            if (!isHacking)
            {
                FixPosition();
                EnableHackAnim();
                DisableWeapon();
                _timeToHack = value;
            }
        }

        public void EndHacking()
        {
            hackCanvas.SetActive(false);
            var button = _currentInteract.GetComponent<ButtonInteractable>();
            button.EndHackInvoke();
            _bastetController.InvokeMoveToPlayer();
            _mainCSounds.StopHackInProcessSound();
            isHacking = false;
        }

        private void UpdateUI()
        {
            if (isHacking)
            {
                hackSlider.value = Time.time;
            }
        }

        public void EnableHackAnim()
        {
            isHackingAnim = true;
            _anim.SetTrigger(string.Format("hack"));
            _mainCLayers.EnableHackLayer();
        }

        public void SpawnRobot()
        {
            robotObj.SetActive(true);
            var position = _currentInteract.transform.position;
            var desiredPosRobot = new Vector3(position.x, position.y - .05f, position.z);
            robotObj.transform.position = desiredPosRobot + _currentInteract.transform.forward * 0.3f;
            Debug.DrawRay(position, _currentInteract.transform.forward * 0.8f, Color.magenta, 10f);
            var desiredPos = new Vector3(position.x, robotObj.transform.position.y, position.z);
            robotObj.transform.LookAt(desiredPos);
            _bastetController.InvokeRotation();
        }

        private void FixPosition()
        {
            _cc.enabled = false;
            var interactables = FindObjectsOfType<ButtonInteractable>();

            foreach (var t in interactables)
            {
                if (t.isActive == true)
                {
                    _currentInteract = t.gameObject;
                }
            }

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
            var canEnableLayer = true;

            _actualTime = Time.time + value;

            while (canEnableLayer)
            {
                if (Time.time > _actualTime)
                {
                    canEnableLayer = false;
                    EndHacking();
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        public void StopHack()
        {
            StopCoroutine(_hackCoroutine);
            isHacking = false;
            isHackingAnim = false;
            hackCanvas.SetActive(false);
            _mainCSounds.StopHackInProcessSound();
            _cc.enabled = true;
            _bastetController.InvokeMoveToPlayer();
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

        private void DisableWeapon()
        {
            if (_mainSwitchWeapon.isUsingWrench)
            {
                if (wrenchObj.activeInHierarchy)
                {
                    wrenchObj.SetActive(false);
                }
            }

            if (_mainSwitchWeapon.isUsingPistol)
            {
                if (pistolObj.activeInHierarchy)
                {
                    pistolObj.SetActive(false);
                }
            }
        }

        private void EnableWeapon()
        {
            if (_mainSwitchWeapon.isUsingWrench)
            {
                if (!wrenchObj.activeInHierarchy)
                {
                    wrenchObj.SetActive(true);
                }
            }

            if (_mainSwitchWeapon.isUsingPistol)
            {
                if (!pistolObj.activeInHierarchy)
                {
                    pistolObj.SetActive(true);
                }
            }
        }

        public void EndAnimHack()
        {
            _mainCLayers.DisableHackLayer();
            isHackingAnim = false;
            _hackCoroutine = StartCoroutine(Hack(_timeToHack));
            hackCanvas.SetActive(true);
            isHacking = true;
            hackSlider.minValue = Time.time;
            hackSlider.maxValue = Time.time + _timeToHack;
            _cc.enabled = true;
            _mainCSounds.PlayHackInProcessSound();
        }
    }
}