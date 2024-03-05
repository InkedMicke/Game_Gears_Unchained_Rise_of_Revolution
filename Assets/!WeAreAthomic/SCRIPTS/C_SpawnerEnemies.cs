using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Enemy;

[System.Serializable]
public class Wave
{
    public List<TypeOfEnemy> enemiesInWave;
}

public class C_SpawnerEnemies : MonoBehaviour
{
    enum TypeOfSpawner
        {
        SpawnerBarrier,
        SpawnerCamera
        }

    

    [SerializeField] private TypeOfSpawner typeOfSpawner;
    [SerializeField] private Transform[] pointToSpawn;
    [SerializeField] private int numberOfWaves;
    private int _maxEnemies = 20;
    
    private int currentWave;
    [SerializeField] private GameObject barrier;
    [SerializeField] private GameObject goHere;

    public List<GameObject> _currentEnemiesObj = new ();
    public List<Wave> _wavesList;

    [SerializeField] private UnityEvent onFinish;
   

    private EnemyP _enemy;

    public void StartSpawning()
    {
        currentWave = 0;
        StartCoroutine(SpawnWaves());

    }
    IEnumerator SpawnWaves()
    {
        while (currentWave < numberOfWaves)
        {

            var random = Random.Range(0, _wavesList.Count); ;
            Wave selectedWave = _wavesList[random];

            foreach (var obj in selectedWave.enemiesInWave)
            {
                var randomPos = Random.Range(0, pointToSpawn.Length);
                var soldado = Instantiate(GameManagerSingleton.Instance.TypeOfEnemyToPrefab(obj), pointToSpawn[randomPos].position, Quaternion.identity);
                soldado.GetComponent<EnemyP>().typeOfBehaviour = TypeOfBehaviour.Fighter;
               _currentEnemiesObj.Add(soldado);
            }

            currentWave++;

            if (typeOfSpawner == TypeOfSpawner.SpawnerBarrier)
            {
                while (!CanNextWave())
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();

                //StartCoroutine(MoveBarrierDown());
                //onFinish.Invoke();
            }

            yield return new WaitForEndOfFrame();
        }

        
    }

    private IEnumerator MoveBarrierDown()
    {
        var startPos = barrier.transform.position;
        while (barrier.transform.position.y < startPos.y - 10f)
        {
            var actualPos = new Vector3(barrier.transform.position.x, barrier.transform.position.y - 0.005f, barrier.transform.position.z);
            barrier.transform.position = actualPos;

            yield return new WaitForEndOfFrame();
        }

        Destroy(barrier.gameObject);
        goHere.SetActive(true);
    }

    private bool CanNextWave()
    {
        for (int i = _currentEnemiesObj.Count - 1; i >= 0; i--)
        {
            var soldado = _currentEnemiesObj[i];
            var hurtbox = soldado.transform.GetChild(0).GetComponent<SoldierHurtBox>();
            if (hurtbox.IsDeath)
            {
                _currentEnemiesObj.RemoveAt(i);
            }
        }
        if (_currentEnemiesObj.Count == 0)
        {
            return true;
        }

        return false;
    }
}
