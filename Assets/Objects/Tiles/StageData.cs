using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

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

