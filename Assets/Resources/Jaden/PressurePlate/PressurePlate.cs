using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : TripMine
{
    //pressure plates are visible
    //so can be used as puzzle elements, unlike trip mines 
    [SerializeField] Sprite pressedSprite;

    void Update() {
        renderer.enabled = true;
    }

    //pressure plates can be tripped by enemies as well as players 
    //can be tripped by anything honestly.
    public override void OnTriggerEnter2D(Collider2D collision) {
        Tile otherTile = collision.gameObject.GetComponent<Tile>();
        if (otherTile == null) {
            return;
        }
        renderer.sprite = pressedSprite;
        //if (otherTile.hasTag(TileTags.Player)) {
            StartCoroutine(Tripped(otherTile));
        //}
    }

}
