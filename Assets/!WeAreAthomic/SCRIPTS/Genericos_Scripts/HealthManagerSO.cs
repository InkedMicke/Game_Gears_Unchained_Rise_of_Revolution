using UnityEngine;
[CreateAssetMenu(fileName = "new HealthManagerSO", menuName = "Logic/Health Manager")]
public class HealthManagerSO : ScriptableObject
{

    public System.Action<float> healthChangeEvent;
    public System.Action OnAcumulativeEvent;
    public System.Action OnDeath;

    public bool CanReceiveDamage;

    public float CurrentHealth;
    [SerializeField]
    private float maxHealth = 100;
    [SerializeField]
    float acumulativeHealth = 250;
    float m_currentAcumulativeHealth;

    private void OnEnable()
    {
        CurrentHealth = maxHealth;
    }

    public void DecreaseHealth(int amount)
    {
        CurrentHealth -= amount;
        m_currentAcumulativeHealth += amount;
        if(m_currentAcumulativeHealth >= acumulativeHealth)
        {
            OnAcumulativeEvent?.Invoke();
            m_currentAcumulativeHealth = 0;
        }

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
