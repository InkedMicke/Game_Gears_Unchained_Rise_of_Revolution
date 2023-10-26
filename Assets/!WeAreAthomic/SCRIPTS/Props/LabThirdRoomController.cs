using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _WeAreAthomic.SCRIPTS.Props
{
    public class LabThirdRoomController : MonoBehaviour
    {
        private DummiesColliderLab _dummiesCollider;

        [SerializeField] private GameObject wave1;
        [SerializeField] private GameObject wave2;
        [SerializeField] private GameObject movableFloor;
        [SerializeField] private GameObject dummieControllerObj;
        [SerializeField] private GameObject goHereObj;
        [SerializeField] private float speedOfFloor = 0.1f;

        [SerializeField] private UnityEvent seActivaCuandoVaASubir;
        [SerializeField] private UnityEvent seActivaCuandoVaABajar;
        [SerializeField] private UnityEvent seActivaCuandoHaSubido;
        [SerializeField] private UnityEvent seActivaCuandoHaBajado;

        private bool _floorIsUp;
        private bool _floorIsDown;
        private bool _isFloorMoving;

        private void Awake()
        {
            _dummiesCollider = dummieControllerObj.GetComponent<DummiesColliderLab>();
        }

        private void Update()
        {
            if (_floorIsUp && wave1.transform.childCount == 0 && !_isFloorMoving)
            {
                goHereObj.SetActive(true);
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
            while (enable)
            {
                var temp = movableFloor.transform.localPosition;
                temp.y += speedOfFloor;
                movableFloor.transform.localPosition = temp;
                if (movableFloor.transform.localPosition.y >= -0.918f)
                {
                    _dummiesCollider.UndoChild(wave1);
                    temp.y = -0.9186499f;
                    movableFloor.transform.localPosition = temp;
                    enable = false;
                    seActivaCuandoHaSubido.Invoke();
                    _floorIsUp = true;
                }

                yield return new WaitForSeconds(0.01f);
            }

            _isFloorMoving = false;
        }

        private IEnumerator MoveDown()
        {
            var enable = true;
            _floorIsUp = false;
            seActivaCuandoVaABajar.Invoke();
            _isFloorMoving = true;
            while (enable)
            {
                var temp = movableFloor.transform.localPosition;
                temp.y -= speedOfFloor;
                movableFloor.transform.localPosition = temp;

                if (movableFloor.transform.localPosition.y <= -9)
                {
                    wave2.SetActive(true);
                    temp.y = -9;
                    movableFloor.transform.localPosition = temp;
                    enable = false;
                    _floorIsDown = true;
                    seActivaCuandoHaBajado.Invoke();
                    Invoke(nameof(InvokeMoveUp), 0f);
                }

                yield return new WaitForSeconds(0.01f);
            }

            _isFloorMoving = false;
        }
    }
}