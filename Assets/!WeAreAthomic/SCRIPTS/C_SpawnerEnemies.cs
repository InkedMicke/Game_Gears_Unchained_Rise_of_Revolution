using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SpawnerEnemies : MonoBehaviour
{
 

    [SerializeField] private Transform[] pointToSpawn;
    [SerializeField] private int numberOfWaves;
    private int _maxEnemies = 20;
    
    private int currentWave;
    [SerializeField] private GameObject barrier;

    public List<GameObject> _currentEnemiesObj = new ();
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
        // Inicializar _wavesList antes de agregar elementos
        _wavesList = new List<List<GameObject>>
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
                var soldado = Instantiate(obj, pointToSpawn[randomPos].position, Quaternion.identity);
                soldado.GetComponent<Enemy>().typeOfBehaviour = TypeOfBehaviour.Fighter;
               _currentEnemiesObj.Add(soldado);
            }

            currentWave++;

            while (!CanNextWave())
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(MoveBarrierDown());
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
