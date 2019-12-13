using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 preMousePos;
    public TileStage controlStage;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var cursorPos = controlStage.GetCursorWorldPos();
        if (cursorPos.HasValue)
        {
            if (Input.GetMouseButtonDown(2))
            {
                preMousePos = cursorPos.Value;
            }
            if (Input.GetMouseButton(2))
            {
                var diff = cursorPos.Value - preMousePos;
                transform.Translate(-diff);
            }
        }
    }
}
