using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameManager gameManager;

    Vector3 preMousePos;

    public Transform cameraZoomTransform;
    public Transform cameraTransform;

    public float zoomDistanceMin;
    public float zoomDistanceMax;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Get();
    }

    // Update is called once per frame
    void Update()
    {
        var cursorPos = gameManager.FocusedStageObject.GetCursorWorldPos();
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

            var pos = cameraTransform.localPosition;
            var OA = -pos.z;
            var A_A = Mathf.Clamp(OA - Input.mouseScrollDelta.y, zoomDistanceMin, zoomDistanceMax);
            var OA_ = OA - A_A;
            var AB = cursorPos.Value - transform.position;
            AB.y = 0;
            var A_B_ = AB * (OA_ / OA);

            transform.Translate(A_B_);
            pos.z = -A_A;

            
            //if (!Input.mouseScrollDelta.Equals(Vector2.zero))
            //{Debug.Log(Input.mouseScrollDelta);
            //    transform.Translate((pos - transform.position) * 0.2f * (pos.z+15.0f)/5.0f);
            //}
            
            cameraTransform.localPosition = pos;
        }
        else
        {
            var pos = cameraTransform.localPosition;
            pos.z = Mathf.Clamp(pos.z + Input.mouseScrollDelta.y, -zoomDistanceMax, -zoomDistanceMin);
            cameraTransform.localPosition = pos;
        }
    }
}
