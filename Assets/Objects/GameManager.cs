using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ゲーム管理クラス
public class GameManager : MonoBehaviour
{
    public StageMeta stageMeta;
    public bool isEditMode;
    public StageObject floor;
    public StageObject instruction;

    public StageObject FocusedStageObject
    {
        get => isEditMode ? floor : instruction;
        set
        {
            if (isEditMode)
                floor = value;
            else
                instruction = value;
        }
    }

    void Start()
    {
        // マップの初期化
        SaveLoad.CreateGameDirectories();
        floor.map = stageMeta.Load(SaveType.Floor) ?? stageMeta.Create();
        floor.UpdateMap();
        instruction.map = stageMeta.Load(SaveType.Instruction) ?? stageMeta.Create();
        instruction.UpdateMap();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var pos = instruction.GetCursorTilePos();
            if (pos.HasValue)
            {
                if (isEditMode)
                    instruction.SetTile(pos.Value);
                else
                {
                    if (floor.map.IsValid(pos.Value))
                    {
                        if (floor.map.GetTile(pos.Value) != 0xff)
                            instruction.SetTile(pos.Value);
                    }
                }
            }
        }
    }

    public void SaveFloor()
    {
        stageMeta.Save(SaveType.Floor, floor.map);
    }

    public void SaveInstruction()
    {
        stageMeta.Save(SaveType.Instruction, instruction.map);
    }

    public static GameManager Get()
    {
        return Object.FindObjectOfType<GameManager>();
    }
}
