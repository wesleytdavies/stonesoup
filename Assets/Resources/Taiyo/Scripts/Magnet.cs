using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : Tile
{

    private bool _holding = false;
    public MagnetCollider magnetCollider;


    public override void pickUp(Tile tilePickingUsUp)
    {
        //Enemies cannot pick up power shield
        if (tilePickingUsUp.hasTag(TileTags.Enemy))
            return;

        base.pickUp(tilePickingUsUp);
        _holding = true;
    }

    private void Update()
    {
        if (_holding && !magnetCollider.IsHittingWall())
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject clickedGameObject = null;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

                if (hit2d)
                {
                    clickedGameObject = hit2d.transform.gameObject;
                }

                if(clickedGameObject != null)
                {
                    Tile tile = clickedGameObject.GetComponent<Tile>();
                    if (tile != null && tile.hasTag(TileTags.CanBeHeld))
                    {
                        clickedGameObject.transform.position = this.transform.position + new Vector3(0, 1, 0);
                        //clickedGameObject.
                    }
                }
            }
        }

    }



    //Destroy when dropped
    /*public override void dropped(Tile tileDroppingUs)
    {
        this.die();
    }*/




}
