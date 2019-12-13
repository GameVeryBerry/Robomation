using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileStage : MonoBehaviour
{
    public float mapHeight;
    public Prefabs mapTiles;
    public string mapName = "sample";
    public StageData map;
    GameObject[] mapObjects;

    public byte selectedTile;
    public int SelectedTile
    {
        set { selectedTile = (byte)value; }
        get { return selectedTile; }
    }

    public void InitMap()
    {
        map = new StageData();
        map.data.width = 64;
        map.data.height = 64;
        map.data.data = Enumerable.Repeat<byte>(0xff, map.data.width * map.data.height).ToArray();
        SaveLoad.SaveBinary(map, mapName, SaveType.Map, (fs, data) => data.SaveMap(fs), overwrite: true);
        LoadMap();
    }

    public void SaveMap()
    {
        SaveLoad.SaveBinary(map, mapName, SaveType.Map, (fs, data) => data.SaveMap(fs), overwrite: true);
    }

    public void LoadMap()
    {
        map = SaveLoad.LoadBinary(mapName, SaveType.Map, fs =>
        {
            var data = new StageData();
            if (data.LoadMap(fs))
                return data;
            return null;
        });
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            mapObjects = new GameObject[0];
        }
        if (map != null)
        {
            mapObjects = new GameObject[map.data.width * map.data.height];
            for (int iy = 0; iy < map.data.height; iy++)
                for (int ix = 0; ix < map.data.width; ix++)
                    UpdateTile(new Vector2Int(ix, iy));
        }
    }

    public void SetTile(Vector2Int pos)
    {
        if (0 <= pos.x && pos.x < map.data.width && 0 <= pos.y && pos.y < map.data.height)
        {
            var index = pos.x + pos.y * map.data.width;
            if (map.data.data[index] != selectedTile)
            {
                map.data.data[index] = selectedTile;
                UpdateTile(pos);
            }
        }
    }

    private void UpdateTile(Vector2Int pos)
    {
        var index = pos.x + pos.y * map.data.width;
        var tile = map.data.data[index];
        Destroy(mapObjects[index]);
        mapObjects[index] = null;

        if (0 <= tile && tile < mapTiles.prefabs.Length)
        {
            var prefab = mapTiles.prefabs[tile];
            var obj = Instantiate(prefab, transform);
            obj.transform.localPosition = new Vector3(pos.x, 0, pos.y);
            mapObjects[index] = obj;
        }
    }

    public Vector2Int WorldToTilePos(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    public Vector3 TileToWorldPos(Vector2Int pos)
    {
        return new Vector3(pos.x, 0, pos.y);
    }

    public GameObject GetTileObject(Vector2Int pos)
    {
        if (map.IsValid(pos))
            return mapObjects[map.GetIndex(pos)];
        return null;
    }

    public Vector3? GetCursorWorldPos()
    {
        var plane = new Plane(Vector3.up, transform.position + Vector3.up * mapHeight);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            var cross = ray.GetPoint(enter);
            return cross;
        }
        return null;
    }

    public Vector2Int? GetCursorTilePos()
    {
        var cross = GetCursorWorldPos();
        if (cross.HasValue)
            return WorldToTilePos(cross.Value);
        return null;
    }
}
