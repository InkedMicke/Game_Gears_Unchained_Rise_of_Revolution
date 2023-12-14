using System.Collections;
using _WeAreAthomic.SCRIPTS.Genericos_Scripts;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using _WeAreAthomic.SCRIPTS.Props;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props_Scripts
{
    public class LabThirdRoomController : MonoBehaviour
    {
        private DummiesColliderLab _dummiesCollider;
        private MainCAttack _mainCAttack;
        private MainCSounds _mainCSounds;
        private GStopMenu _gStopMenu;
        private MainCTutorialChecker _mainCTutorial;
        private MainCMovement _mainCMovement;

        [SerializeField] private GameObject wave1;
        [SerializeField] private GameObject wave2;
        [SerializeField] private GameObject wave3;
        [SerializeField] private GameObject movableFloor;
        [SerializeField] private GameObject dummieControllerObj;
        [SerializeField] private GameObject goHereObj;
        [SerializeField] private GameObject goHereAbilityAttackObj;
        [SerializeField] private GameObject[] firstReds;
        [SerializeField] private GameObject[] secondReds;
        [SerializeField] private GameObject[] thirdReds;
        [SerializeField] private GameObject lateralTresspasingContainer;
        [SerializeField] private GameObject leftTutorial;
        [SerializeField] private GameObject movableFloorPivot;
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
        private bool _isWave3;
        private bool _isWave2;
        private bool _isFloorMoving;

        private void Awake()
        {
            _dummiesCollider = dummieControllerObj.GetComponent<DummiesColliderLab>();
            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _mainCAttack = _playerObj.GetComponent<MainCAttack>();
            _mainCSounds = _playerObj.GetComponent<MainCSounds>();
            _mainCTutorial = _playerObj.GetComponent<MainCTutorialChecker>();
            _gStopMenu = _playerObj.GetComponent<GStopMenu>();
            _mainCMovement = _playerObj.GetComponent<MainCMovement>();
        }

        private void Start()
        {
            _initalMovableFloatY = movableFloor.transform.localPosition.y;
            _isWave1 = true;
        }

        private void Update()
        {
            if (_floorIsUp && _isWave1 && wave1.transform.childCount == 0 && !_isFloorMoving)
            {
                foreach (var obj in firstReds)
                {
                    obj.SetActive(false);
                }
                goHereObj.SetActive(true);
                leftTutorial.GetComponent<Animator>().SetTrigger(string.Format("close"));
                _isWave1 = false;
                _isWave2 = true;
                var floor = Instantiate(movableFloorPivot, _playerObj.transform.position  - Vector3.right * 1.4f, Quaternion.identity);
                _mainCMovement.StartFollowTrajectory();
            }

            if (_floorIsUp && _isWave2 && wave2.transform.childCount == 0 && !_isFloorMoving)
            {
                foreach (var obj in secondReds)
                {
                    obj.SetActive(false);
                }
                _mainCSounds.PlayTutorialSound(7, "pc");
                leftTutorial.GetComponent<Animator>().SetTrigger(string.Format("close"));
                var floor = Instantiate(movableFloorPivot, _playerObj.transform.position - Vector3.right * 1.4f, Quaternion.identity);
                _mainCMovement.StartFollowTrajectory();
                goHereObj.SetActive(true);
                _isWave2 = false;
                _isWave3 = true;
                _mainCMovement.StartFollowTrajectory();
            }


            if (_floorIsUp && _isWave3 && wave3.transform.childCount == 0 && !_isFloorMoving)
            {
                StartCoroutine(MoveDownToZero());
                _isWave3 = false;
                _mainCAttack.SetHasUnlockedAbilityAttack(true);
                _mainCAttack.ShowWeapon();
                _mainCAttack.EnableCanAttack();
                _mainCTutorial.movedDerImage.GetComponent<Animator>().SetTrigger(string.Format("close"));
                _mainCTutorial.movedIzqImage.GetComponent<Animator>().SetTrigger(string.Format("close"));
                _mainCSounds.PlayTutorialSound(9, "pc");
            }
        }

        private void Freeze()
        {
            GameManagerSingleton.Instance.FreezeTime(true);
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
            _floorIsDown = false;
            seActivaCuandoVaASubir.Invoke();
            _isFloorMoving = true;

            if(_isWave1)
            {
                _mainCSounds.PlayTutorialSound(4, "pc");
            }

            yield return new WaitForSeconds(1f);

            while (true)
            {
                var temp = movableFloor.transform.localPosition;
                if (_isWave3)
                {
                    temp.y += speedOfFloor / 2;
                }
                else
                {
                    temp.y += speedOfFloor;
                }

                movableFloor.transform.localPosition = temp;
                if (_isWave1)
                {
                    if (movableFloor.transform.localPosition.y >= 0)
                    {
                        _mainCTutorial.AttackImage();
                        _mainCAttack.EnableCanAttack();

                        _dummiesCollider.UndoChild(wave1);
                        temp.y = 0;
                        movableFloor.transform.localPosition = temp;
                        seActivaCuandoHaSubido.Invoke();
                        _floorIsUp = true;
                        break;
                    }
                }

                if (_isWave2)
                {
                    if (movableFloor.transform.localPosition.y >= 0f)
                    {
                        _mainCAttack.DisableCanAttack();
                        _mainCAttack.HideWeapon();
                        _mainCSounds.PlayTutorialSound(6, "pc");
                        _mainCTutorial.AttackImage();
                        _dummiesCollider.UndoChild(wave2);
                        temp.y = 0f;
                        movableFloor.transform.localPosition = temp;
                        seActivaCuandoHaSubido.Invoke();
                        _floorIsUp = true;
                        goHereAbilityAttackObj.SetActive(true);
                        break;
                    }
                }

                if (_isWave3)
                {
                    if (movableFloor.transform.localPosition.y >= 3f)
                    {
                        _mainCAttack.SetHasUnlockedAbilityAttack(false);
                        _mainCAttack.DisableCanAttack();
                        _mainCAttack.HideWeapon();
                        _mainCSounds.PlayTutorialSound(8, "pc");
                        _mainCTutorial.movedDerImage.SetActive(true);
                        _mainCTutorial.movedIzqImage.SetActive(true);
                        GameManagerSingleton.Instance.SetHasUnlockedBastetAttack(true);
                        _dummiesCollider.UndoChild(wave3);
                        temp.y = 3f;
                        movableFloor.transform.localPosition = temp;
                        seActivaCuandoHaSubido.Invoke();
                        _floorIsUp = true;
                        lateralTresspasingContainer.SetActive(true);
                        break;
                    }
                }

                yield return new WaitForSeconds(0.01f);
            }

            _isFloorMoving = false;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator MoveDown()
        {
            _floorIsUp = false;
            seActivaCuandoVaABajar.Invoke();
            _isFloorMoving = true;
            while (true)
            {
                var temp = movableFloor.transform.localPosition;
                temp.y -= speedOfFloor;
                movableFloor.transform.localPosition = temp;

                if (movableFloor.transform.localPosition.y <= _initalMovableFloatY)
                {
                    if (_isWave2)
                    {
                        wave2.SetActive(true);
                    }      
                    
                    if (_isWave3)
                    {
                        wave3.SetActive(true);
                    }
                    temp.y = _initalMovableFloatY;
                    movableFloor.transform.localPosition = temp;
                    _floorIsDown = true;
                    seActivaCuandoHaBajado.Invoke();
                    _dummiesCollider.ClearList();
                    wave1.SetActive(true);
                    StartCoroutine(MoveUp());
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }

            _isFloorMoving = false;
        }

        private IEnumerator MoveDownToZero()
        {
            while (true)
            {
                var temp = movableFloor.transform.localPosition;
                temp.y -= speedOfFloor;
                movableFloor.transform.localPosition = temp;

                if (movableFloor.transform.localPosition.y <= 0)
                {
                    foreach (var obj in thirdReds)
                    {
                        obj.SetActive(false);
                    }
                    seActivaCuandoLasOleadasTerminan.Invoke();
                    _mainCAttack.HideWeapon();
                    temp.y = 0f;
                    movableFloor.transform.localPosition = temp;
                    lateralTresspasingContainer.SetActive(false);
                    break;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}