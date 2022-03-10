using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaiyoApple : Tile
{
    public override void pickUp(Tile tilePickingUsUp)
    {
        Debug.Log("Heal");

        //tilePickingUsUp.restoreAllHealth();
        //die();
        //base.pickUp(tilePickingUsUp);
    }
}
