using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerHitBox : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    private GameObject[] arrowDisplayer;

    private Transform cameraTr;

    public List<GameObject> enemies;

    private void Start()
    {
        cameraTr = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    public void ApplyDamage(PlayerDamageData dmgData)
    {
        foreach (var enemy in enemies) 
        {
            if(enemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.GetDamage(GameManagerSingleton.Instance.GetPlayerDamage(dmgData, enemy));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        enemies.Add(other.gameObject);
/*        var transform1 = other.transform;
        var position = transform1.position;
        var locUp = new Vector3(position.x, position.y + 1.5f, position.z);
        Instantiate(arrow, locUp, Quaternion.identity);*/
    }
}
