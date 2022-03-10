using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableBarrel : Tile
{

    //Every videogame needs exploding barrels.

    private int damageDeals = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	public override void takeDamage(Tile tileDamagingUs, int damageAmount, DamageType damageType) {
		
        //make the sprite flash to warn we've recieved dmg 
        StartCoroutine(flashWhite());

        base.takeDamage(tileDamagingUs, damageAmount, damageType);
        
	}

    IEnumerator flashWhite() {
        _sprite.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        _sprite.color = Color.white;
        
    }

	protected override async void die() {

        StopCoroutine(flashWhite());

        //deal explosive damage to tiles in a 3x3 radius.
        TileTags tags = TileTags.Wall + TileTags.CanBeHeld + TileTags.Creature + TileTags.Player +
                     TileTags.Enemy + TileTags.Friendly + TileTags.Weapon + TileTags.Consumable + 
                     TileTags.Wearable + TileTags.Money + TileTags.Dateable + TileTags.Dirt + 
                     TileTags.Plant + TileTags.Flammable + TileTags.Merchant;

        Vector2 pos = new Vector2(Tile.toGridCoord(globalX), Tile.toGridCoord(globalY));
        
        for(int i = pos.x - 1; i <= pos.x + 1; i++) {
            for(int j = pos.y - 1; j <= pos.y + 1; j++) {
                Tile.tileAtPoint(new Vector2(i, j), tags).takeDamage(this, 3, DamageType.Explosive);
            }
        }
        
        //TODO play sfx?

		base.die();
	}
}
