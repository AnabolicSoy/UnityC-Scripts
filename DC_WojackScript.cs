using System.Collections;
using UnityEngine;

public class DC_WojackScript : MonoBehaviour
{
    DC_UIMDS UIMDS;

    Rigidbody2D RB;

    Sprite OriginalVariant;

    SpriteRenderer SR;

    BoxCollider2D Box;

    public bool SuspendAutoDispawn;

    [SerializeField]
    bool IsWojak;

    SpriteRenderer ShieldSR;
    SpriteRenderer WeaponSR;

    private void Awake()
    {
        UIMDS = DC_UIMDS.UIMDS;
        SS = DC_SpawnerSystem.DC_SS;
        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        OriginalVariant = SR.sprite;
        Box = GetComponent<BoxCollider2D>();
        if (Shield != null)
        { ShieldSR = Shield.GetComponent<SpriteRenderer>(); }
        if (Weapon != null)
        { WeaponSR = Weapon.GetComponent<SpriteRenderer>(); }
        Mathf.Clamp(DefenceValue, 10, 50);

        if(GetComponent<NinjaNPCScr>() != null)
        {
            CanBounceOffWalls = true;
        }

    }

    bool CanBounceOffWalls;

    DC_SpawnerSystem SS;

    [SerializeField]
    GameObject SlashEffectObject; //Only uesed if pool is full

    [SerializeField]
    Sprite NoCutVariant;
    [SerializeField]
    Sprite[] VerticalHalf;
    [SerializeField]
    Sprite[] HorizontalHalf;
    [SerializeField]
    Sprite[] DiagonalHalfPos;
    [SerializeField]
    Sprite[] DiagonalHalfNeg;

    [SerializeField]
    Rigidbody2D Weapon;
    [SerializeField]
    Rigidbody2D Shield;

    [SerializeField]
    Transform WeaponSlot;
    [SerializeField]
    Transform ShieldSlot;

    [SerializeField]
    int CoinsDrop;

    [SerializeField]
    int DMG;

    bool CanDealDMG;

    float CoinMod = 0;

    ParticleSystem LastUsedParticleSystem;

    [SerializeField]
    Sprite[] DeflectAtackAnimations;
    [SerializeField]
    bool CanDeflectAtack;
    [SerializeField]
    int DefenceValue; // DefenceValue is compared to atack power in order to calculate the chance that a wojak is killed
                      // DefnceValue [10-50]
    bool IsDefending;

    float DespawnTimer;

    //----------------------------------------------//

    public enum CutDirection
    {
        Vertical,
        Horizontal,
        DiagonalPos,
        DiagonalNeg,
        Point,
    }

    public void ActivateNPC()
    {
        Debug.Log("ActivateNPC");
        CanDealDMG = true;
        SR.flipX = false;
        SR.sprite = OriginalVariant;

        SR.color = Color.white;

        transform.rotation = Quaternion.identity;

        RB.simulated = true;
        Box.enabled = true;
        if (Weapon != null)
        {
            Debug.Log("Weapon != null 1");
            Weapon.transform.parent = WeaponSlot;
            Weapon.transform.localPosition = new Vector3(0, 0, 0);
            Weapon.transform.rotation = new Quaternion(0, 0, 0, 0);
            Weapon.gameObject.SetActive(true);
            Weapon.simulated = false;
            Debug.Log("Weapon != null 2");
            WeaponSR.color = Color.white;
        }
        if(Shield != null)
        {
            Debug.Log("Shield != null 1");
            Shield.transform.parent = ShieldSlot;
            Shield.transform.localPosition = new Vector3(0, 0, 0);
            Shield.transform.rotation = new Quaternion(0, 0, 0, 0);
            Shield.gameObject.SetActive(true);
            Shield.simulated = false;
            Debug.Log("Shield != null 2");
            ShieldSR.color = Color.white;
        }



    }

