using UnityEngine;
[CreateAssetMenu(fileName = "new HealthManagerSO", menuName = "Logic/Health Manager")]
public class HealthManagerSO : ScriptableObject
{

    public System.Action<float> healthChangeEvent;
    public System.Action OnDeath;

    public bool CanReceiveDamage;

    public float CurrentHealth;
    [SerializeField]
    private float maxHealth = 100;

    private void OnEnable()
    {
        CurrentHealth = maxHealth;
    }

    public void DecreaseHealth(float amount)
    {
        CurrentHealth -= amount;
        healthChangeEvent?.Invoke(CurrentHealth);

        if(IsDeath())
            OnDeath?.Invoke();
    }

    public bool IsDeath()
    {
        if (CurrentHealth <= 0)
            return true;
        else
            return false;
    }
}
