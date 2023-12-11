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
        private MainCAttack _mainCAttack;

        private Coroutine _checkHealthCoroutine;
        private Coroutine _checkHackCoroutine;

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
        private bool _isRoom1;
        private bool _isRoom2;
        private bool _isRoom3;
        private bool _isCheckingHealth;
        private bool _isCheckingHack;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCHealth = GetComponentInChildren<MainCHealthManager>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
            _gStopMenu = GetComponent<GStopMenu>();
            _mainCAttack = GetComponent<MainCAttack>();
        }

        private void Start()
        {
            _currentScene = SceneManager.GetActiveScene();
            if (_currentScene.name == "S2_LABTUTORIAL")
            {
                GameManagerSingleton.Instance.SetHasUnlockedBastetAttack(false);
                IsOnTutorial = true;
                goHereMove.SetActive(true);
                wasdImage.SetActive(true);
                _checkHealthCoroutine = StartCoroutine(CheckHealth());
                _checkHackCoroutine = StartCoroutine(CheckHacking());
                _mainCAttack.DisableCanAttack();
            }
            else
            {
                _mainCAttack.EnableCanAttack();
                GameManagerSingleton.Instance.SetHasUnlockedBastetAttack(true);
            }
        }

        private IEnumerator CheckHealth()
        {
            while (true)
            {
                _isCheckingHealth = true;
                if (_mainCHealth.currentHealth >= 100)
                {
                    _isCheckingHealth = false;
                    _mainCSounds.RemoveAllSounds();
                    _mainCSounds.PlayExpressionSound();
                    goHerePosaManoMove.SetActive(true);
                    goHereBreather.SetActive(false);
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private IEnumerator CheckHacking()
        {
            while (true)
            {
                _isCheckingHack = true;
                if (_mainCHacking.isHacking)
                {
                    _isCheckingHack = false;
                    _mainCSounds.RemoveAllSounds();
                    _mainCSounds.PlayExpressionSound();
                    var lengthOfClip = _mainCSounds.GetAudioClipLength(_mainCSounds.CurrentExpressionClip.name);
                    Invoke(nameof(PlayTutorialThird), lengthOfClip);
                    eImage.GetComponent<Animator>().SetTrigger(string.Format("close"));
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        public void StopAllTutorialCoroutine()
        {
            if(_isCheckingHack)
            {
                StopCoroutine(_checkHackCoroutine);
            }

            if(_isCheckingHealth)
            {
                StopCoroutine(_checkHealthCoroutine);
            }
        }

        public void HideAllTutorialImages()
        {
            izqImage.SetActive(false);
            eImage.SetActive(false);
            wasdImage.SetActive(false);
            _mainCSounds.RemoveAllTutorialSounds();
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
    }
}