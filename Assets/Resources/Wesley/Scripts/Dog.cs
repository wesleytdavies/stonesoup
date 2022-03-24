using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// The dog is supposed to chase the bone and damage any enemies in its path. I haven't gotten it to work quite yet!
/// </summary>
public class Dog : Tile
{
    public Tile tileWereChasing;

    private void Start()
    {
        //StartCoroutine(MoveToTarget(tileWereChasing));
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _body.position = Vector2.MoveTowards(transform.position, tileWereChasing.transform.position, 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.GetComponent<Tile>())
        {
            return;
        }
        Tile collidedTile = collision.gameObject.GetComponent<Tile>();
        if (collidedTile.hasTag(TileTags.Enemy))
        {
            //damage the tile we collided with
            collidedTile.takeDamage(this, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != tileWereChasing.gameObject)
        {
            return;
        }
        die();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == tileWereChasing.gameObject)
        {
            die();
        }
    }
}
