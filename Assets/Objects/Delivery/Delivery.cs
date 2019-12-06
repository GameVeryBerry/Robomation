using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Delivery : MonoBehaviour, ITile
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTileAction(RobotController robot)
    {
        Money MoneyClass = FindObjectOfType<Money>();
        MoneyClass.AddMoney(500);
    }
}
