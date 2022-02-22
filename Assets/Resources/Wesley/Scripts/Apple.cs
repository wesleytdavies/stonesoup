using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Tile {
    public override void pickUp(Tile tilePickingUsUp) {
        tilePickingUsUp.restoreAllHealth();
        die();
    }
}
