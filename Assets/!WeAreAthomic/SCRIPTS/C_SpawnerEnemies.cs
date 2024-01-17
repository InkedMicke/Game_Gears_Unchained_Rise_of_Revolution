using _WeAreAthomic.SCRIPTS.Enemi_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SpawnerEnemies : MonoBehaviour
{
    [SerializeField] private GameObject soldadoVerde;
    [SerializeField] private GameObject soldadoNaranja;
    [SerializeField] private GameObject soldadoRojo;

    [SerializeField] private Transform[] pointToSpawn;
    private int _maxEnemies = 20;
    private int _currentEnemies;
    private int currentWave;
    private List<GameObject>  _currentEnemiesObj;
    private List<GameObject>  _wave1;
    private List<GameObject> _wave2;
    private List<GameObject> _wave3;
    private List<GameObject> _wave4;
    private List<GameObject> _wave5;
    private List<GameObject> _wave6;
    private List<List<GameObject>> _wavesList;

    private Enemy _enemy;
    private void Awake()
    {
        //2 verdes / 1 naranja / 1 rojo
        // 1 verde / 2 naranjas/ 1 rojo
        // 3 rojos / 1 verde
        // 4 verdes
        //2 naranjas /1 verde
        //2 rojos / 1 naranja
        _wave1.Add(soldadoVerde);
        _wave1.Add(soldadoVerde);
        _wave1.Add(soldadoNaranja);
        _wave1.Add(soldadoRojo);

        _wave2.Add(soldadoRojo);
        _wave2.Add(soldadoVerde);
        _wave2.Add(soldadoNaranja);
        _wave2.Add(soldadoNaranja);

        _wave3.Add(soldadoRojo);
        _wave3.Add(soldadoRojo);
        _wave3.Add(soldadoRojo);
        _wave3.Add(soldadoVerde);

        _wave4.Add(soldadoVerde);
        _wave4.Add(soldadoVerde);
        _wave4.Add(soldadoVerde);
        _wave4.Add(soldadoVerde);
        
        _wave5.Add(soldadoNaranja);
        _wave5.Add(soldadoNaranja);
        _wave5.Add(soldadoVerde);

        _wave6.Add(soldadoRojo);
        _wave6.Add(soldadoRojo);
        _wave6.Add(soldadoNaranja);

        _wavesList.Add(_wave1);
        _wavesList.Add(_wave2);
        _wavesList.Add(_wave3);
        _wavesList.Add(_wave4);
        _wavesList.Add(_wave5);
        _wavesList.Add(_wave6);


    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnWaves());

    }
    IEnumerator SpawnWaves()
    {
        while (currentWave < 6)
        {

            var random = Random.Range(0, _wavesList.Count);

            foreach (var wave in _wavesList[random])
            {
                var randomPos = Random.Range(0, pointToSpawn.Length);

                Instantiate(wave, pointToSpawn[randomPos].position, Quaternion.identity);
                _currentEnemies++;
            }

            currentWave++;

            while (!CanNextWave())
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private bool CanNextWave()
    {
        return true;
    }
}
