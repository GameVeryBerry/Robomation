using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricController : MonoBehaviour
{
    // モードステートインターフェース
    public interface IMode
    {
        // 毎ロボットチックの処理 (今は1秒ごと)
        void OnTick(RobotController robot);
    }
    // 発電モード
    public class GenerateMode : IMode
    {
        void IMode.OnTick(RobotController robot)
        {
            throw new NotImplementedException();
        }

        void OnTick(RobotController robot)
        {
            // ～～～～～
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
