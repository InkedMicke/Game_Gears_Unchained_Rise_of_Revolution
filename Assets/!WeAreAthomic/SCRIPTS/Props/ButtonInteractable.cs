using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class ButtonInteractable : MonoBehaviour, IInteractable
    {
        RequiredActionForButton _requiredAction;

        [SerializeField] private GameObject _eButtonObj;
        [SerializeField] private GameObject _circleObj;
        private GameObject _cameraObj;

        private Transform _playerTr;

        public bool isActive;
        public bool hasActivatedSomething;
        private bool _isShowingButton;

        public UnityEvent seActivanCuandoLeDasAlBoton;
        public UnityEvent seActivanCuandoTerminaElHack;

        protected virtual void Awake()
        {
            _requiredAction = GetComponent<RequiredActionForButton>();
        }

        protected virtual void Start()
        {
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
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
            if (_requiredAction.isRequiredAction)
            {
                ButtonInteractable button = _requiredAction.requiredObject.GetComponent<ButtonInteractable>();
                if (button.hasActivatedSomething)
                {
                    seActivanCuandoLeDasAlBoton.Invoke();
                }
            }

            if(_requiredAction.isRequiredAction == false)
            {
                seActivanCuandoLeDasAlBoton.Invoke();
            }
        }

        public void EndHackInvoke()
        {
            seActivanCuandoTerminaElHack.Invoke();
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

        public void ToggleActivateSomething()
        {
            hasActivatedSomething = !hasActivatedSomething;
        }

    }
}