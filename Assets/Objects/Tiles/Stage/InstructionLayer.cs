using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 指示タイル層
public class InstructionLayer : MonoBehaviour
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
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var pos = manager.instruction.GetCursorTilePos();
            if (pos.HasValue)
            {
                if (manager.floor.map.IsValid(pos.Value))
                {
                    if (manager.floor.map.GetTile(pos.Value) != 0xff)
                        manager.instruction.SetTile(pos.Value);
                }
            }
        }
    }
}
