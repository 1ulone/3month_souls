using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instances;

    [SerializeField] private TextMeshProUGUI info;
    [SerializeField] private Image healthUI;
    [SerializeField] private TextMeshProUGUI expUI;

    private void Awake()
    {
        instances = this;
    }

    public void UpdateHealthUI(int health, int maxHealth)
    {
        healthUI.fillAmount = (float)health / (float)maxHealth;
        info.text = health + "/" + maxHealth;
    }

    public void UpdateExpUI(int exp)
    {
        expUI.text = "EXP : " + exp.ToString();
    }
}
