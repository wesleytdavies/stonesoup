using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//the tripmine object gets tripped if the player steps on it and explodes if the player moves before the max ticking time is up
public class TripMine : Tile {
    protected int _damageDealt = 1;
    protected float _initalLeewayTime = 0.25f; //how much time the player has to react to stepping on a tripmine before it explodes
    protected float _maxTickingTime = 3f;
    protected float _explosionRadius = 0.75f;
    [SerializeField] protected AudioClip _trippedSound;
    [SerializeField] protected AudioClip _explosionSound;
    [SerializeField] protected AudioClip _deactivateSound;

    private void OnTriggerEnter2D(Collider2D collision) {
        Tile otherTile = collision.gameObject.GetComponent<Tile>();
        if (otherTile == null) {
            return;
        }
        if (otherTile.hasTag(TileTags.Player)) {
            StartCoroutine(Tripped(otherTile));
        }
    }

    protected virtual IEnumerator Tripped(Tile tripperTile) {
        AudioManager.playAudio(_trippedSound);
        Debug.Log("Tripped");
        yield return new WaitForSeconds(_initalLeewayTime);
        //check if player has already teleported away during the leeway period and die and break if they have
        if(Vector2.Distance(tripperTile.transform.position, transform.position) > TILE_SIZE * 2f) {
            die();
            yield break;
        }
        Vector2 minePosition = tripperTile.transform.position;
        float timeTicking = 0f;
        while (timeTicking <= _maxTickingTime) {
            if (tripperTile.body.velocity != Vector2.zero) {
                Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(minePosition, _explosionRadius);
                if(Array.Exists(collidersInRange, x => x.gameObject.GetComponent<Tile>() == tripperTile)) {
                    foreach (Collider2D collider in collidersInRange) {
                        Tile otherTile = collider.gameObject.GetComponent<Tile>();
                        if (otherTile != null) {
                            tripperTile.takeDamage(this, _damageDealt, DamageType.Explosive);
                        }
                    }
                    deathSFX = _explosionSound;
                    die();
                    yield break;
                }
            }
            timeTicking += Time.deltaTime;
            yield return null;
        }
        deathSFX = _deactivateSound;
        die();
        Debug.Log("Deactivated");
        yield break;
    }
}
