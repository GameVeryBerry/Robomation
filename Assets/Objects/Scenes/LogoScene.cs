using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        //シーン遷移
        if (Input.anyKey)//何かのキーが押されたら
        {
            SceneManager.LoadScene("SampleScene");//プレイシーンへ移行
        }
    }
}
