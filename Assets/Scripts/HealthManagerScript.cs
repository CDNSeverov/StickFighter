using UnityEngine;
using UnityEngine.UI;

public class HealthManagerScript : MonoBehaviour
{
    [SerializeField] public Image healthBar;
    public float healthAmount = 100f;

    public void TakeDamage(float damage) {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void ResetHealthBar() {
        healthBar.fillAmount = 1;
        healthAmount = 100f;
    }

    public float GetHealth() {
        return healthAmount;
    }
}
