using System.Collections;
using _WeAreAthomic.SCRIPTS.PP_Scripts;
using _WeAreAthomic.SCRIPTS.Props_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _WeAreAthomic.SCRIPTS.Player_Scripts
{
    public class MainCTutorialChecker : MonoBehaviour
    {
        [SerializeField] private PP m_PP;
        private CharacterController _cc;
        private MainCSounds _mainCSounds;
        private MainCHealthManager _mainCHealth;
        private MainCHackingSystem _mainCHacking;
        private MainCStopMenu _gStopMenu;
        private MainCAttack _mainCAttack;
        private MainCFuryAttack _mainCFuryAttack;

        private Coroutine _checkHealthCoroutine;
        private Coroutine _checkHackCoroutine;

        private Scene _currentScene;

        [SerializeField] private GameObject botonPosaMano;
        [SerializeField] private GameObject goHereBreather;
        [SerializeField] private GameObject goHerePosaManoMove;



        private Vector3 _lastPosition;

        [System.NonSerialized] public bool IsOnTutorial;
        [System.NonSerialized] public bool S;
        public bool FirstTimeAbility;
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
            _gStopMenu = GetComponent<MainCStopMenu>();
            _mainCAttack = GetComponent<MainCAttack>();
            _mainCFuryAttack = GetComponent<MainCFuryAttack>();
        }

        private void Start()
        {
            _currentScene = SceneManager.GetActiveScene();
            if (_currentScene.name == "S2_LABTUTORIAL" || _currentScene.name == "N2_LAB")
            {
                GameManagerSingleton.Instance.SetHasUnlockedBastetAttack(false);
                IsOnTutorial = true;
                FirstTimeAbility = true;
              
               
                _checkHealthCoroutine = StartCoroutine(CheckHealth());
                _checkHackCoroutine = StartCoroutine(CheckHacking());
                _mainCAttack.DisableCanAttack();
                GameManagerSingleton.Instance.SetThereIsCanvasBelow(true);
            }
            else
            {
                _mainCAttack.EnableCanAttack();
                GameManagerSingleton.Instance.SetHasUnlockedBastetAttack(true);
                _mainCFuryAttack.SetHasUnlockedAbilityAttack(true);
            }
        }

        public void SetFirstAbilityAttack(bool condition)
        {
            FirstTimeAbility = condition;
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
                if (_mainCHacking.IsHacking)
                {
                    _isCheckingHack = false;
                    
                
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

  

     
    }
}