using System.Collections;
using UnityEngine;

public class Seth : MonoBehaviour
{
    private SethSoldierWave _sethWave;
    private SethLaser _sethLaser;

    private int _waveCount;

    private void Awake()
    {
        _sethWave = GetComponent<SethSoldierWave>();
        _sethLaser = GetComponent<SethLaser>();
    }

    public void StartWaves()
    {

    }

    public IEnumerator WaveController()
    {
        while(true)
        {
            _waveCount++;
            if (_waveCount == 1)
            {
                //Spawnear soldados
                _sethWave.StartSpawning();
            }

            if( _waveCount == 2)
            {
                //Ataque del ojo 
            }
        }
    }

    
}
