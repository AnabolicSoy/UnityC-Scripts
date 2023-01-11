using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaNPCScr : MonoBehaviour
{
    Rigidbody2D RB;

    void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Active = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      

        if (Active && collision.CompareTag("WorldEdge"))
        {
    
            RB.velocity = new Vector2(-RB.velocity.normalized.x * Random.Range(4, 7), Random.Range(3,5));
            RB.AddTorque(Mathf.Clamp(RB.velocity.x, -4, 4) * 8);
        }
    }
    bool Active;
    public void SetActiveInactive(bool value)
    {
        Active = value;
    }
}
