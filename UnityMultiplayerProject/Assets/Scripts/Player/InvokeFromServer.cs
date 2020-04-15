using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeFromServer : MonoBehaviour
{
    public delegate void changeInt(int change);

    public void DeductHealth(int change)
    {
        changeInt handler = playerHealth.deductHealth;
        handler(change);
    }

    public void IncreaseHealth(int change)
    {
        changeInt handler = playerHealth.increaseHealth;
        handler(change);
    }
}
