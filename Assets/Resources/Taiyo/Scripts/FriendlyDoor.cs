using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyDoor : Tile
{

    private EnemyUI _enemyUI;

    public Text valueText;
    private int _doorValue = 2;

    public int doorValueMin = 3;

    public int doorValueMax = 15;

    public AudioClip openSe;

    private bool _entered = false;
    private void Start()
    {
        Invoke("SetUp", 0.5f);

    }

    private void SetUp()
    {
        _doorValue = Random.Range(doorValueMin, doorValueMax);
        _enemyUI = GameObject.Find("EnemyUI").GetComponent<EnemyUI>();
        valueText.text = _doorValue.ToString();
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
        if (_entered)
            return;
        if(collision.gameObject.name == "player_tile(Clone)")
        {
            if(_doorValue <= _enemyUI.GetEnemyCount())
            {
                _entered = false;
                this.GetComponent<AudioSource>().PlayOneShot(openSe);
                Invoke("DestroySelf", 0.7f);
            }
        }
    }
    private void DestroySelf()
    {
        Destroy(this.gameObject);

    }
}
