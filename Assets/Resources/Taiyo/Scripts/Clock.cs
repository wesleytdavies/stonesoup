using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Tile
{
    public override void pickUp(Tile tilePickingUsUp)
    {
        Time.timeScale = 0.3f;
        base.pickUp(tilePickingUsUp);
    }

    public override void dropped(Tile tileDroppingUs)
    {
        Time.timeScale = 1;
        base.dropped(tileDroppingUs);
    }
}
