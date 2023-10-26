using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCHealthManager : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    public float currentHealth = 50f;
    public float maxHealth = 100f;

    private void Start()
    {
        SetMaxHealthSlider();
        SetHealthSlider();
    }

    private void Update()
    {
         if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        GameManager.Instance.currentHealth = currentHealth;
        SetHealthSlider();
    }

    public void GetHealth(float health)
    {
        currentHealth += health;
        GameManager.Instance.currentHealth = currentHealth;
        SetHealthSlider();
    }

    public void SetHealthSlider()
    {
        healthSlider.value = currentHealth;
    }

    private void SetMaxHealthSlider()
    {
        healthSlider.maxValue = maxHealth;
    }
}
