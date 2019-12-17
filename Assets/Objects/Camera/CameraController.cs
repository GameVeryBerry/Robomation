using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 preMousePos;
    public TileStage controlStage;

    public Transform cameraZoomTransform;
    public Transform cameraTransform;

    public float zoomDistanceMin;
    public float zoomDistanceMax;

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
            if (Input.GetMouseButtonDown(1))
            {
                preMousePos = cursorPos.Value;
            }
            if (Input.GetMouseButton(1))
            {
                var diff = cursorPos.Value - preMousePos;
                transform.Translate(-diff);
            }
        }

        var pos = cameraTransform.localPosition;
        pos.z = Mathf.Clamp(pos.z + Input.mouseScrollDelta.y, -zoomDistanceMax, -zoomDistanceMin);
        cameraTransform.localPosition = pos;
    }
}
