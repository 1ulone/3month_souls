using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI info;
    [SerializeField] private Image bar;

    public void UpdateHealthUI(int health, int maxHealth)
    {
        bar.fillAmount = (float)health / (float)maxHealth;
        info.text = health + "/" + maxHealth;
    }
}
