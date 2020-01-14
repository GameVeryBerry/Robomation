using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageMeta : ScriptableObject
{
    public string mapName;
    public Vector2Int size;

    public void Save(SaveType type, StageData map)
    {
        SaveLoad.SaveBinary(map, mapName, SaveType.Floor, (fs, data) => data.SaveMap(fs), overwrite: true);
    }

    public StageData Load(SaveType type)
    {
        return SaveLoad.LoadBinary(mapName, SaveType.Floor, fs =>
        {
            var data = new StageData();
            if (data.LoadMap(fs))
                return data;
            return null;
        });
    }

    public StageData Create()
    {
        return new StageData
        {
            data =
            {
                width = (short) size.x,
                height = (short) size.y,
                data = Enumerable.Repeat<byte>(0xff, size.x * size.y).ToArray()
            }
        };
    }
}
