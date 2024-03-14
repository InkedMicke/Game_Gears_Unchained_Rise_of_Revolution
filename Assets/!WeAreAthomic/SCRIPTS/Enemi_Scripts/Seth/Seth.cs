using Player;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using Enemy;
using Generics;

namespace Seth
{
    public class Seth : MonoBehaviour
    {
        private SethSoldierWave _sethWave;
        private SethEyeAttack _sethEyeAttack;
        private GTrajectory _gTrajectory;
        private MainCMovement _mainCMove;
        SethAnimator m_sethAnim;
        SethEye m_sethEye;
        [SerializeField] private SethHurtBox sethHurtBox;

        public System.Action OnPushBack;
        public System.Action OnEnemiesDead;

        [SerializeField] HealthManagerSO healthManagerSO;

        [SerializeField] GameObject eye;
        private GameObject _playerObj;

        [SerializeField] private Transform barrier;

        [SerializeField] private float receivedDamageForPushPlayerBack = 250;

        private Vector3 barrierInitalPos;

        bool m_canPushBack;

        private int WaveCount;

        private void Awake()
        {
            _sethWave = GetComponent<SethSoldierWave>();
            _gTrajectory = GetComponent<GTrajectory>();
            _sethEyeAttack = GetComponent<SethEyeAttack>();
            m_sethEye = eye.GetComponent<SethEye>();

            _playerObj = GameObject.FindGameObjectWithTag("Player");
            _gTrajectory.origin = _playerObj.transform;
            _mainCMove = _playerObj.GetComponent<MainCMovement>();

            sethHurtBox.OnAcumulativeEvent += () => m_canPushBack = true;
        }

        private void Start()
        {
            barrierInitalPos = barrier.position;
            StartWaves();
        }

        public void StartWaves()
        {
            StartCoroutine(SpawnEnemies());
            barrier.transform.DOMoveY(barrierInitalPos.y + 5f, .5f).SetEase(Ease.Linear);
        }

        private IEnumerator SpawnEnemies()
        {
            _sethWave.StartSpawning();

            while (!IsCurrentWaveDead())
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(2f);
            barrier.transform.DOMoveY(barrierInitalPos.y, .5f).SetEase(Ease.Linear);
            OnEnemiesDead?.Invoke();
            StartCoroutine(CheckSethHealthForPush());
        }

        private IEnumerator CheckSethHealthForPush()
        {
            while (!m_canPushBack)
            {
                yield return new WaitForEndOfFrame();
            }
            m_canPushBack = false;
            OnPushBack?.Invoke();
            barrier.transform.DOMoveY(barrierInitalPos.y + 5f, .5f).SetEase(Ease.Linear);
            _mainCMove.Trajectory = _gTrajectory;
            _playerObj.GetComponent<CharacterController>().enabled = false;
            _mainCMove.SetFollowTrajectory(true);
            Utilities.Invoke(this, () =>
            {
                eye.SetActive(true);
                m_sethEye.StartAttack();
                StartCoroutine(CheckForEndEyeAttack());
            }, 2f);
        }

        public IEnumerator CheckForEndEyeAttack()
        {
            while (m_sethEye.IsEyeAttacking)
            {
                yield return new WaitForEndOfFrame();
            }
            WaveCount++;
            
            StartCoroutine(SpawnEnemies());
        }

        private bool IsCurrentWaveDead()
        {
            for (int i = _sethWave.spawnedSoldiers.Count - 1; i >= 0; i--)
            {
                var soldado = _sethWave.spawnedSoldiers[i];
                var hurtbox = soldado.transform.GetChild(0).GetComponent<SoldierHurtBox>();

                if (hurtbox.IsDeath)
                {
                    _sethWave.spawnedSoldiers.RemoveAt(i);
                }
            }
            if (_sethWave.spawnedSoldiers.Count == 0)
            {
                return true;
            }

            return false;


        }


    }
}