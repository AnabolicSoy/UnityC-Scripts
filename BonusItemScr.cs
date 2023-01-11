using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItemScr : InteractableScr
{
    [SerializeField]
    ParticleSystem PS;
    [SerializeField]
    Rigidbody2D RB;
    [SerializeField]
    SpriteRenderer SR;

    DC_SpawnerSystem SS;

    public override void SpawnFromBox()
    {
        IsSpawningFromBox = true;
    }
    bool IsSpawningFromBox;

    private void Start()
    {
        Destroy(gameObject, 4);
        SS = DC_SpawnerSystem.DC_SS;
        if(IsSpawningFromBox)
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
       
        RB.angularVelocity = Random.Range(-32, 32);

    }
    public override void CallHit()
    {

        PS.Play();
        PS.gameObject.transform.parent = null;
        Destroy(PS.gameObject, 3);
        SR.color = Color.clear;
        IsDeactivated = true;
        DC_UIMDS.UIMDS.AddRemoveCoins(CoinsOnExplode);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsDeactivated && collision.gameObject.CompareTag("Base"))
        {
            SS.ChangeHP(Healing);
            PS.Play();

        }
    }

    bool IsDeactivated;

    [SerializeField]
    int Healing;

    [SerializeField]
    int CoinsOnExplode;
}

