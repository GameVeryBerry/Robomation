using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.IO;

public class TileStage : MonoBehaviour
{
    public class StageData
    {
        // 4 bytes マジックバイト 0xbebedead
        // 4 bytes width:int height:int
        // width*height bytes マップデータ

        public static readonly byte[] MagicByte = { 0xbe, 0xbe, 0xde, 0xad };

        public struct MapData
        {
            public short width, height;
            public byte[] data;
        }

        public MapData data;

        public bool Valid
        {
            get
            {
                return data.data != null;
            }
        }

        public void SaveMap(FileStream fs)
        {
            {
                fs.Write(MagicByte, 0, MagicByte.Length);
            }
            {
                fs.Write(BitConverter.GetBytes(data.width), 0, 2);
                fs.Write(BitConverter.GetBytes(data.height), 0, 2);
            }
            {
                fs.Write(data.data, 0, data.data.Length);
            }
        }

        public bool LoadMap(FileStream fs)
        {
            data = default;
            MapData map;

            {
                int cap = 4;
                var mgbcheck = new byte[cap];
                if (fs.Read(mgbcheck, 0, cap) < cap)
                    return false;
                if (!MagicByte.SequenceEqual(mgbcheck))
                    return false;
            }

            {
                int cap = 4;
                byte[] size = new byte[cap];
                if (fs.Read(size, 0, cap) < cap)
                    return false;
                short width = BitConverter.ToInt16(size, 0);
                short height = BitConverter.ToInt16(size, 2);
                if (width < 0)
                    return false;
                if (height < 0)
                    return false;
                map.width = width;
                map.height = height;
            }

            {
                int cap = (int)(map.width * map.height);
                byte[] data = new byte[cap];
                if (fs.Read(data, 0, cap) < cap)
                    return false;
                map.data = data;
            }

            data = map;
            return true;
        }

        public int GetIndex(Vector2Int pos)
        {
            return pos.x + pos.y * data.width;
        }

        public bool IsValid(Vector2Int pos)
        {
            return (0 <= pos.x && pos.x < data.width && 0 <= pos.y && pos.y < data.height);
        }

        public byte GetTile(int index)
        {
            return data.data[index];
        }

        public byte GetTile(Vector2Int pos)
        {
            return GetTile(GetIndex(pos));
        }
    }

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

    public Vector2Int? GetCursorTilePos()
    {
        var plane = new Plane(Vector3.up, transform.position + Vector3.up * mapHeight);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            var cross = ray.GetPoint(enter);
            var pos = WorldToTilePos(cross);
            return pos;
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveLoad.CreateGameDirectories();

        LoadMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var pos = GetCursorTilePos();
            if (pos.HasValue)
                SetTile(pos.Value);
        }
    }
}
