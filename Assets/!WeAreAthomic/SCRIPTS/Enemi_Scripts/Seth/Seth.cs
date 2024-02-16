using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Seth : MonoBehaviour
{
    private SethSoldierWave _sethWave;
    private SethEyeAttack _sethEyeAttack;
    private GTrajectory _gTrajectory;
    private MainCMovement _mainCMove;
    [SerializeField] private SethHurtBox sethHurtBox;

    private GameObject _playerObj;

    [SerializeField] private Transform barrier;

    [SerializeField] private float receivedDamageForPushPlayerBack = 250;

    private Vector3 barrierInitalPos;

    protected int WaveCount;

    private void Awake()
    {
        _sethWave = GetComponent<SethSoldierWave>();
        _gTrajectory = GetComponent<GTrajectory>();
        _sethEyeAttack = GetComponent<SethEyeAttack>();

        _playerObj = GameObject.FindGameObjectWithTag("Player");
        _gTrajectory.origin = _playerObj.transform;
        _mainCMove = _playerObj.GetComponent<MainCMovement>();
    }

    private void Start()
    {
        barrierInitalPos = barrier.position;
    }

    public void StartWaves()
    {
        StartCoroutine(WaveController());
        barrier.transform.DOMoveY(barrierInitalPos.y + 5f, .5f).SetEase(Ease.Linear);
    }

/*    private IEnumerator SpawnEnemies()
    {

    }*/

    public IEnumerator WaveController()
    {
        while (true)
        {
            WaveCount++;
            if (WaveCount == 1)
            {
                //Spawnear soldados
                _sethWave.StartSpawning();
            }

            while (!IsCurrentWaveDead())
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(1f);

            barrier.transform.DOMoveY(barrierInitalPos.y, .5f).SetEase(Ease.Linear);

            while (sethHurtBox.AcumulativeTakenHealth < receivedDamageForPushPlayerBack)
            {
                yield return new WaitForEndOfFrame();
            }

            sethHurtBox.AcumulativeTakenHealth = 0;

            _mainCMove.Trajectory = _gTrajectory;
            _playerObj.GetComponent<CharacterController>().enabled = false;
            _mainCMove.SetFollowTrajectory(true);

            if (WaveCount == 2)
            {
                //Ataque del ojo 
                _sethEyeAttack.StarEyeAttacking();
            }
        }

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
