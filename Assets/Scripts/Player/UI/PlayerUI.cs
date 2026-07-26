using UnityEngine;
using UnityEngine.UI;
using TMPro;
using wine.util;

namespace wine.player.ui
{
    public class PlayerUI : MonoBehaviour
    {
        public static PlayerUI instances;

        [SerializeField] private Image healthUI;
        [SerializeField] private Image vesselUI;
        [SerializeField] private TextMeshProUGUI expUI;
        [SerializeField] private PlayerController controller;

        private PlayerStats stats;
        private int health;
        private int maxHealth;
        private float vintake;

        private void Awake()
            => instances = this;

        private void Start()
        {
            stats = PlayerStats.instances;
            maxHealth = stats.health;
            health = maxHealth;
            vintake = 0;

            UpdateHealthUI(health, maxHealth);
        }

        public void HealthOnDamaged(int damage)
        {
            int tdamage = stats.defense - damage > 0 ? 0 : stats.defense - damage;
            health -= Mathf.Abs(tdamage);
            Debug.Log(tdamage);
            controller.canBeHurt = false;

            if (health <= 0)
                wine.util.ui.GameOverUI.instances.StartPanel();

            UpdateHealthUI(health, maxHealth);
        }

        public void UpdateHealthUI(int health, int maxHealth)
        {
            healthUI.fillAmount = (float)health / (float)maxHealth;
        }

        public void UpdateVesselUI(float vessel, int maxVessel)
        {
            vesselUI.fillAmount = (float)vessel / (float)maxVessel;
        }

        public void UpdateExpUI(int exp)
        {
            expUI.text = "EXP : " + exp.ToString();
        }

        private void Update()
        {
            controller.onTransition = wine.util.ui.FadeTransitionUI.isTransitioning;

            if (InputController.instances.GetInput("inventory"))
                InventoryUI.instances.ToggleInventory();


            if (InputController.instances.GetInput("heal", true))
                vintake = 0;

            if (InputController.instances.GetInput("heal") && PlayerStats.instances.currentVessel > 1.0f && health < stats.health)
            {
                if (vintake >= 1.0f)
                {
                    vintake = 0.4f;
                    // PlayerStats.instances.ControlVessel(-1);

                    health += 1;
                    PlayerUI.instances.UpdateHealthUI(health, stats.health);
                } else { vintake += 1 - Mathf.Sqrt(1-Mathf.Pow(Time.deltaTime*17.5f, 2)); } 
            }

        }
    }
}
