using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The minesweeper is an item that reveals nearby mines when held.
/// </summary>
public class MineSweeper : Tile
{
    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        foreach(TripMine mine in TripMine.AllMines)
        {
            if (mine == null)
            {
                continue;
            }
            mine.renderer.enabled = true;
        }
    }

    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        foreach (TripMine mine in TripMine.AllMines)
        {
            if (mine == null)
            {
                continue;
            }
            mine.renderer.enabled = false;
        }
    }
}
