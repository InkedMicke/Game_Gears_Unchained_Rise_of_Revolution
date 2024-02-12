using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField]private List<TypeOfEnemy> _wave1;
    [SerializeField] private List<TypeOfEnemy> _wave2;
    [SerializeField] private List<TypeOfEnemy> _wave3;
    [SerializeField] private List<TypeOfEnemy> _wave4;
    [SerializeField] private List<TypeOfEnemy> _wave5;
    [SerializeField] private List<TypeOfEnemy> _wave6;

    private List<List<TypeOfEnemy>> _wavesList;

    [SerializeField] private UnityEvent onFinish;
   

    private Enemy _enemy;
    private void Start()
    {
        // Inicializar _wavesList antes de agregar elementos
        _wavesList = new List<List<TypeOfEnemy>>
        {
            //2 verdes / 1 naranja / 1 rojo
            // 1 verde / 2 naranjas/ 1 rojo
            // 3 rojos / 1 verde
            // 4 verdes
            //2 naranjas /1 verde
            //2 rojos / 1 naranja

            _wave1,
            _wave2,
            _wave3,
            _wave4,
            _wave5,
            _wave6
        };


    }

    

    public void StartSpawning()
    {
        currentWave = 0;
        StartCoroutine(SpawnWaves());

    }
    IEnumerator SpawnWaves()
    {
        while (currentWave < numberOfWaves)
        {

            var random = Random.Range(0, _wavesList.Count);

            foreach (var obj in _wavesList[random])
            {
                var randomPos = Random.Range(0, pointToSpawn.Length);
                var soldado = Instantiate(GameManagerSingleton.Instance.TypeOfEnemyToPrefab(obj), pointToSpawn[randomPos].position, Quaternion.identity);
                soldado.GetComponent<Enemy>().typeOfBehaviour = TypeOfBehaviour.Fighter;
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

                StartCoroutine(MoveBarrierDown());
                onFinish.Invoke();
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

            if (soldado.GetComponentInChildren<SoldierHurtBox>().IsDeath)
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
