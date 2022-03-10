using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CuteWall : Tile
{
    public GameObject seedPrefab = null;
    public List<Sprite> spriteOptions = new List<Sprite>();
    private SpriteRenderer _sr = null;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.sprite = spriteOptions[Random.Range(0, spriteOptions.Count)]; // randomize sprite
    }

    protected override void die()
    {
        var pos = transform.position;
        var gridPos = toGridCoord(pos.x, pos.y);
        spawnTile(seedPrefab, transform.parent, (int)gridPos.x % 10,  (int)gridPos.y % 8);
        base.die();
    }

    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
    {
        if (damageType == DamageType.Explosive)
        {
            base.takeDamage(tileDamagingUs, amount, damageType);
        }
    }
}
