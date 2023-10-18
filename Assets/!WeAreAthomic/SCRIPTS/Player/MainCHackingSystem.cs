using System.Collections;
using _WeAreAthomic.SCRIPTS.Player.Robot;
using _WeAreAthomic.SCRIPTS.Props;
using UnityEngine;
using UnityEngine.UI;

namespace _WeAreAthomic.SCRIPTS.Player
{
    public class MainCHackingSystem : MonoBehaviour
    {
        private Animator _anim;
        private MainCLayers _mainCLayers;
        private CharacterController _cc;
        private BastetController _bastetController;

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
        }

        private void Update()
        {
            UpdateUI();
        }

        public void StartHacking(float value)
        {
            FixPosition();
            EnableHackAnim();
            SaveWeapon();
            DisableWeapon();
            _timeToHack = value;
        }

        public void EndHacking()
        {
            hackCanvas.SetActive(false);
            var button = _currentInteract.GetComponent<ButtonInteractable>();
            button.EndHackInvoke();
            _bastetController.InvokeMoveToPlayer();
        }   

        private void UpdateUI()
        {
            if(isHacking)
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
            robotObj.transform.position = new Vector3(_currentInteract.transform.GetChild(0).position.x,
                _currentInteract.transform.GetChild(0).position.y, _currentInteract.transform.GetChild(0).position.z);
        }

        public void FixPosition()
        {
            _cc.enabled = false;
            var interactables = FindObjectsOfType<ButtonInteractable>();

            foreach(var t in interactables)
            {
                if(t.isActive == true)
                {
                    _currentInteract = t.gameObject;
                }
            }
            
            var r = new Ray(_currentInteract.transform.position, _currentInteract.transform.forward);
            var rayPos = r.GetPoint(1f);
            var desiredPos = new Vector3(rayPos.x, transform.position.y, rayPos.z);
            transform.position = desiredPos;
            
            var desiredRot = new Vector3(_currentInteract.transform.GetChild(0).position.x, transform.position.y, _currentInteract.transform.GetChild(0).position.z);
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

        private void SaveWeapon()
        {
            if (pistolObj.activeSelf)
            {
                _currentWeapon = pistolObj;
            }

            if (wrenchObj.activeSelf)
            {
                _currentWeapon = wrenchObj;
            }
        }

        private void DisableWeapon()
        {
            _currentWeapon.SetActive(false);
        }
        private void EnableWeapon()
        {
            _currentWeapon.SetActive(true);
        }

        public void EndAnimHack()
        {
            _mainCLayers.DisableHackLayer();
            EnableWeapon();
            isHackingAnim = false;
            StartCoroutine(Hack(_timeToHack));
            hackCanvas.SetActive(true);
            isHacking = true;
            hackSlider.minValue = Time.time;
            hackSlider.maxValue = Time.time + _timeToHack;
            _cc.enabled = true;
        }
    }
}