    private void TryDropWeapons()
    {
        if(Shield != null && Weapon != null)
        {
            SS.DoWeaponDropCoRoutine(Weapon.gameObject, Shield.gameObject);
            Debug.Log("Drop3");
        }
        else if (Shield != null && Weapon == null)
        {
            SS.DoWeaponDropCoRoutine(null, Shield.gameObject);
            Debug.Log("Drop3");
        }
        else if (Shield == null && Weapon != null)
        {
            SS.DoWeaponDropCoRoutine(Weapon.gameObject, null);
            Debug.Log("Drop3");
        }

        if (Weapon != null)
        {
            Weapon.simulated = true;
            Weapon.transform.parent = null;
            Weapon.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-48, 48);
            Weapon.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-16, 16), Random.Range(-16, 16)).normalized * Random.Range(1, 3), ForceMode2D.Impulse);
        }
        if (Shield != null)
        {
            Shield.simulated = true;
            Shield.transform.parent = null; Shield.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-48, 48);
            Shield.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-16, 16), Random.Range(-16, 16)).normalized * Random.Range(1, 3), ForceMode2D.Impulse);
        }

    }

    //-----------Call Hit----------------------//

    public void CallHit(CutDirection direction, int CutPower, int CutLength, Vector3 CutVector)
    {
        //CutPower [0,20]
        //CutLength [1,8]

        if (CanDeflectAtack && Random.Range(0,DefenceValue) > CutPower + CutLength)
        {
            SR.sprite = DeflectAtackAnimations[Random.Range(0, DeflectAtackAnimations.Length)];
           
            if (IsDefending && DeflectAtackAnimations.Length == 1)
            {
                SR.flipX = !SR.flipX;
            }
            IsDefending = true;
            CutVector.y = Mathf.Abs(CutVector.y);
            RB.AddForce(2*Mathf.Clamp(CutLength ,1,3)* CutVector, ForceMode2D.Impulse);

            RB.AddTorque(CutVector.x*16);

            DespawnTimer += 2;
        }
        else
        {

        if (CutLength > 2)
        {
             if (CutPower < 5)
            {
                CoinMod = 0.5f;
                DoHitEffect(direction);
                SpawnEffect(ParticleEffects.WhiteCrossFlash);
            }
            else if (CutPower < 8)
            {
                CoinMod = 1;
                StartCoroutine(EffectCoRoutine_5());
            }
            else if (CutPower <  12)
            {
                CoinMod = 1.2f;
                StartCoroutine(EffectCoRoutine_5());
            }
            else if (CutPower < 15)
            {
                CoinMod = 1.5f;
                StartCoroutine(EffectCoRoutine_3(direction));
            }
            else if (CutPower < 17)
            {
                CoinMod = 2f;
                StartCoroutine(EffectCoRoutine_4(direction));
            }
            else if (CutPower <= 20)
            {
                CoinMod = 3f;
                if (Random.Range(0,20) > CutPower)
                {
                    StartCoroutine(EffectCoRoutine_2(1, .3f, 32, direction));
                }
                else
                {
                    StartCoroutine(EffectCoRoutine_1(1, .3f, 32, direction));
                }


            }
        }
        else
        {

            if (Random.Range(0, 20) > CutPower)
            {
                DoHitEffectNoSplit();
                CoinMod = 0.1f;
            }
            else 
            {
                CoinMod = 0.5f;
                DoHitEffect(direction);
                SpawnEffect(ParticleEffects.WhiteCrossFlash);
            }
        }

        UIMDS.AddRemoveCoins(Mathf.RoundToInt((float)CoinsDrop * CoinMod));

        }
    }

    //----------------Hit Types-----------------//
    private void DoHitEffect(CutDirection direction)
    {
        Debug.Log("DoHitEffect");
        if (SS.TryGetObjectsFromPool(out GameObject Obj, out GameObject Obj1))
        {
            Debug.Log("------------");
            SS.CallSlashCoRoutine(3, Obj, Obj1);
            Obj.transform.position = gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f;
            Obj1.transform.position = gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f;
            Debug.Log("------------");
        }
        else
        {
            Obj = Instantiate(SlashEffectObject, gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f, Quaternion.identity);
            Obj1 = Instantiate(SlashEffectObject, gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f, Quaternion.identity);

            Destroy(Obj, 3);
            Destroy(Obj1, 3);
        }

        Obj.transform.rotation = Quaternion.identity;
        Obj1.transform.rotation = Quaternion.identity;
        Obj.transform.SetParent(gameObject.transform.parent);
        Obj.transform.localScale = new Vector3(DC_SpawnerSystem.Get_NPC_Scale, DC_SpawnerSystem.Get_NPC_Scale, 0);
        Obj1.transform.SetParent(gameObject.transform.parent);
        Obj1.transform.localScale = new Vector3(DC_SpawnerSystem.Get_NPC_Scale, DC_SpawnerSystem.Get_NPC_Scale, 0);

        Obj.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-18, 19);
        Obj1.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-18, 19);

        Obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-16, 16), Random.Range(-16, 16)).normalized * Random.Range(1, 3), ForceMode2D.Impulse);
        Obj1.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-16, 16), Random.Range(-16, 16)).normalized * Random.Range(1, 3), ForceMode2D.Impulse);


        switch (direction)
        {
            case CutDirection.Vertical:
                Obj.GetComponent<SpriteRenderer>().sprite = VerticalHalf[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = VerticalHalf[1];

                break;
            case CutDirection.Horizontal:
                Obj.GetComponent<SpriteRenderer>().sprite = HorizontalHalf[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = HorizontalHalf[1];

                break;
            case CutDirection.DiagonalPos:
                Obj.GetComponent<SpriteRenderer>().sprite = DiagonalHalfPos[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = DiagonalHalfPos[1];

                break;
            case CutDirection.DiagonalNeg:
                Obj.GetComponent<SpriteRenderer>().sprite = DiagonalHalfNeg[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = DiagonalHalfNeg[1];

                break;
        }

        TryDropWeapons();

        SR.color = Color.clear;

        

        CanDealDMG = false; //Redundant...in theory
        Box.enabled = false;
        StartCoroutine(DespawnRoutine(3));
    }
    private void DoHitEffect(CutDirection direction, float ForceMod, float AngularVelocityMaxValue)
    {
        Debug.Log("HitEffect");
        if (SS.TryGetObjectsFromPool(out GameObject Obj, out GameObject Obj1))
        {
          
            SS.CallSlashCoRoutine(3, Obj, Obj1);
            Obj.transform.position = gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f;
            Obj1.transform.position = gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f;
           
        }
        else
        {
            Obj = Instantiate(SlashEffectObject, gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f, Quaternion.identity);
            Obj1 = Instantiate(SlashEffectObject, gameObject.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0) * 0.1f, Quaternion.identity);

            Destroy(Obj, 3);
            Destroy(Obj1, 3);
        }

        Obj.transform.rotation = Quaternion.identity;
        Obj1.transform.rotation = Quaternion.identity;
        Obj.transform.SetParent(gameObject.transform.parent);
        Obj.transform.localScale = new Vector3(DC_SpawnerSystem.Get_NPC_Scale, DC_SpawnerSystem.Get_NPC_Scale, 0);
        Obj1.transform.SetParent(gameObject.transform.parent);
        Obj1.transform.localScale = new Vector3(DC_SpawnerSystem.Get_NPC_Scale, DC_SpawnerSystem.Get_NPC_Scale, 0);

        Obj.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-AngularVelocityMaxValue, AngularVelocityMaxValue)  ;
        Obj1.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-AngularVelocityMaxValue, AngularVelocityMaxValue)  ;

        Obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-16, 16), Random.Range(-16, 16)).normalized * ForceMod, ForceMode2D.Impulse);
        Obj1.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-16, 16), Random.Range(-16, 16)).normalized * ForceMod, ForceMode2D.Impulse);


        switch (direction)
        {
            case CutDirection.Vertical:
                Obj.GetComponent<SpriteRenderer>().sprite = VerticalHalf[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = VerticalHalf[1];

                break;
            case CutDirection.Horizontal:
                Obj.GetComponent<SpriteRenderer>().sprite = HorizontalHalf[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = HorizontalHalf[1];

                break;
            case CutDirection.DiagonalPos:
                Obj.GetComponent<SpriteRenderer>().sprite = DiagonalHalfPos[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = DiagonalHalfPos[1];

                break;
            case CutDirection.DiagonalNeg:
                Obj.GetComponent<SpriteRenderer>().sprite = DiagonalHalfNeg[0];
                Obj1.GetComponent<SpriteRenderer>().sprite = DiagonalHalfNeg[1];

                break;
        }

        TryDropWeapons();

        SR.color = Color.clear;



        CanDealDMG = false; //Redundant...in theory
        Box.enabled = false;
        StartCoroutine(DespawnRoutine(3));
    }
    private void DoHitEffectNoSplit()
    {
        SR.sprite = NoCutVariant;

        TryDropWeapons();

        CanDealDMG = false;

        StartCoroutine(DespawnRoutine(3));

        Box.enabled = false;
    }
    private void DoHitEffectToConversionSpawn()
    {
        if(IsWojak)
        {
            SR.sprite = SS.GetRandomConversionSpawnFruit();
        }
        else
        {
            SR.sprite = SS.GetRandomConversionSpawnPepes();
        }
     

        RB.AddForce(new Vector2(Random.Range(-16, 16), Random.Range(-16, 16)).normalized * Random.Range(1, 3), ForceMode2D.Impulse);

        RB.angularVelocity = Random.Range(-32, 32);

        TryDropWeapons();

        CanDealDMG = false;

        StartCoroutine(DespawnRoutine(3));

        Box.enabled = false;
    }
    //-------------------------------------------//

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CanDealDMG && collision.CompareTag("Base"))
        {
     
            SS.ChangeHP(-DMG);
            StartCoroutine(DespawnRoutine(1));
      
        }
        else if(collision.CompareTag("WorldEdge") && !CanBounceOffWalls)
        {
            CanDealDMG = false;
            StartCoroutine(DespawnRoutine(1));
        }
        else if(collision.CompareTag("DeactivateLayer"))
        {
            StartCoroutine(DespawnRoutine(1));
        }
    }

    IEnumerator EffectCoRoutine_1(float VibrationTime, float VibrationAmplitude, float VibrationFrequency, CutDirection direction)    //Wojak stops mid air, vibrates for X seconds and then (break or switch to dmg state)
    {
        RB.simulated = false;
        RB.velocity = new Vector2(0,0);
        SpawnEffect(ParticleEffects.Leaves,  VibrationTime);

        float T = 3;
        while (T > 0)
        {
            T -= Time.deltaTime;
            transform.position += new Vector3(0, Time.deltaTime * .3f, 0) + new Vector3(0, 0.05f *  Mathf.Sin(Time.time * 24), 0);

            yield return null;
        }

        Vector3 RefPos = transform.position;

        while (VibrationTime > 0)
        {
            VibrationTime -= Time.deltaTime;

            transform.position = RefPos + new Vector3(0, VibrationAmplitude * Mathf.Sin(Time.time* VibrationFrequency), 0);
            
            yield return null;
        }

        RB.simulated = true;

        
            DoHitEffect(direction);
            SpawnEffect(ParticleEffects.PurpleLineCircleFlash);
     

    }
    IEnumerator EffectCoRoutine_2(float VibrationTime, float VibrationAmplitude, float VibrationFrequency, CutDirection direction)   //Wojak  stops mid air, glows progressively more until it explodes , PlAY explosion effect
    {
        RB.simulated = false;
        RB.velocity = new Vector2(0, 0);
        SpawnEffect(ParticleEffects.Leaves,-2 + VibrationTime);

        float T = 1;
        while (T > 0)
        {
            T -= Time.deltaTime;
            transform.position += new Vector3(0, Time.deltaTime * .3f, 0) + new Vector3(0, 0.05f * Mathf.Sin(Time.time * 24), 0);

            yield return null;
        }

        Vector3 RefPos = transform.position;

        while (VibrationTime > 0)
        {
            VibrationTime -= Time.deltaTime;

            transform.rotation = Quaternion.Euler(0,0, 360 * Mathf.Sin(Time.time * 3));

            yield return null;
        }

        RB.simulated = true;

       
            DoHitEffect(direction);
            SpawnEffect(ParticleEffects.WhiteCircleFlash);
        
       
    }
    IEnumerator EffectCoRoutine_3( CutDirection direction)    //wojak stops mid air, play multi slash effect, (break or switch to dmg state)
    {
        RB.simulated = false;
        RB.velocity = new Vector2(0, 0);

        SpawnEffect(ParticleEffects.PurpleLineCircleFlash);

        DoHitEffect(direction,6,48);

       yield return null;
    }
    IEnumerator EffectCoRoutine_4(CutDirection direction)
    {

        RB.simulated = false;
        RB.velocity = new Vector2(0, 0);

        SpawnEffect(ParticleEffects.WhiteCircleFlash);

        DoHitEffect(direction, 12, 0);

        yield return null;
    }
    IEnumerator EffectCoRoutine_5()
    {
        
        RB.velocity = new Vector2(0, 0);

        SpawnEffect(ParticleEffects.SmokeCloud);

        DoHitEffectToConversionSpawn();

        yield return null;
    }

    IEnumerator DespawnRoutine(float Timer)
    {
        DespawnTimer = Timer;
        while (DespawnTimer > 0)
        {
            DespawnTimer -= Time.deltaTime;

            yield return null;

        }

        SS.AddToInactive(gameObject);
        gameObject.SetActive(false);

        if (LastUsedParticleSystem != null)
        {
            LastUsedParticleSystem.transform.parent = gameObject.transform;
            LastUsedParticleSystem.transform.localPosition = new Vector3(0, 0, 0);
            LastUsedParticleSystem.gameObject.SetActive(false);
        }
    }

    //-----------------------------------------------------------------------------------//

    [SerializeField]
    ParticleSystem PS_WhiteCrossFlash; //WhiteCrossFlash
    [SerializeField]
    ParticleSystem PS_FireCrossFlash; //FireCrossFlash
    [SerializeField]
    ParticleSystem PS_WhiteFlash; //WhiteFlash
    [SerializeField]
    ParticleSystem PS_WhiteCircleFlash; //WhiteCircleFlash
    [SerializeField]
    ParticleSystem PS_PurpleLineCircleFlash; //PurpleLineCircleFlash
    [SerializeField]
    ParticleSystem PS_SmokeCloud; //SmokeCloud
    [SerializeField]
    ParticleSystem PS_Leaves; //Leaves

    private enum ParticleEffects
    {
        WhiteCrossFlash,
        FireCrossFlash,
        WhiteFlash,
        WhiteCircleFlash,
        PurpleLineCircleFlash,
        SmokeCloud,
        Leaves,
    }

    private void SpawnEffect(ParticleEffects effect)
    {
        if (LastUsedParticleSystem != null)
        {
            LastUsedParticleSystem.transform.parent = gameObject.transform;
            LastUsedParticleSystem.transform.localPosition = new Vector3(0, 0, 0);
            LastUsedParticleSystem.gameObject.SetActive(false);
        }
        LastUsedParticleSystem = null;

        switch (effect)
        {
            case ParticleEffects.WhiteCrossFlash:
                LastUsedParticleSystem = PS_WhiteCrossFlash;
                break;
            case ParticleEffects.FireCrossFlash:
                LastUsedParticleSystem = PS_FireCrossFlash;
                break;
            case ParticleEffects.WhiteFlash:
                LastUsedParticleSystem = PS_WhiteFlash;
                break;
            case ParticleEffects.WhiteCircleFlash:
                LastUsedParticleSystem = PS_WhiteCircleFlash;
                break;
            case ParticleEffects.PurpleLineCircleFlash:
                LastUsedParticleSystem = PS_PurpleLineCircleFlash;
                break;
            case ParticleEffects.SmokeCloud:
                LastUsedParticleSystem = PS_SmokeCloud;
                break;
            case ParticleEffects.Leaves:
                LastUsedParticleSystem = PS_Leaves;
                break;
        }

        LastUsedParticleSystem.gameObject.SetActive(true);
        LastUsedParticleSystem.gameObject.transform.parent = gameObject.transform.parent;
        LastUsedParticleSystem.Play();
    }
    private void SpawnEffect(ParticleEffects effect, float Duration)
    {
        if (LastUsedParticleSystem != null)
        {
            LastUsedParticleSystem.transform.parent = gameObject.transform;
            LastUsedParticleSystem.transform.localPosition = new Vector3(0, 0, 0);
            LastUsedParticleSystem.gameObject.SetActive(false);
        }
        LastUsedParticleSystem = null;

        switch (effect)
        {
            case ParticleEffects.WhiteCrossFlash:
                LastUsedParticleSystem = PS_WhiteCrossFlash;
                break;
            case ParticleEffects.FireCrossFlash:
                LastUsedParticleSystem = PS_FireCrossFlash;
                break;
            case ParticleEffects.WhiteFlash:
                LastUsedParticleSystem = PS_WhiteFlash;
                break;
            case ParticleEffects.WhiteCircleFlash:
                LastUsedParticleSystem = PS_WhiteCircleFlash;
                break;
            case ParticleEffects.PurpleLineCircleFlash:
                LastUsedParticleSystem = PS_PurpleLineCircleFlash;
                break;
            case ParticleEffects.SmokeCloud:
                LastUsedParticleSystem = PS_SmokeCloud;
                break;
            case ParticleEffects.Leaves:
                LastUsedParticleSystem = PS_Leaves;
                break;
        }

        ParticleSystem.MainModule A = LastUsedParticleSystem.main;
        A.duration = Duration;
  
        LastUsedParticleSystem.gameObject.SetActive(true);
        LastUsedParticleSystem.gameObject.transform.parent = gameObject.transform.parent;
        LastUsedParticleSystem.Play();
    }
}
