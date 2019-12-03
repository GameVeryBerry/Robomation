using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float duration = 1;
    float time = 0;

    public TileStage tileStage;
    Vector2Int currentPosition;

    public int direction = 0;
    readonly Vector2Int[] forward =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
    };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > duration)
        {
            time = 0;
            if (tileStage.map.IsValid(currentPosition + forward[direction]))
            {
                currentPosition += forward[direction];

                var tile = tileStage.GetTileObject(currentPosition);
                if (tile != null)
                {
                    var itile = tile.GetComponent<ITile>();
                    if (itile != null)
                        itile.OnTileAction(this);
                }
            }
        }

        transform.position = Vector3.Lerp(transform.position, tileStage.TileToWorldPos(currentPosition), .2f);
    }
}