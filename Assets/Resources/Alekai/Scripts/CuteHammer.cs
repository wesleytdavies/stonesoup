using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuteHammer : Tile
{
	public Vector2Int healthRange = new Vector2Int(2,6);
	public float swingDuration = .1f;
	private SpriteRenderer _sr = null;

	private bool _activated = false;
	
	protected void Start()
	{
		health = Random.Range(healthRange.x, healthRange.y);
		_sr = GetComponent<SpriteRenderer>();
	}
	
	protected virtual void Update() {
		if (_tileHoldingUs != null) {
			tileName = $"Cute Hammer (HP: {health})";
		}
		updateSpriteSorting();
	}

	private IEnumerator swing()
	{
		_activated = true;
		_sr.color = Color.black; // come up w/ a better effect
		yield return new WaitForSeconds(swingDuration); // wait for swing to complete
		_sr.color = Color.white;
		_activated = false;
		takeDamage(null, 1);
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.GetComponent<Tile>() != null && (other.GetComponent<Tile>().tags & TileTags.Wall) != 0 && _activated)
		{
			other.GetComponent<Tile>().takeDamage(this, 10, DamageType.Explosive);
		}
	}

	public override void useAsItem(Tile tileUsingUs)
	{
		StartCoroutine(swing());
	}
}
