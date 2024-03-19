using Enemy;
using Player;
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

    [RequireComponent(typeof(C_MaterialChanger))]

    public class ButtonInteractable : MonoBehaviour, IInteractable
    {
        private MainCHackingSystem _mainCHacking;
        private C_MaterialChanger[] _materialChanger;

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
            _materialChanger = GetComponents<C_MaterialChanger>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            ShowCircle();
        }

        private void ShowCircle()
        {
            if (Vector3.Distance(transform.position, _playerTr.position) < 10 && CanShowButton())
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

                if (typeOfHacked != TypeOfHacked.soldier)
                {
                    for (int i = 0; i < _materialChanger.Length; i++)
                    {

                        _materialChanger[i].OnPlayerCatched();
                    }
                }
                
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
            if (CanShowButton()) 
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
            HideButton();
        }

        bool CanShowButton()
        {
            if(_isShowingButton)
            {
                Debug.Log("hola1");
                return false;
            }

            if(_mainCHacking.IsHacking)
            {
                Debug.Log("hola2");
                return false;
            }

            if(!canHack)
            {
                Debug.Log("hola3");
                return false;
            }

            if(_mainCHacking.GotCached)
            {
                Debug.Log("hola4");
                return false;
            }

            return true;
        }

    }
}