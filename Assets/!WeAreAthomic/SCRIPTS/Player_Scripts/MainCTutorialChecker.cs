using System;
using System.Collections;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Props;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCTutorialChecker : MonoBehaviour
    {
        private CharacterController _cc;
        private MainCSounds _mainCSounds;
        private MainCHealthManager _mainCHealth;
        private MainCHackingSystem _mainCHacking;
        private GStopMenu _gStopMenu;

        private Scene _currentScene;

        [SerializeField] private GameObject breatherObj;
        [SerializeField] private GameObject botonPosaMano;
        [SerializeField] private GameObject goHereMove;
        [SerializeField] private GameObject goHereBreather;
        [SerializeField] private GameObject goHerePosaMano;
        [SerializeField] private GameObject goHerePosaManoMove;
        [SerializeField] private GameObject wasdImage;
        [SerializeField] private GameObject eImage;
        [SerializeField] private GameObject izqImage;

        private Vector3 _lastPosition;

        [System.NonSerialized] public bool IsOnTutorial;
        [System.NonSerialized] public bool IsOnTutorialImage;
        private bool _isRoom1;
        private bool _isRoom2;
        private bool _isRoom3;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCHealth = GetComponent<MainCHealthManager>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
            _gStopMenu = GetComponent<GStopMenu>();
        }

        private void Update()
        {

        }

        private void Start()
        {
            _currentScene = SceneManager.GetActiveScene();
            if (_currentScene.name == "S2_LABTUTORIAL")
            {
                IsOnTutorialImage = true;
                IsOnTutorial = true;
                goHereMove.SetActive(true);
                wasdImage.SetActive(true);
                GameManagerSingleton.Instance.SetThereIsCanvasBelow(true);
                _gStopMenu.CursorMode(true);
                _gStopMenu.FreezeTime(true);
                StartCoroutine(CheckHealth());
                StartCoroutine(CheckHacking());
            }
        }

        private IEnumerator CheckHealth()
        {
            var enable = true;

            while (enable)
            {
                if (_mainCHealth.currentHealth >= 100)
                {
                    enable = false;
                    _mainCSounds.RemoveAllSounds();
                    _mainCSounds.PlayExpressionSound();
                    goHerePosaManoMove.SetActive(true);
                    goHereBreather.SetActive(false);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator CheckHacking()
        {
            var enable = true;

            while (enable)
            {
                if (_mainCHacking.isHacking)
                {
                    enable = false;
                    _mainCSounds.RemoveAllSounds();
                    _mainCSounds.PlayExpressionSound();
                    var lengthOfClip = _mainCSounds.GetAudioClipLength(_mainCSounds.CurrentExpressionClip.name);
                    Invoke(nameof(PlayTutorialThird), lengthOfClip);
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        public void PlayTutorialOne()
        {
            _mainCSounds.RemoveAllSounds();
            _mainCSounds.PlayTutorialSound(1, "pc");
            var healthBreather = breatherObj.GetComponent<HealthBreather>();
            healthBreather.EnableBreather();
            goHereBreather.SetActive(true);
        }

        public void PlayTutorialTwo()
        {
            IsOnTutorialImage = true;
            _gStopMenu.CursorMode(true);
            _gStopMenu.FreezeTime(true);
            GameManagerSingleton.Instance.SetThereIsCanvasBelow(true);
            eImage.SetActive(true);
            goHereBreather.SetActive(false);
            _mainCSounds.PlayTutorialSound(2, "pc");
            var buttonInt = botonPosaMano.GetComponent<ButtonInteractable>();
            buttonInt.EnableCanHack();
            goHerePosaMano.SetActive(true);
        }

        private void PlayTutorialThird()
        {
            _mainCSounds.PlayTutorialSound(3, "pc");
        }

        public void AttackImage()
        {
            izqImage.SetActive(true);
        }

        public void SetIsOnTutorialImageBool(bool condition)
        {
            IsOnTutorialImage = condition;
        }
    }
}