using UnityEngine;
[CreateAssetMenu(fileName = "new HealthManagerSO", menuName = "Logic/Health Manager")]
public class HealthManagerSO : ScriptableObject
{

    public System.Action<float> healthChangeEvent;
    public System.Action OnDeath;

    public bool CanReceiveDamage;

    public float CurrentHealth;
    public float MaxHealth = 100;

    private void OnEnable()
    {
        healthChangeEvent?.Invoke(CurrentHealth);
    }

    public void DecreaseHealth(float amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        healthChangeEvent?.Invoke(CurrentHealth);
        if (IsDeath())
            OnDeath?.Invoke();
    }

    public void GetHealth(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        healthChangeEvent?.Invoke(CurrentHealth);
    }

    public bool IsDeath()
    {
        if (CurrentHealth <= 0)
            return true;
        else
            return false;
    }
}
