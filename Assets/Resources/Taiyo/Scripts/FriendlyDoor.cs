using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyDoor : Tile
{

    private EnemyUI _enemyUI;

    public Text valueText;
    public int doorValue = 2;

    private void Start()
    {
        Invoke("SetUp", 0.5f);

    }

    private void SetUp()
    {
        _enemyUI = GameObject.Find("EnemyUI").GetComponent<EnemyUI>();
        valueText.text = doorValue.ToString();
    }

    // Walls only take explosive damage.
    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
    {
        //Doesnt take damage
        /*
        if (damageType == DamageType.Explosive)
        {
            base.takeDamage(tileDamagingUs, amount, damageType);
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("‚±‚ç : " + collision.gameObject.tag);
        if(collision.gameObject.name == "player_tile(Clone)")
        {
            if(doorValue <= _enemyUI.GetEnemyCount())
            {
                Destroy(this.gameObject);
            }
        }
    }
}
