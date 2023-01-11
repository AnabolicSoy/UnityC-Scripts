using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : InteractableScr
{
    [SerializeField]
    ParticleSystem PS;
    DC_SpawnerSystem SS;
    [SerializeField]
    Rigidbody2D RB;


    public override void SpawnFromBox()
    {
        IsSpawningFromBox = true;
    }
    bool IsSpawningFromBox;

    private void Start()
    {
        Destroy(gameObject,3);
        SS = DC_SpawnerSystem.DC_SS;
        if (IsSpawningFromBox)
        {
            if (transform.position.x > 0)
            {
                RB.AddForce(new Vector2(-Random.Range(2, 5), Random.Range(5, 8)), ForceMode2D.Impulse);
            }
            else
            {
                RB.AddForce(new Vector2(Random.Range(2, 5), Random.Range(5, 8)), ForceMode2D.Impulse);
            }
        }
        else
        {
            if (transform.position.x > 0)
            {
                RB.AddForce(new Vector2(-Random.Range(11, 15), Random.Range(10, 13)), ForceMode2D.Impulse);
            }
            else
            {
                RB.AddForce(new Vector2(Random.Range(11, 15), Random.Range(10, 13)), ForceMode2D.Impulse);
            }
        }
        RB.angularVelocity = Random.Range(-32,32);

    }
    public override void CallHit()
    {
      
            PS.Play();
            PS.gameObject.transform.parent = null;
            Destroy(PS.gameObject, 3);
            Destroy(gameObject);
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Base"))
        {
            SS.ChangeHP(DMG);
            PS.Play();
            
        }
    }
    [SerializeField]
    int DMG;
}
