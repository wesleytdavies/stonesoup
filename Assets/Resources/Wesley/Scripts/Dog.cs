using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// The dog chases the bone and damages any enemies in its path.
/// </summary>
public class Dog : Tile
{
    public Tile tileWereChasing;
    public Tile tileThatThrewUs;

    protected TileTags _attackTag;
    [SerializeField] protected AudioClip _dogBark;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        AudioManager.playAudio(_dogBark);
    }

    private void Update()
    {
        _body.position = Vector2.MoveTowards(transform.position, tileWereChasing.transform.position, 0.2f);
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
