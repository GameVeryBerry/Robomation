using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    public Prefabs mapTiles;
    public string mapName = "sample";
    public StageData map;

    public void SaveMap()
    {
        map = new StageData();
        map.data.width = 100;
        map.data.height = 100;
        map.data.data = new byte[100*100];
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
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        if (map != null)
            for (int iy = 0; iy < map.data.height; iy++)
                for (int ix = 0; ix < map.data.width; ix++)
                {
                    var id = map.data.data[ix + iy * map.data.width];
                    var obj = Instantiate(mapTiles.prefabs[id], transform);
                    obj.transform.localPosition = new Vector3(ix, 0, iy);
                }
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

    }
}
