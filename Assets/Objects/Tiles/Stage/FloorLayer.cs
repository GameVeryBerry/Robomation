using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 床レイヤー
public class FloorLayer : MonoBehaviour
{
    private GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameManager.Get();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2) && !EventSystem.current.IsPointerOverGameObject())
        {
            var pos = manager.instruction.GetCursorTilePos();
            if (pos.HasValue)
                manager.instruction.SetTile(pos.Value);
        }
    }
}
