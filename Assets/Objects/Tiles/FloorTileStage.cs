using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorTileStage : MonoBehaviour
{
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
        if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            var pos = stage.GetCursorTilePos();
            if (pos.HasValue)
                stage.SetTile(pos.Value);
        }
    }
}
