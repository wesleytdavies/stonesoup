using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpell : Tile
{


    protected ContactPoint2D[] _contacts = null;

    void Start()
    {
        _contacts = new ContactPoint2D[10];
        if (GetComponent<TrailRenderer>() != null)
        {
            GetComponent<TrailRenderer>().Clear();
        }
    }

    void Update()
    {
        if (_body == null)
            _body = this.GetComponent<Rigidbody2D>();

        _body.velocity = Vector2.zero;
    }

    public void DestroySelf()
    {
        die();
    }


    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>() != null)
        {

            Tile otherTile = collision.gameObject.GetComponent<Tile>();

            //Do not deal damage to player
            if (otherTile.hasTag(TileTags.Player) || otherTile.hasTag(TileTags.Friendly))
                return;

            otherTile.takeDamage(this, 1);
        }
    }
}
