using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using _WeAreAthomic.SCRIPTS.Player_Scripts;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Seth : MonoBehaviour
{
    private SethSoldierWave _sethWave;
    private SethLaser _sethLaser;
    private GTrajectory _gTrajectory;
    private MainCMovement _mainCMove;

    private GameObject _playerObj;

    [SerializeField] private Transform barrier;

    [SerializeField] private float timeToHitSeth;

    private Vector3 barrierInitalPos;

    protected int WaveCount;

    private void Awake()
    {
        _sethWave = GetComponent<SethSoldierWave>();
        _sethLaser = GetComponent<SethLaser>();
        _gTrajectory = GetComponent<GTrajectory>();

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
        barrier.transform.DOMoveY(barrierInitalPos.y + 5f, 1f).SetEase(Ease.Linear);
    }

    public IEnumerator WaveController()
    {
        while(true)
        {
            WaveCount++;
            if (WaveCount == 1)
            {
                //Spawnear soldados
                _sethWave.StartSpawning();
            }

            while(!IsCurrentWaveDead())
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(1f);

            barrier.transform.DOMoveY(barrierInitalPos.y, 1f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(timeToHitSeth);

            _mainCMove.Trajectory = _gTrajectory;
            _playerObj.GetComponent<CharacterController>().enabled = false;
            _mainCMove.SetFollowTrajectory(true);

            if (WaveCount == 2)
            {
                //Ataque del ojo 
            }

            yield return new WaitForSeconds(1);
        }
    }

    private bool IsCurrentWaveDead()
    {
        for (int i = _sethWave.spawnedSoldiers.Count - 1; i >= 0; i--)
        {
            var soldado = _sethWave.spawnedSoldiers[i];

            if (soldado.GetComponentInChildren<SoldierHurtBox>().IsDeath)
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
