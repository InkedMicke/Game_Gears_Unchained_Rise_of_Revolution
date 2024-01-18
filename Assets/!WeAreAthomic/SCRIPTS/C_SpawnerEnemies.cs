using _WeAreAthomic.SCRIPTS.Enemi_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SpawnerEnemies : MonoBehaviour
{
 

    [SerializeField] private Transform[] pointToSpawn;
    private int _maxEnemies = 20;
    
    private int currentWave;
    private List<GameObject>  _currentEnemiesObj;
    [SerializeField]private List<GameObject>  _wave1;
    [SerializeField] private List<GameObject> _wave2;
    [SerializeField] private List<GameObject> _wave3;
    [SerializeField] private List<GameObject> _wave4;
    [SerializeField] private List<GameObject> _wave5;
    [SerializeField] private List<GameObject> _wave6;
    private List<List<GameObject>> _wavesList;

    private Enemy _enemy;
    private void Start()
    {
        //2 verdes / 1 naranja / 1 rojo
        // 1 verde / 2 naranjas/ 1 rojo
        // 3 rojos / 1 verde
        // 4 verdes
        //2 naranjas /1 verde
        //2 rojos / 1 naranja

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

                var soldado = Instantiate(wave, pointToSpawn[randomPos].position, Quaternion.identity);
                soldado.GetComponent<Enemy>()._typeOfBehaviour = TypeOfBehaviour.Fighter;
               _currentEnemiesObj.Add(wave);
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
        foreach (var hurtBox in _currentEnemiesObj)
        {
            if (hurtBox.GetComponentInChildren<SoldierHurtBox>().IsDeath)
            {
                _currentEnemiesObj.Remove(hurtBox);
            }

        }
        if(_currentEnemiesObj.Count == 0)
        {
            return true;
        }

        return false;
    }
}
