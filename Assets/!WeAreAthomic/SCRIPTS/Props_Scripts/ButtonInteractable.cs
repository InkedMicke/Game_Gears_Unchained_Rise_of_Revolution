using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public enum TypeOfHacked
    {
        prop,
        soldier
    }

    public class ButtonInteractable : MonoBehaviour, IInteractable
    {
        private MainCHackingSystem _mainCHacking;

        [SerializeField] private TypeOfHacked typeOfHacked;

        [SerializeField] private GameObject _eButtonObj;
        [SerializeField] private GameObject _circleObj;
        private GameObject _cameraObj;
        private GameObject _playerObj;

        private Transform _playerTr;

        public bool isActive;
        public bool canHack = true;
        private bool _isShowingButton;

        [SerializeField] private float timeToHack;

        public UnityEvent seActivacuandoLeDasAlBotonYNoPuedesHackear;
        public UnityEvent seActivanCuandoLeDasAlBoton;
        public UnityEvent seActivanCuandoTerminaElHack;
        public UnityEvent seActivanCuandoEstasHackenadoYSeCancela;

        protected virtual void Awake()
        {
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _playerTr = _playerObj.transform;
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            _mainCHacking = _playerObj.GetComponent<MainCHackingSystem>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            ShowCircle();
        }

        private void ShowCircle()
        {
            if (Vector3.Distance(transform.position, _playerTr.position) < 10 && !_isShowingButton && !_mainCHacking.IsHacking)
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
            if (canHack && !_mainCHacking.IsHacking)
            {
                seActivanCuandoLeDasAlBoton.Invoke();
                ToggleActive();
                _mainCHacking.StartHacking(timeToHack, typeOfHacked);
                HideButton();
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
            if (!_mainCHacking.IsHacking) 
            {
                _isShowingButton = true;
                _eButtonObj.SetActive(true);
            }
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
                seActivanCuandoEstasHackenadoYSeCancela.Invoke();
            }
        }

    }
}