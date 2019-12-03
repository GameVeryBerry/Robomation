using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTile : MonoBehaviour, ITile
{
    public int direction;

    public void OnTileAction(RobotController robot)
    {
        robot.direction = direction;
    }
}
