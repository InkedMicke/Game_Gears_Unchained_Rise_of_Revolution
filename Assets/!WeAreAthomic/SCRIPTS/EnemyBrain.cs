using _WeAreAthomic.SCRIPTS.Enemi_Scripts.Generic;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies = new();

    public void RefreshEnemiesList()
    {
        var enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var x in enemyObjects)
        {
            var currentEnemie = x.GetComponent<Enemy>();
            enemies.Add(currentEnemie);
        }
    }

    public bool CheckIfAnEnemyIsAttacking()
    {
        foreach(var x in enemies)
        {
            if(x.IsAttacking)
            {
                return true;
            }
        }

        return false;
    }
}
