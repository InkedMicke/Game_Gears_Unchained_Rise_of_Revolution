using System.Collections;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Props;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class LabThirdRoomController : MonoBehaviour
    {
        private DummiesColliderLab _dummiesCollider;
        private MainCChargingSwordSphereTarget _chargingSwordSphereTarget;
        private MainCAttack _mainCAttack;
        private MainCSounds _mainCSounds;
        private GStopMenu _gStopMenu;
        private MainCTutorialChecker _mainCTutorial;

        [SerializeField] private GameObject wave1;
        [SerializeField] private GameObject wave2;
        [SerializeField] private GameObject movableFloor;
        [SerializeField] private GameObject dummieControllerObj;
        [SerializeField] private GameObject goHereObj;
        private GameObject _playerObj;
        
        [SerializeField] private float speedOfFloor = 0.1f;
        private float _initalMovableFloatY;

        [SerializeField] private UnityEvent seActivaCuandoVaASubir;
        [SerializeField] private UnityEvent seActivaCuandoVaABajar;
        [SerializeField] private UnityEvent seActivaCuandoHaSubido;
        [SerializeField] private UnityEvent seActivaCuandoHaBajado;
        [SerializeField] private UnityEvent seActivaCuandoLasOleadasTerminan;

        private bool _floorIsUp;
        private bool _floorIsDown;
        private bool _isWave1;
        [System.NonSerialized] public bool IsWave2;
        private bool _isFloorMoving;

        private void Awake()
        {
            _dummiesCollider = dummieControllerObj.GetComponent<DummiesColliderLab>();
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _mainCAttack = _playerObj.GetComponent<MainCAttack>();
            _chargingSwordSphereTarget = _playerObj.GetComponent<MainCChargingSwordSphereTarget>();
            _mainCSounds = _playerObj.GetComponent<MainCSounds>();
            _mainCTutorial = _playerObj.GetComponent<MainCTutorialChecker>();
            _gStopMenu = _playerObj.GetComponent<GStopMenu>();
        }

        private void Start()
        {
            _initalMovableFloatY = movableFloor.transform.localPosition.y;
            _isWave1 = true;
        }

        private void Update()
        {
            if (_floorIsUp && !IsWave2 && wave1.transform.childCount == 0 && !_isFloorMoving)
            {
                if (goHereObj)
                {
                    goHereObj.SetActive(true);
                }
            }
            
            if (_floorIsUp && IsWave2 && wave1.transform.childCount == 0 && !_isFloorMoving)
            {
                seActivaCuandoLasOleadasTerminan.Invoke();
                _mainCAttack.HideWeapon();
                IsWave2 = false;
            }
        }

        public void InvokeMoveUp()
        {
            _dummiesCollider.ClearList();
            wave1.SetActive(true);
            StartCoroutine(MoveUp());
        }

        public void InvokeMoveDown()
        {
            StartCoroutine(MoveDown());
        }

        private IEnumerator MoveUp()
        {
            var enable = true;
            _floorIsDown = false;
            seActivaCuandoVaASubir.Invoke();
            _isFloorMoving = true;
            if (!_isWave1)
            {
                IsWave2 = true;
                //_chargingSwordSphereTarget.EnableHasUnlockedAbility();
            }

            yield return new WaitForSeconds(1f);

            while (enable)
            {
                var temp = movableFloor.transform.localPosition;
                temp.y += speedOfFloor;
                movableFloor.transform.localPosition = temp;
                if (movableFloor.transform.localPosition.y >= 0)
                {
                    if (IsWave2)
                    {
                        _mainCAttack.DisableCanAttack();
                        _mainCAttack.HideWeapon();
                        _mainCSounds.PlayTutorialSound(6, "pc");
                    }
                    if(_isWave1)
                    {
                        _mainCTutorial.SetIsOnTutorialImageBool(true);
                        _mainCTutorial.AttackImage();
                        _gStopMenu.CursorMode(true);
                        _gStopMenu.FreezeTime(true);
                        _mainCSounds.PlayTutorialSound(4, "pc");
                        _mainCAttack.EnableCanAttack();
                    }
                    _dummiesCollider.UndoChild(wave1);
                    temp.y = 0;
                    movableFloor.transform.localPosition = temp;
                    enable = false;
                    seActivaCuandoHaSubido.Invoke();
                    _floorIsUp = true;
                }

                yield return new WaitForSeconds(0.01f);
            }

            _isFloorMoving = false;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MoveDown()
        {
            var enable = true;
            _floorIsUp = false;
            seActivaCuandoVaABajar.Invoke();
            _isFloorMoving = true;
            _isWave1 = false;
            while (enable)
            {
                var temp = movableFloor.transform.localPosition;
                temp.y -= speedOfFloor;
                movableFloor.transform.localPosition = temp;

                if (movableFloor.transform.localPosition.y <= _initalMovableFloatY)
                {
                    wave2.SetActive(true);
                    temp.y = _initalMovableFloatY;
                    movableFloor.transform.localPosition = temp;
                    enable = false;
                    _floorIsDown = true;
                    seActivaCuandoHaBajado.Invoke();
                    _dummiesCollider.ClearList();
                    wave1.SetActive(true);
                    StartCoroutine(MoveUp());
                }

                yield return new WaitForSeconds(0.01f);
            }

            _isFloorMoving = false;
        }
    }
}