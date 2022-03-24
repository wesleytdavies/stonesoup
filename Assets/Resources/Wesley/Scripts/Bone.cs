using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The bone can be thrown, which spawns and attracts a dog. Based off apt283Rock script.
/// </summary>
public class Bone : Tile
{
	public Dog dogPrefab;
	private Dog _thisDog;

	public float throwForce = 3000f;

	// How slow we need to be going before we consider ourself "on the ground" again
	public float onGroundThreshold = 0.8f;

	// We keep track of the tile that threw us so we don't collide with it immediately.
	protected Tile _tileThatThrewUs = null;

	// Keep track of whether we're in the air and whether we were JUST thrown
	protected bool _isInAir = false;
	protected float _afterThrowCounter;
	public float afterThrowTime = 0.2f;

	// Like walls, rocks need explosive damage to be hurt.
	public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
	{
		if (damageType == DamageType.Explosive)
		{
			base.takeDamage(tileDamagingUs, amount, damageType);
		}
	}

	public override void useAsItem(Tile tileUsingUs)
	{
		if (_tileHoldingUs != tileUsingUs)
		{
			return;
		}
		if (onTransitionArea())
		{
			return; // Don't allow us to be thrown while we're on a transition area.
		}

		_sprite.transform.localPosition = Vector3.zero;

		_tileThatThrewUs = tileUsingUs;
		_isInAir = true;

		// We use IgnoreCollision to turn off collisions with the tile that just threw us.
		if (_tileThatThrewUs.GetComponent<Collider2D>() != null)
		{
			Physics2D.IgnoreCollision(_tileThatThrewUs.GetComponent<Collider2D>(), _collider, true);
		}
		// We're thrown in the aim direction specified by the object throwing us.
		Vector2 throwDir = _tileThatThrewUs.aimDirection.normalized;

		// Have to do some book keeping similar to when we're dropped.
		_body.bodyType = RigidbodyType2D.Dynamic;
		transform.parent = tileUsingUs.transform.parent;
		_tileHoldingUs.tileWereHolding = null;
		_tileHoldingUs = null;

		_collider.isTrigger = false;

		// Since we're thrown so fast, we switch to continuous collision detection to avoid tunnelling
		// through walls.
		_body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

		// Finally, here's where we get the throw force.
		_body.AddForce(throwDir * throwForce);

		_afterThrowCounter = afterThrowTime;
	}

	protected virtual void Update()
	{
		if (_isInAir)
		{
			if (_afterThrowCounter > 0)
			{
				_afterThrowCounter -= Time.deltaTime;
			}
			// If we've been in the air long enough, need to check if it's time to consider ourself "on the ground"
			else if (_body.velocity.magnitude <= onGroundThreshold)
			{
				_body.velocity = Vector2.zero;
				if (_afterThrowCounter <= 0 && _tileThatThrewUs != null && _tileThatThrewUs.GetComponent<Collider2D>() != null)
				{
					Physics2D.IgnoreCollision(_tileThatThrewUs.GetComponent<Collider2D>(), _collider, false);
				}
				_body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
				_collider.isTrigger = true;
				addTag(TileTags.CanBeHeld);
				_isInAir = false;
				//still spawn the dog since no collision was made
				if (_thisDog == null)
				{
					_thisDog = Instantiate(dogPrefab, _tileThatThrewUs.transform.position, Quaternion.identity);
					_thisDog.tileWereChasing = this;
				}
			}
		}
		if (_tileHoldingUs != null)
		{
			// We aim the rock behind us.
			_sprite.transform.localPosition = new Vector3(-0.5f, 0, 0);
			float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x) * Mathf.Rad2Deg;
			transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
		}
		else
		{
			_sprite.transform.localPosition = Vector3.zero;
		}
		updateSpriteSorting();
	}

	public virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (_isInAir && collision.gameObject.GetComponent<Tile>() != null)
		{
            if (_thisDog == null)
            {
                _thisDog = Instantiate(dogPrefab, _tileThatThrewUs.transform.position, Quaternion.identity);
                _thisDog.tileWereChasing = this;
				_thisDog.tileThatThrewUs = this;
            }
        }
	}
}
