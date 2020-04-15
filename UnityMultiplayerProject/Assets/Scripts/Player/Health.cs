using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    public float updatedHealth;
    public float maxHealth;
    float healthIncreasedPerSecond;

    public TMP_Text
       currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        updatedHealth = 100;
        maxHealth = 100;
        healthIncreasedPerSecond = 1.0f;

        HUD_HealthDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        updatedHealth += healthIncreasedPerSecond * Time.deltaTime; //Regenerate Health

        if (updatedHealth > maxHealth)
        {
            updatedHealth = 100;
        }

        if (updatedHealth < 0)
        {
            updatedHealth = 0;
        }
    }

    public void HUD_HealthDisplay()
    {
        currentHealth.text = "HP: " + (int)updatedHealth + "/100";
    }
}
