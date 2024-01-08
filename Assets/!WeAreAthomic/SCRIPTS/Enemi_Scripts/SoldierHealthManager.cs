using _WeAreAthomic.SCRIPTS.Interfaces_Scripts;
using UnityEngine;
using UnityEngine.UI;


namespace _WeAreAthomic.SCRIPTS.Enemi_Scripts
{
    public class SoldierHealthManager : MonoBehaviour, IDamageable
    {
        private GreenSoliderMovement _greenMove;
        private GreenSoliderAttack _greenAttack;
        private SoldierAgent _soldierAgent;
        private SoldierHurtBox _soldierHurtbox;

       [SerializeField] private C_DisolveEnemi _disolveEnemi;

        [SerializeField] private Slider healthSlider;

        [SerializeField] private GameObject mesh;
        [SerializeField] private GameObject soldierWithoutBones;
        [SerializeField] private GameObject healthSliderObj;
        [SerializeField] private GameObject botonSoldier;

        public float maxHealth = 100f;
        public float currentHealth = 0f;

        private void Awake()
        {
            _greenMove = GetComponentInParent<GreenSoliderMovement>();
            _greenAttack = GetComponentInParent<GreenSoliderAttack>();
            _soldierAgent = GetComponentInParent<SoldierAgent>();
            _soldierHurtbox = GetComponent<SoldierHurtBox>();

            currentHealth = maxHealth;
            SetMaxhealthSlider(maxHealth);
            SetHealthSlider(currentHealth);
        }

        public void Damage(float damage)
        {
            currentHealth -= damage;
            SetHealthSlider(currentHealth);
            CheckForDeath();
            if (!_soldierHurtbox.IsDeath)
            {
                _greenMove.SetChasePlayer(true);
            }
        }

        private void CheckForDeath()
        {
            if (currentHealth <= 0)
            {
                if (!_soldierHurtbox.IsDeath)
                {
                    _soldierHurtbox.SetDeath(true);
                    Death();
                }
            }
        }

        private void Death()
        {
            mesh.SetActive(false);
            _soldierAgent.StopTotallyAgent();
            healthSliderObj.SetActive(false);
            soldierWithoutBones.SetActive(true);
            _greenMove.DisableMovement();
            botonSoldier.SetActive(false);
            _disolveEnemi.StartDisolving();
            _greenAttack.DestroyDecal();
        }

        public void SetMaxhealthSlider(float maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }

        public void SetHealthSlider(float health)
        {
            healthSlider.value = health;
        }
    }
}
