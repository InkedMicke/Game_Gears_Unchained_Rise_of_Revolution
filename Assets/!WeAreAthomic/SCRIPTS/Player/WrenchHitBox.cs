using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WrenchHitBox : MonoBehaviour
{
    private MainCAttack _mainCAttack;

    private readonly Vector3 _boxSize = new Vector3(.2f, .1f, .5f);

    [SerializeField] private LayerMask enemyHurtBox;

    public List<Collider> colliderList = new List<Collider>();

    private void Awake()
    {
        _mainCAttack = FindObjectOfType<MainCAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyHurtBox.value & (1 << other.gameObject.layer)) != 0)
        {
            if (!colliderList.Contains(other))
            {
                other.GetComponent<DummieHurtBox>().TakeDamage(20);
                SpeedDownTime();
                Invoke(nameof(SpeedUpTime), .005f);
                colliderList.Add(other);
            }
        }
    }

    public void ClearList()
    {
        colliderList.Clear();
    }

    private void SpeedUpTime()
    {
        Time.timeScale = 1f;
    }

    private void SpeedDownTime()
    {
        Time.timeScale = 0.1f;
    }
}