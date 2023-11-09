using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Props;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class ButtonInteractable : MonoBehaviour, IInteractable
    {
        private RequiredActionForButton _requiredAction;
        private MainCHackingSystem _mainCHacking;

        [SerializeField] private GameObject _eButtonObj;
        [SerializeField] private GameObject _circleObj;
        private GameObject _cameraObj;

        private Transform _playerTr;

        public bool isActive;
        public bool canHack = true;
        private bool _isShowingButton;

        public UnityEvent seActivacuandoLeDasAlBotonYNoPuedesHackear;
        public UnityEvent seActivanCuandoLeDasAlBoton;
        public UnityEvent seActivanCuandoTerminaElHack;

        protected virtual void Awake()
        {
            _requiredAction = GetComponent<RequiredActionForButton>();
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            _mainCHacking = GameObject.FindGameObjectWithTag("Player").GetComponent<MainCHackingSystem>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            ShowCircle();
        }

        private void ShowCircle()
        {
            if (Vector3.Distance(transform.position, _playerTr.position) < 10 && !_isShowingButton)
            {
                _circleObj.SetActive(true);
            }
            else
            {
                _circleObj.SetActive(false);
            }
        }

        public void ToggleActive()
        {
            isActive = !isActive;
        }

        public void Interact()
        {
            if (canHack)
            {
                seActivanCuandoLeDasAlBoton.Invoke();
            }
        }

        public void EndHackInvoke()
        {
            if (canHack)
            {
                seActivanCuandoTerminaElHack.Invoke();
            }
        }

        public void ShowButton()
        {
            _isShowingButton = true;
            _eButtonObj.SetActive(true);
        }

        public void HideButton()
        {
            _isShowingButton = false;
            _eButtonObj.SetActive(false);
        }

        public void EnableCanHack()
        {
            canHack = true;
        }

        public void DisableCanHack()
        {
            canHack = false;
            if (isActive)
            {
                _mainCHacking.StopHack();
            }
        }

    }
}