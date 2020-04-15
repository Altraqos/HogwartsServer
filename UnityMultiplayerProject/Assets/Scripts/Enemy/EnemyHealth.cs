using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int ID = 0;
    public float health = 100;
    public NetworkManager nManager;

    //Place enemy health script here

    public void OnEnable()
    {
        nManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        ID = Random.Range(0, 100000);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (nManager != null)
        {
            nManager.ForwardEnemyState(
                nManager.playerID + "#" +
                health);
        }

        if (health <= 0f)
        {
            //place enmey death effect here
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
