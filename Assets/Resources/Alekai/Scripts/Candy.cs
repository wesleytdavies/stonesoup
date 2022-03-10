using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Candy : Tile
{
	public Vector2Int healthRange = new Vector2Int(2,6);
	public float healthStep = .7f;

	private SpriteRenderer _sr;
	private Color _startColor;
	private bool _activated = false;
	
	protected void Start()
	{
		health = Random.Range(healthRange.x, healthRange.y);
		_sr = GetComponent<SpriteRenderer>();
	}
	
	protected virtual void Update() {
		if (_tileHoldingUs != null) {
			tileName = string.Format("Candy (HP: {0})", health);
		}
		updateSpriteSorting();
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.GetComponent<Tile>() != null && (other.GetComponent<Tile>().tags & TileTags.Wall) != 0 && _activated)
		{
			other.GetComponent<Tile>().takeDamage(this, 10, DamageType.Explosive);
		}
	}

	public override void pickUp(Tile tilePickingUsUp)
	{
		base.pickUp(tilePickingUsUp);
		_startColor = tilePickingUsUp.sprite.color;
	}

	public override void dropped(Tile tileDroppingUs)
	{
		tileDroppingUs.sprite.color = _startColor;
		base.dropped(tileDroppingUs);
	}

	public override void useAsItem(Tile tileUsingUs)
	{
		StartCoroutine(triggerCandy(healthStep));
		StartCoroutine(changeColors(.06f));
		_activated = true;
	}

	// effect when used
	private IEnumerator triggerCandy(float step)
	{
		while (health > 0)
		{
			takeDamage(null, 1);
			yield return new WaitForSeconds(step);
		}
	}

	private IEnumerator changeColors(float step)
	{
		while (gameObject)
		{
			var color = Random.ColorHSV();
			_sr.color = color;
			if (_tileHoldingUs)
			{
				_tileHoldingUs.sprite.color = color;
			}
			yield return new WaitForSeconds(step);
		}
	}

	protected override void die()
	{
		if (_tileHoldingUs != null)
		{
			_tileHoldingUs.sprite.color = _startColor; // reset player color
		}
		base.die();
	}
}