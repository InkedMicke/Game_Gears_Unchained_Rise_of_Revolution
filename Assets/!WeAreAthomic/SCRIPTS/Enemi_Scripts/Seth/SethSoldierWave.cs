using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SethSoldierWave : MonoBehaviour
{
    [NonSerialized] public List<GameObject> spawnedSoldiers = new List<GameObject>();

    [SerializeField] private List<GameObject> soldiersWave1 = new List<GameObject>();
    [SerializeField] private List<GameObject> soldiersWave2 = new List<GameObject>();
    [SerializeField] private List<GameObject> soldiersWave3 = new List<GameObject>();

    [SerializeField] private Transform spawnPos;

    [NonSerialized] public int CurrentSoldiersWavesCount;

    public void StartSpawning()
    {

        StartCoroutine(SpawnSoldiers());

    }
    private IEnumerator SpawnSoldiers()
    {
        CurrentSoldiersWavesCount++;
        if (CurrentSoldiersWavesCount == 1)
        {
            foreach (var x in soldiersWave1)
            {
                var soldier = Instantiate(x, spawnPos.position, Quaternion.identity);
                SoldierToFight(soldier);
                spawnedSoldiers.Add(soldier);
                yield return new WaitForSeconds(0.5f);
            }
        }

        else if (CurrentSoldiersWavesCount == 2)
        {
            foreach (var x in soldiersWave2)
            {
                var soldier = Instantiate(x, spawnPos.position, Quaternion.identity);
                SoldierToFight(soldier);
                spawnedSoldiers.Add(soldier);
                yield return new WaitForSeconds(0.5f);
            }
        }

        else if (CurrentSoldiersWavesCount == 3)
        {
            foreach (var x in soldiersWave3)
            {
                var soldier = Instantiate(x, spawnPos.position, Quaternion.identity);
                SoldierToFight(soldier);
                spawnedSoldiers.Add(soldier);
                yield return new WaitForSeconds(0.5f);
            }
        }
        
    }

    private void SoldierToFight(GameObject obj)
    {
        obj.GetComponent<Enemy>().typeOfBehaviour = TypeOfBehaviour.Fighter;
    }
}
