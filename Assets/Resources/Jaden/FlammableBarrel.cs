using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableBarrel : Tile
{

    //Every videogame needs exploding barrels.

    private int damageDeals = 3;
    private float explosionRadius = 1f;

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

        //deal explosive damage to tiles in a radius.
        Vector2 pos2D = toGridCoord(globalX, globalY);

        Collider[] inRadius = new Collider[15];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, inRadius);
        for(int i = 0; i < numColliders; i++) {
            Tile tile = inRadius[i].GetComponent<Tile>();
            if(tile != null) {
                tile.takeDamage(this, 3, DamageType.Explosive);
            }
        }
        
        //TODO play sfx?

		base.die();
	}
}
