using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CandyCorn : Tile
{
    public List<Sprite> growthSprites = new List<Sprite>();
    public float growthTimer = 5f;
    private int _growthState = 0; // 0 is seed, 1 is planted, 2 is grown
    // use Grow() to advance ^^^

    private SpriteRenderer _sr = null;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update() {
        if (_tileHoldingUs != null) {
            switch (_growthState)
            {
                case 0:
                    tileName = "Candy Seed";
                    break;
                case 1:
                    tileName = "Candy Sapling";
                    break;
                case 2:
                    tileName = "Candy Heart";
                    break;
            }
        }

        if (_growthState == 1 && growthTimer > 0)
        {
            growthTimer -= Time.deltaTime;
        } else if (_growthState == 1)
        {
            Grow();
        }
        updateSpriteSorting();
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        switch (_growthState)
        {
            case 0:
                Grow();
                dropped(_tileHoldingUs);
                break;
            case 2:
                tileUsingUs.health += 1;
                takeDamage(tileUsingUs, 100);
                break;
        }
    }

    private void Grow()
    {
        _growthState += 1;
        _sr.sprite = growthSprites[_growthState];
    }
}