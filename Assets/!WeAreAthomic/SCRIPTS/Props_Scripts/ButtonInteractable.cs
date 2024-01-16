using _WeAreAthomic.SCRIPTS.Enemi_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public enum TypeOfHacked
    {
        botonPuerta,
        botonCamara,
        soldier
    }

    public class ButtonInteractable : MonoBehaviour, IInteractable
    {
        private MainCHackingSystem _mainCHacking;

        public TypeOfHacked typeOfHacked;

        [SerializeField] private GameObject soldier;
        [SerializeField] private GameObject eButtonObj;
        [SerializeField] private GameObject eastButtonObj;
        [SerializeField] private GameObject circleObj;
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
            if (Vector3.Distance(transform.position, _playerTr.position) < 10 && !_isShowingButton && !_mainCHacking.IsHacking && canHack)
            {
                circleObj.SetActive(true);
            }
            else
            {
                circleObj.SetActive(false);
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
                eButtonObj.SetActive(false);
                if (typeOfHacked == TypeOfHacked.botonPuerta)
                {
                    canHack = false;
                }
                isActive = false;
            }
        }

        public void ShowButton()
        {
            if (!_mainCHacking.IsHacking && canHack) 
            {
                if(typeOfHacked == TypeOfHacked.soldier)
                {
                    if (soldier.GetComponent<Enemy>().IsChasingPlayer)
                    {

                    }
                    else
                    {
                        _isShowingButton = true;
                        switch (GameManagerSingleton.Instance.typeOfInput)
                        {
                            case TypeOfInput.pc:
                                eButtonObj.SetActive(true);
                                break;
                            case TypeOfInput.gamepad:
                                eastButtonObj.SetActive(true);
                                break;
                        }
                        eButtonObj.SetActive(true);
                    }
                }
                else
                {
                    _isShowingButton = true;
                    switch (GameManagerSingleton.Instance.typeOfInput)
                    {
                        case TypeOfInput.pc:
                            eButtonObj.SetActive(true);
                            break;
                        case TypeOfInput.gamepad:
                            eastButtonObj.SetActive(true);
                            break;
                    }
                    eButtonObj.SetActive(true);
                }
            }
        }

        public void HideButton()
        {
            _isShowingButton = false;
            eButtonObj.SetActive(false);
            eastButtonObj.SetActive(false);
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
                isActive = false;
            }
        }

    }
}