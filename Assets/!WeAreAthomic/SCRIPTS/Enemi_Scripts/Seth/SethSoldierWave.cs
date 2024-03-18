using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Seth
{
    public class SethSoldierWave : MonoBehaviour
    {
        [SerializeField] SethHurtBox sethHurtBox;

        [NonSerialized] public List<GameObject> spawnedSoldiers = new List<GameObject>();

        [SerializeField] List<GameObject> enemiesPrefab;
        [SerializeField] GameObject broom;

        [SerializeField] private Transform spawnPos;

        [SerializeField] public int enemiesToSpawn = 4;

        public void StartSpawning()
        {

            StartCoroutine(SpawnSoldiers());

        }
        private IEnumerator SpawnSoldiers()
        {
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    var soldier = Instantiate(enemiesPrefab[UnityEngine.Random.Range(0, enemiesPrefab.Count)], spawnPos.position, Quaternion.identity);
                    SoldierToFight(soldier);
                    spawnedSoldiers.Add(soldier);
                    yield return new WaitForSeconds(0.5f);
                }

            if (sethHurtBox.WillDieOnNextAttack)
            {
                broom.SetActive(true);
            }

        }

        private void SoldierToFight(GameObject obj)
        {
            obj.GetComponent<EnemyP>().typeOfBehaviour = TypeOfBehaviour.Fighter;
        }
    }
}