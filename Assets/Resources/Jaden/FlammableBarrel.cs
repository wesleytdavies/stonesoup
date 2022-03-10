using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableBarrel : Tile
{

    //Every videogame needs exploding barrels.
    //Oil is valuable on the stock market. 


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

	protected override void die() {

        StopCoroutine(flashWhite());

        //TODO deal explosive damage to tiles in a 3x3 radius. 

		base.die();
	}
}
