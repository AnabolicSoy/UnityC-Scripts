using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDropScr : InteractableScr
{
    [SerializeField]
    ParticleSystem PS;

    [SerializeField]
    Rigidbody2D RB;

    [SerializeField]
    SpriteRenderer SR;

    DC_SpawnerSystem SS;

    private void Start()
    {
        Destroy(gameObject, 4);
        SS = DC_SpawnerSystem.DC_SS;
        if (transform.position.x > 0)
        {
            RB.AddForce(new Vector2(-Random.Range(11, 15), Random.Range(10, 13)), ForceMode2D.Impulse);
        }
        else
        {
            RB.AddForce(new Vector2(Random.Range(11, 15), Random.Range(10, 13)), ForceMode2D.Impulse);
        }
        RB.angularVelocity = Random.Range(-32, 32);

    }

    public override void CallHit()
    {

        PS.Play();
        PS.gameObject.transform.parent = null;
        Destroy(PS.gameObject, 3);

        SR.color = Color.clear;

        int Spawns = Random.Range(2, 5);
        for (int i = 0; i < Spawns; i++)
        {
            GameObject obj = Instantiate(SpawnsOnExplode[Random.Range(0, SpawnsOnExplode.Length)], gameObject.transform.position, Quaternion.identity);
            obj.GetComponent<InteractableScr>().SpawnFromBox();
        }

    }

    [SerializeField]
    GameObject[] SpawnsOnExplode;

}
