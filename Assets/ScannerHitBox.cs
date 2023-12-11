using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerHitBox : MonoBehaviour
{
    public List<GameObject> enemies;
    public void ApplyDamage(PlayerDamageData dmgData)
    {
        foreach (var enemy in enemies) 
        {
            if(enemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(GameManagerSingleton.Instance.GetDamage(dmgData, enemy));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        enemies.Add(other.gameObject);
    }
}
