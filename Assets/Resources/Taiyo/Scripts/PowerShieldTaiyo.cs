using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerShieldTaiyo : Tile
{

    public Collider2D onGroundCollider;
    public Collider2D heldCollider;

    public Sprite heldSprite;
    public Sprite onGroundSprite;


    private float _duration = 10;

    public override Collider2D mainCollider
    {
        get { return onGroundCollider; }
    }

    // Power shield does not take any damage
    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType) {
        Debug.Log("NO DAMAGE");
    }

    public override void pickUp(Tile tilePickingUsUp)
    {
        //Enemies cannot pick up power shield
        if (tilePickingUsUp.hasTag(TileTags.Enemy))
            return;

        base.pickUp(tilePickingUsUp);
        if (_tileHoldingUs == tilePickingUsUp)
        {
            transform.parent = null; // To make joints work, we have to do this. 
            _sprite.sprite = heldSprite;
            onGroundCollider.enabled = false;
            heldCollider.enabled = true;
            _body.bodyType = RigidbodyType2D.Dynamic;
            Joint2D ourJoint = GetComponent<Joint2D>();
            ourJoint.connectedBody = _tileHoldingUs.body;
            ourJoint.enabled = true;
        }
    }

    //Destroy when dropped
    public override void dropped(Tile tileDroppingUs)
    {
        this.die();
    }

    void Update()
    {
        if (_tileHoldingUs != null)
        {
            _duration -= Time.deltaTime;

            tileName = "PowerShield (Sec: {"+(int)(_duration+1)+"})";

            if (_duration <= 0)
                this.die();
        }
    }


}
