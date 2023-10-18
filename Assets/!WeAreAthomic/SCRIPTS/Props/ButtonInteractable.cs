using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class ButtonInteractable : MonoBehaviour, IInteractable
    {

        public GameObject eButtonObj;
        public GameObject circleObj;
        private GameObject _cameraObj;

        private Transform _playerTr;

        public bool isActive;
        public bool activatedSomething;
        private bool _isShowingButton;

        public UnityEvent[] condicionesParaQueSeActive;
        public UnityEvent[] seActivanCuandoLeDasAlBoton;
        public UnityEvent[] seActivanCuandoTerminaElHack;

        private void Awake()
        {

        }

        private void Start()
        {
            _playerTr = GameObject.FindGameObjectWithTag("Player").transform;
            _cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        }

        // Update is called once per frame
        private void Update()
        {
            ShowCircle();
        }

        private void ShowCircle()
        {
            if (Vector3.Distance(transform.position, _playerTr.position) < 10 && !_isShowingButton)
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
            foreach (var i in seActivanCuandoLeDasAlBoton)
            {
                i.Invoke();
            }
        }

        public void EndHackInvoke()
        {
            foreach (var i in seActivanCuandoTerminaElHack)
            {
                i.Invoke();
            }
        }

        public void ShowButton()
        {
            _isShowingButton = true;
            eButtonObj.SetActive(true);
        }

        public void HideButton()
        {
            _isShowingButton = false;
            eButtonObj.SetActive(false);
        }

        public void ToggleActivateSomething()
        {
            activatedSomething = !activatedSomething;
        }

    }
}