using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System.Collections;
using UnityEngine;

public class Seth : MonoBehaviour
{
    private SethSoldierWave _sethWave;
    private SethLaser _sethLaser;

    [SerializeField] private Transform barrier;

    private Vector3 barrierInitalPos;

    protected int WaveCount;

    private void Awake()
    {
        _sethWave = GetComponent<SethSoldierWave>();
        _sethLaser = GetComponent<SethLaser>();
    }

    private void Start()
    {
        barrier.position = barrierInitalPos;
    }

    public void StartWaves()
    {
        StartCoroutine(WaveController());
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

            if(WaveCount == 2)
            {
                //Ataque del ojo 
            }

            yield return new WaitForSeconds(1);
        }
    }

    private bool IsCurrentWaveDead()
    {
        foreach(var x in _sethWave.spawnedSoldiers)
        {
            if(x.GetComponentInChildren<SoldierHurtBox>().IsDeath)
            {
                _sethWave.spawnedSoldiers.Remove(x);
            }
        }

        return true;
    }

    private IEnumerator MoveBarrier(string UpOrDown)
    {
        if(UpOrDown == "Up")
        {
            while(barrier.position.y < barrierInitalPos.y + 6f)
            {
                barrier.position += barrier.up * 4f;
            }
        }
        else if(UpOrDown == "Down")
        {

        }

        yield return null;
    }

    
}
