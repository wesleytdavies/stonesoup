using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPack : Tile
{
    public GameObject booster;
    public SpriteRenderer mainSprite;
   

    // Walls only take explosive damage.
    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
    {
        if (damageType == DamageType.Explosive)
        {
            base.takeDamage(tileDamagingUs, amount, damageType);
        }
    }

    void FixedUpdate()
    {
        if (_tileHoldingUs != null)
        {
            //Something while holding
        }
        updateSpriteSorting();
    }

    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        if (tilePickingUsUp.hasTag(TileTags.Player))
        {
            var p = GameObject.Find("player_tile(Clone)").GetComponent<Player>();
            p.moveSpeed *= 2;
            p.moveAcceleration *= 2;

            mainSprite.color = new Color(0, 0, 0, 0);
            booster.SetActive(true);
        }
    }

    

    public override void dropped(Tile tileDroppingUs)
    {
        base.dropped(tileDroppingUs);
        var p = GameObject.Find("player_tile(Clone)").GetComponent<Player>();
        p.moveSpeed /= 2;
        p.moveAcceleration /= 2;

        mainSprite.color = new Color(1, 1, 1, 1);
        booster.SetActive(false);
    }
    

}
