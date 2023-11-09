using System.Collections;
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

        private UnityEngine.SceneManagement.Scene _currentScene;

        [SerializeField] private GameObject breatherObj;
        [SerializeField] private GameObject botonPosaMano;

        private Vector3 _lastPosition;

        [System.NonSerialized] public bool IsOnTutorial;
        private bool _isRoom1;
        private bool _isRoom2;
        private bool _isRoom3;

        private float _distanciaRecorrida = 0.0f;
        private float _startDistance;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _mainCSounds = GetComponent<MainCSounds>();
            _mainCHealth = GetComponent<MainCHealthManager>();
            _mainCHacking = GetComponent<MainCHackingSystem>();
        }

        private void Start()
        {
            _startDistance = _cc.transform.position.magnitude;
            _currentScene = SceneManager.GetActiveScene();
            if (_currentScene.name == "S2_LABTUTORIAL")
            {
                IsOnTutorial = true;
                StartCoroutine(CheckDistance());
                StartCoroutine(CheckHealth());
                StartCoroutine(CheckHacking());
            }
        }

        private IEnumerator CheckDistance()
        {
            var enable = true;
            while (enable)
            {
                var position = _cc.transform.position;
                var desplazamiento = position - _lastPosition;
                _distanciaRecorrida += desplazamiento.magnitude;
                _lastPosition = position;

                if (_distanciaRecorrida >= _startDistance + 20f)
                {
                    enable = false;
                    _mainCSounds.RemoveAllSounds();
                    _mainCSounds.PlayExpressionSound();
                    var lengthOfClip = _mainCSounds.GetAudioClipLength(_mainCSounds.CurrentExpressionClip.name);
                    Invoke(nameof(PlayTutorialOne), lengthOfClip);
                }

                yield return new WaitForSeconds(0.01f);
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
                    var lengthOfClip = _mainCSounds.GetAudioClipLength(_mainCSounds.CurrentExpressionClip.name);
                    Invoke(nameof(PlayTutorialTwo), lengthOfClip);
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

        private void PlayTutorialOne()
        {
            _mainCSounds.PlayTutorialSound(1, "pc");
            var healthBreather = breatherObj.GetComponent<HealthBreather>();
            healthBreather.EnableBreather();
        }

        private void PlayTutorialTwo()
        {
            _mainCSounds.PlayTutorialSound(2, "pc");
            var buttonInt = botonPosaMano.GetComponent<ButtonInteractable>();
            buttonInt.EnableCanHack();
        }

        private void PlayTutorialThird()
        {
            _mainCSounds.PlayTutorialSound(3, "pc");
        }
    }
}