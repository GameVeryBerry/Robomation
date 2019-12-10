using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InstructionTileStage : MonoBehaviour
{
    public FloorTileStage floorStage;
    [HideInInspector]
    public TileStage stage;

    // Start is called before the first frame update
    void Start()
    {
        stage = GetComponent<TileStage>();

        SaveLoad.CreateGameDirectories();

        stage.LoadMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var pos = stage.GetCursorTilePos();
            if (pos.HasValue)
            {
                if (floorStage.stage.map.IsValid(pos.Value))
                {
                    if (floorStage.stage.map.GetIndex(pos.Value) != 0xff)
                        stage.SetTile(pos.Value);
                }
            }
        }
    }
}
