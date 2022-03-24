using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/// <summary>
/// The tripmine object gets tripped if the player steps on it and explodes if the player moves before the max ticking time is up.
/// </summary>
public class TripMine : Tile {
    public static List<TripMine> AllMines = new List<TripMine>();

    protected int _damageDealt = 1;
    protected float _initalLeewayTime = 0.3f; //how much time the player has to react to stepping on a tripmine before it explodes
    protected float _maxTickingTime = 3f;
    protected float _explosionRadius = 0.75f;

    [SerializeField] protected AudioClip _trippedSound;
    [SerializeField] protected AudioClip _explosionSound;
    [SerializeField] protected AudioClip _deactivateSound;
    public new SpriteRenderer renderer;
    //protected Animator _animator;
    protected TextMeshProUGUI _freezeText;

    public override void init() {
        base.init();
        renderer = GetComponent<SpriteRenderer>();
        //_animator = GetComponent<Animator>();
        //_animator.speed = 0f;
        _freezeText = GetComponentInChildren<TextMeshProUGUI>();
        renderer.enabled = false;
        AllMines.Add(this);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision) {
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
        _freezeText.text = "Freeze!";
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
                    StartCoroutine(Explode(collidersInRange));
                    yield break;
                }
            }
            timeTicking += Time.deltaTime;
            yield return null;
        }
        deathSFX = _deactivateSound;
        AllMines.Remove(this);
        die();
        Debug.Log("Deactivated");
        yield break;
    }

    protected IEnumerator Explode(Collider2D[] collidersToExplode)
    {
        renderer.enabled = true;
        //_animator.speed = 1f;
        deathSFX = _explosionSound;
        yield return new WaitForSeconds(0.2f);
        foreach (Collider2D collider in collidersToExplode)
        {
            if (collider == null)
                continue;
            Tile otherTile = collider.gameObject.GetComponent<Tile>();
            if (otherTile != null)
            {
                otherTile.takeDamage(this, _damageDealt, DamageType.Explosive);
            }
        }
        yield return new WaitForSeconds(0.4f);
        AllMines.Remove(this);
        die();
        yield break;
    }
}
