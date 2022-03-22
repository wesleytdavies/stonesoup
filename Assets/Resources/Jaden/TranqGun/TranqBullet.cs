using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranqBullet : apt283Bullet
{

    [SerializeField] float secToTranq = 5;

    public override void OnCollisionEnter2D(Collision2D collision) {

        //TODO is this also getting the stuff that inherits from basic ai creature?
        BasicAICreature otherCreature = collision.gameObject.GetComponent<BasicAICreature>();
        if(otherCreature != null) {
            //if there's another creature
            StartCoroutine(Tranquilize(otherCreature));
            return;
        }

        //if not a creature, continue as normal 
        //damage anything else so you can use this to set off barrels 
        base.OnCollisionEnter2D(collision);
	}

    IEnumerator Tranquilize(BasicAICreature creature) {
        
        //haha.... BasicAICreature move speed is public... going to abuse this
        //make the creature unable to move 

        //it seems like if you tranq a creature twice it might stay unmoving forever.
        //cause the 2nd coroutine will save the 0s and set it to 0s at the end.
        //kind of neat behavior though? like you overdose them.
        //if i want to fix this, could just check if the speed and accel are already zero.
        while(creature.moveSpeed == 0) { //if already tranq'd, wait til the first one wears off 
            yield return null;
        } 

        float origMoveSpeed = creature.moveSpeed;
        float origMoveAcceleration = creature.moveAcceleration;
        creature.moveSpeed = 0;
        creature.moveAcceleration = 0;

        //wait a few seconds
        for(float counter = 0; counter < secToTranq; counter += Time.deltaTime) {
            yield return null;
        }

        //let the creature move again
        creature.moveSpeed = origMoveSpeed;
        creature.moveAcceleration = origMoveAcceleration;

    } 

    //TODO: could override die() to make bullet sprites remain even when inactive 
    //but, performance 
    
}
