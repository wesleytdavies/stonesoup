using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellCreature : FriendlyEnemy
{


    [SerializeField] int friendlyHealthThreshold = 1;
    [SerializeField] Sprite readyToFriendlySprite;



    // Update is called once per frame
    void Update()
    {
        //copied from parent- ?? 
        // Update our counters.
        if (_nextMoveCounter > 0)
        {
            _nextMoveCounter -= Time.deltaTime;
        }

        // When it's time to try a new move.
        if (_nextMoveCounter <= 0)
        {
            takeStep();
        }

        updateSpriteSorting();

        CheckIfShouldChange();
    }

    protected override void CheckIfShouldChange()
    {
        if(health <= friendlyHealthThreshold) {
            base.CheckIfShouldChange();
        }
        
    }

    public override void takeDamage(Tile tileDamagingUs, int damageAmount, DamageType damageType)
    {
        if(health - damageAmount <= friendlyHealthThreshold) {
            ReadyToFriendly();
        }
        base.takeDamage(tileDamagingUs, damageAmount, damageType);
    }

    void ReadyToFriendly() {
        //TODO sound effect or other feedback? 
        mySprite.sprite = readyToFriendlySprite;
    }
}

