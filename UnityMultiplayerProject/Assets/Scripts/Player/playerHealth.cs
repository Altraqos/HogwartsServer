using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    public Text healthText;
    static playerHealth pHealth;
    public static int currentHealth;
    public static int maxHealth = 100;

    static NetworkManager nManager;

    private void OnEnable()
    {
        nManager = NetworkManager.instance;
        pHealth = this;
        currentHealth = maxHealth;
        SetUI();
    }

    public static void deductHealth(int change)
    {
        if (currentHealth != 0)
        {
            currentHealth -= change;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Death();
            }
        }
        SetUI();
    }

    public static void increaseHealth(int change)
    {
        if (currentHealth != maxHealth)
        {
            currentHealth += change;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
        SetUI();
    }

    public static void Death()
    {
        Debug.Log("you died :D");
    }

    public static void SetUI()
    {
        pHealth.healthText.text = currentHealth + " / " + maxHealth;
    }
}
