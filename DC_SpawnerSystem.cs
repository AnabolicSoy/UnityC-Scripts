using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DC_SpawnerSystem : MonoBehaviour 
{
    //--------------------Main Functions------------------------//
    private void Awake()
    {
        DC_SS = GetComponent<DC_SpawnerSystem>();
        Get_NPC_Scale = NPC_Scale;
    }

    void Start()
    {
        UIMDS = DC_UIMDS.UIMDS;

        GameObject Obj_1 = Instantiate(ComboPS_Explosion_Combo_V, new Vector3(0, 0, 0), Quaternion.identity);
        ComboEffectV = Obj_1.GetComponent<ParticleSystem>();
        GameObject Obj_2 = Instantiate(ComboPS_Explosion_Combo_H, new Vector3(0, 0, 0), Quaternion.identity);
        ComboEffectH = Obj_2.GetComponent<ParticleSystem>();
        GameObject Obj_3 = Instantiate(ComboPS_Explosion_Combo_DP, new Vector3(0, 0, 0), Quaternion.identity);
        ComboEffectDP = Obj_3.GetComponent<ParticleSystem>();
        GameObject Obj_4 = Instantiate(ComboPS_Explosion_Combo_DN, new Vector3(0, 0, 0), Quaternion.identity);
        ComboEffectDN = Obj_4.GetComponent<ParticleSystem>();

        ComboDisplaySlots[0].color = Color.clear;
        ComboDisplaySlots[1].color = Color.clear;
        ComboDisplaySlots[2].color = Color.clear;

    }

    void Update()
    {
        if(Play)
        {

        Timer -= Time.deltaTime;
        if(Timer < 0)
        {
            Timer = Random.Range(SpawnTimeInterval_Min, SpawnTimeInterval_Max + 1); 
            
                if(Random.Range(0,5) == 0)
                {
                    if (Random.Range(0f, 1f) < BombSpawnChance)
                    {
                        GameObject Bomb = Instantiate(BombObj[Random.Range(0, BombObj.Length)], BombSpawns[1].position, Quaternion.identity);

                        GameObject Bomb2 = Instantiate(BombObj[Random.Range(0, BombObj.Length)], BombSpawns[0].position, Quaternion.identity);
                        
                    }
                }
                else
                {
                    if (Random.Range(0f, 1f) < BombSpawnChance)
                    {
                        GameObject Bomb = Instantiate(BombObj[Random.Range(0, BombObj.Length)], BombSpawns[Random.Range(0, 2)].position, Quaternion.identity);
                    }
                }
             

                Spawn();
                if(Random.Range(0,2) == 0)
                {
                    Spawn();
                }
        }

        if(Input.touchCount > 0)
        {
           if(IsSlashing)
           {
                //DragSlashObject
                pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                pos.z = 0;
                ActiveSlashObject.transform.position = pos;

           }
           else
           {
                //StartSlashing
                SpawnSlash();
           }
        }
        else
        {
            if(IsSlashing)
            {
                DoIdleAnim();
                //StopSlashing
                ActiveSlashObject.SetActive(false);
                IsSlashing = false;
                InactiveSlashPool.Add(ActiveSlashObject);
                VisibleSlashPool.Remove(ActiveSlashObject);
                End_Pos = pos;
                CalculateSlashLine();
            }
            else
            {
                //Idle
            }
        }

        }
    }


    //---------------------------------------------------------//



    public static DC_SpawnerSystem DC_SS;
    DC_UIMDS UIMDS;


    [SerializeField]
    float NPC_Scale;

    public static float Get_NPC_Scale;
    

    [SerializeField]
    Sprite[] ConversionSpawnsPepes; public Sprite GetRandomConversionSpawnPepes() { return ConversionSpawnsPepes[Random.Range(0, ConversionSpawnsPepes.Length)]; }

    [SerializeField]
    Sprite[] ConversionSpawnsFruit; public Sprite GetRandomConversionSpawnFruit() { return ConversionSpawnsFruit[Random.Range(0, ConversionSpawnsFruit.Length)]; }

    bool Play;

    public void StartUp(int numberOfSpawns)
    {
        Debug.Log("Start");
        HP = MaxHP;
        ChangeHP(0);
        IsSlashing = false;
        Play = true;

        switch (UIMDS.GetSelectedMap())
        {
            case 0:
                NPCs = UIMDS.GetMap_1_NPCs();
                break;
            case 1:
                NPCs = UIMDS.GetMap_2_NPCs();
                break;
            case 2:
                NPCs = UIMDS.GetMap_3_NPCs();
                break;
        }

        SetUpCharacter();

        ActiveObjects = new List<GameObject>();
        InactiveObjects = new List<GameObject>();
        List<int> ID = new List<int>();
        int NPC_ID;

        for (int i = 0; i < SpawnPoolSize; i++)
        {
            NPC_ID = Random.Range(0, NPCs.Length);
            while (ID.Contains(NPC_ID))
            {
                NPC_ID = Random.Range(0, NPCs.Length);
            }
            ID.Add(NPC_ID);
            GameObject NPC = Instantiate(NPCs[NPC_ID], SpawnPoint.position, Quaternion.identity);
            AddToInactive(NPC);
            NPC.transform.SetParent(SpawnPoint);
            NPC.transform.localScale = new Vector3(NPC_Scale, NPC_Scale, 1);

           
        }

        VisibleSlashPool = new List<GameObject>();
        InactiveSlashPool = new List<GameObject>();

        for (int i = 0; i < MaxSlashes; i++)
        {
            GameObject Slash = Instantiate(SlashObject, new Vector3(0, 1000, 0), Quaternion.identity);
            InactiveSlashPool.Add(Slash);
            Slash.SetActive(false);
        }

        SlashPartPool = new List<GameObject>();
        for (int i = 0; i < NPC_SlashPart_ObjectPoolCount; i++)
        {
            GameObject SlashPart = Instantiate(NPC_SlashPart, SpawnPoint.position, Quaternion.identity);
            SlashPartPool.Add(SlashPart);
            SlashPart.SetActive(false);

        }

        UpdatePowerMeter(0);

        this.NumberOfSpawns = numberOfSpawns;
        WojakPoolSize = numberOfSpawns;
        UpdateWojakPool();
    }

    public void Stop()
    {
        StopAllCoroutines();

        Play = false;
        for (int i = 0; i < InactiveSlashPool.Count; i++)
        {
            Destroy(InactiveSlashPool[i]);
        }
        for (int i = 0; i < VisibleSlashPool.Count; i++)
        {
            Destroy(VisibleSlashPool[i]);
        }
        for (int i = 0; i < SlashPartPool.Count; i++)
        {
            Destroy(SlashPartPool[i]);
        }
        for (int i = 0; i < ActiveObjects.Count; i++)
        {
            Destroy(ActiveObjects[i]);
        }
        for (int i = 0; i < InactiveObjects.Count; i++)
        {
            Destroy(InactiveObjects[i]);
        }

        ActiveObjects.Clear();
        InactiveObjects.Clear();
        InactiveSlashPool.Clear();
        VisibleSlashPool.Clear();
        SlashPartPool.Clear();

        UIMDS.OpenCloseMainPanel(true);
        UIMDS.OpenCloseLowerPanel(true);

        UIMDS.TransferCoinsToStorage();
    }

    //----------------------SLASH SYSTEM-------------------//
    [SerializeField]
    GameObject SlashObject;

    private void SetSlashObjectColor()
    {
        ParticleSystem.ColorOverLifetimeModule Ref = SlashObject.GetComponent<ParticleSystem>().colorOverLifetime;
        Ref.color = SelectedGradient;
    }

    List<GameObject> InactiveSlashPool;
    List<GameObject> VisibleSlashPool;

    Vector3 pos;

    Gradient SelectedGradient;

    [SerializeField]
    int MaxSlashes;

    bool IsSlashing;

    GameObject ActiveSlashObject;

    private void SpawnSlash()
    {
        if (InactiveSlashPool.Count > 0)
        {
            IsSlashing = true;
            ActiveSlashObject = InactiveSlashPool[0];
            VisibleSlashPool.Add(InactiveSlashPool[0]);
            InactiveSlashPool.RemoveAt(0);
            ActiveSlashObject.SetActive(true);
            pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
            pos.z = 0;
            ActiveSlashObject.transform.position = pos;
            Start_Pos = pos;

            StartCoroutine(DoSlash(.25f, ActiveSlashObject));
        }
    }

    private void CalculateSlashLine()
    {
        RaycastHit2D[] Hits = new RaycastHit2D[16];

        Hits = Physics2D.RaycastAll(Start_Pos, End_Pos - Start_Pos, Vector3.Distance(Start_Pos, End_Pos));

        End_Pos.z = 0;
        Start_Pos.z = 0;

        float angle = Mathf.Atan2( End_Pos.y - Start_Pos.y,End_Pos.x - Start_Pos.x);

        angle = Mathf.Rad2Deg * angle;

        if(angle < 0)
        {
            angle += 180;
        }

        DC_WojackScript.CutDirection Direction = DC_WojackScript.CutDirection.Vertical;

        if (angle >= 0 && angle < 30)
        {
            Direction = DC_WojackScript.CutDirection.Horizontal;
        }
        else if (angle >= 30 && angle <= 60)
        {
            Direction = DC_WojackScript.CutDirection.DiagonalPos;
        }
        else if (angle > 60 && angle < 120)
        {

            Direction = DC_WojackScript.CutDirection.Vertical;
        }
        else if (angle >= 120 && angle <= 150)
        {
            Direction = DC_WojackScript.CutDirection.DiagonalNeg;
        }
        else if (angle > 150 && angle < 210)
        {
            Direction = DC_WojackScript.CutDirection.Horizontal;
        }
        else if (angle >= 210 && angle <= 240)
        {
            Direction = DC_WojackScript.CutDirection.DiagonalPos;
        }
        else if (angle > 240 && angle < 300)
        {
            Direction = DC_WojackScript.CutDirection.Vertical;
            //vertical 
        }
        else if (angle >= 300 && angle <= 330)
        {
            Direction = DC_WojackScript.CutDirection.DiagonalNeg;
            //Diagonal neg
        }
        else
        {
            Direction = DC_WojackScript.CutDirection.Horizontal;
            //Horizontal
        }

        int HitsCounter = 0;
//        Debug.LogError(Vector3.Distance(Start_Pos, End_Pos));
        if(Vector3.Distance(Start_Pos, End_Pos) > 1)
        {
          for (int i = 0; i < Hits.Length; i++)
          {
            if(Hits[i] != null && Hits[i].transform.CompareTag("NPC"))
            {
                Hits[i].transform.GetComponent<DC_WojackScript>().CallHit(Direction, PowerCounter , Mathf.RoundToInt(Vector3.Distance(Start_Pos, End_Pos)), (End_Pos-Start_Pos).normalized);
                    LastHitNPC_Pos = Hits[i].transform.position;
                    switch (Direction)
                    {
                        case DC_WojackScript.CutDirection.Vertical:
                            AddComboValue(Combos.Vertical);
                            break;
                        case DC_WojackScript.CutDirection.Horizontal:
                            AddComboValue(Combos.Horizontal);
                            break;
                        case DC_WojackScript.CutDirection.DiagonalPos:
                            AddComboValue(Combos.DiagonalPos);
                            break;
                        case DC_WojackScript.CutDirection.DiagonalNeg:
                            AddComboValue(Combos.DiagonalNeg);
                            break;
                        case DC_WojackScript.CutDirection.Point:
                            AddComboValue(Combos.Null);
                            break;
                    }
                    HitsCounter ++;
            }
            else if(Hits[i] != null && Hits[i].transform.CompareTag("Bomb"))
            {
                Hits[i].transform.GetComponent<InteractableScr>().CallHit();
                   
                HitsCounter++;
            }
          }
        }

        if (HitsCounter > 1)
        {
            UpdatePowerMeter(HitsCounter*2);
            ComboDisplay[0].text = "Combo X " + HitsCounter.ToString();
            ComboDisplay[1].text = "Combo X " + HitsCounter.ToString();
        }
        else
        {
            UpdatePowerMeter(-PowerCounter);
            ComboDisplay[0].text = "";
            ComboDisplay[1].text = "";
        }
        
    }

    Vector3 Start_Pos;

    Vector3 End_Pos;

    [SerializeField]
    Image PowerMeter;

    int PowerCounter;

    [SerializeField]
    int MaxPower;

    Vector3 LastHitNPC_Pos;

    private void UpdatePowerMeter(int PowerChangeAmount)
    {
        PowerCounter += PowerChangeAmount;
        if(PowerCounter < 0)
        {
            PowerCounter = 0;
        }
        else if(PowerCounter > MaxPower)
        {
            PowerCounter = 0;
        }
        PowerMeter.fillAmount = ((float)PowerCounter / MaxPower);
    }

    //----------------------SPAWN SYSTEM-------------------//

    int NumberOfSpawns;

 
    GameObject[] NPCs;   //Map Dependant Value

    [SerializeField]
    Transform SpawnPoint;
    List<GameObject> ActiveObjects; public void AddToActive(GameObject Obj) { Obj.SetActive(true); ActiveObjects.Add(Obj); if (InactiveObjects.Contains(Obj)) { InactiveObjects.Remove(Obj); } }

    List<GameObject> InactiveObjects; public void AddToInactive(GameObject Obj) { Obj.SetActive(false); InactiveObjects.Add(Obj); if (ActiveObjects.Contains(Obj)) { ActiveObjects.Remove(Obj); } }

    [SerializeField]
    int SpawnPoolSize;    //Map Dependant Value

    [SerializeField]
    float SpawnTimeInterval_Min, SpawnTimeInterval_Max;     //Map Dependant Value

    float Timer;

    private void Spawn()
    {
        if (InactiveObjects.Count > 0 && NumberOfSpawns > 0)
        {

            SpawnNPC(5, InactiveObjects[0]);
            NumberOfSpawns--;

            UpdateWojakPool();
        }
        else if(NumberOfSpawns == 0)
        {
            UpdateWojakPool();
            StartCoroutine(DoExitRoutine());
        }

    }

    //---------------------DOGE Variants SYSTEM------------//

    [SerializeField]
    SpriteRenderer DogeSR;

    [SerializeField]
    Sprite[] AtackAnimations;

    [SerializeField]
    Sprite IdleAnimation;

    private void DoSwordCutAnim()
    {
        DogeSR.sprite = AtackAnimations[Random.Range(0, AtackAnimations.Length)];
    }

    private void DoIdleAnim()
    {
        DogeSR.sprite = IdleAnimation;
    }

    public void SetUpCharacter()
    {
        DC_DogeData Data = UIMDS.GetSelectedDogeData();
        DogeSR.sprite = Data.GetDogeImg();
        AtackAnimations = Data.GetDogeAtackAnimations();
        IdleAnimation = Data.GetDogeImg();
        SelectedGradient = Data.GetDogeSlashGradient();
        HP = MaxHP;
        SetSlashObjectColor();
    }

    //-----------------------PlayerHP--------------------------//

    [SerializeField]
    Image HP_BarFill;

    [SerializeField]
    int MaxHP;

    int HP;

    public void ChangeHP(int Amount)
    {
        HP += Amount;
        if(HP < 0)
        {
            ForceStop();
        }
        else if(HP > MaxHP)
        {
            HP = MaxHP;
        
        }

        HP_BarFill.fillAmount = (float)HP / MaxHP;

    }

    //-----------------------SLASHED OBJECTS--------------------------//

    [SerializeField]
    GameObject NPC_SlashPart;

    [SerializeField]
    int NPC_SlashPart_ObjectPoolCount;

    List<GameObject> SlashPartPool;

    public void AddObjectToSlashPool(GameObject Obj_1, GameObject Obj_2)
    {
        SlashPartPool.Add(Obj_1);
        SlashPartPool.Add(Obj_2);
    }

    public bool TryGetObjectsFromPool(out GameObject Obj_1, out GameObject Obj_2)
    {
        if (SlashPartPool.Count > 1)
        {
            Debug.Log("SlashPool > 2");
            Obj_1 = SlashPartPool[0];
            Obj_2 = SlashPartPool[1];
            Debug.Log("------------");
            SlashPartPool.RemoveAt(0);
            SlashPartPool.RemoveAt(0);
            return true;
        }
        else
        {
            Debug.Log("SlashPool < 2");
            Obj_1 = null;
            Obj_2 = null;

            return false;
        }
    }

    //----------------Display---------------------------//

    [SerializeField]
    TextMesh[] ComboDisplay;

    [SerializeField]
    Text WojakPoolCounterText;
    int WojakPoolSize;
    private void UpdateWojakPool()
    {
        WojakPoolCounterText.text = NumberOfSpawns + "/" + WojakPoolSize;
    }

    //---------------------ComboSystem--------------------//

    private enum Combos
    {
        Null,
        Horizontal,
        Vertical, 
        DiagonalPos,
        DiagonalNeg,
    }
    [SerializeField]
    Image[] ComboDisplaySlots;
    int DefinedComboSpriteID;
    [SerializeField]
    Sprite[] ComboIcons; //V H DP DN
    private Combos DefinedComboType;
    int ComboCount;

    private void AddComboValue(Combos combo)
    {
            if (ComboCount == 0 && combo != Combos.Null)
            {
                DefinedComboType = combo;
            
                switch (DefinedComboType)
                {
                    case Combos.Horizontal:
                    DefinedComboSpriteID = 1;
                        break;
                    case Combos.Vertical:
                    DefinedComboSpriteID = 0;
                        break;
                    case Combos.DiagonalPos:
                    DefinedComboSpriteID = 2;
                        break;
                    case Combos.DiagonalNeg:
                    DefinedComboSpriteID = 3;
                        break;
                }

                ComboDisplaySlots[ComboCount].sprite = ComboIcons[DefinedComboSpriteID];
                ComboDisplaySlots[ComboCount].color = new Color(1, 1, 1, 0.4f);
                ComboCount++;
            }
            else if (ComboCount < 3)
            {
               if(combo == DefinedComboType)
               {
                  ComboDisplaySlots[ComboCount].sprite = ComboIcons[DefinedComboSpriteID];
                  ComboDisplaySlots[ComboCount].color = new Color(1, 1, 1, 0.4f);
                  ComboCount++;
               }
               else
               {
                ComboCount = 0;
                ComboDisplaySlots[0].color = Color.clear;
                ComboDisplaySlots[1].color = Color.clear;
                ComboDisplaySlots[2].color = Color.clear;
               }
            }
            else if(ComboCount >= 3)
            {
               if (combo == DefinedComboType)
               {
                  ComboCount = 0;
                  CallComboHit();
                  ComboDisplaySlots[0].color = Color.clear;
                  ComboDisplaySlots[1].color = Color.clear;
                  ComboDisplaySlots[2].color = Color.clear;
            }
               else
               {
                  ComboCount = 0;
                  ComboDisplaySlots[0].color = Color.clear;
                  ComboDisplaySlots[1].color = Color.clear;
                  ComboDisplaySlots[2].color = Color.clear;
               }

            }
    }

    [SerializeField]
    GameObject ComboPS_Explosion_Combo_H;
    [SerializeField]
    GameObject ComboPS_Explosion_Combo_V;
    [SerializeField]
    GameObject ComboPS_Explosion_Combo_DP;
    [SerializeField]
    GameObject ComboPS_Explosion_Combo_DN;

    ParticleSystem ComboEffectV;
    ParticleSystem ComboEffectH;
    ParticleSystem ComboEffectDP;
    ParticleSystem ComboEffectDN;

    private void CallComboHit()
    {
        LastHitNPC_Pos.z = 0;
        switch (DefinedComboType)
        {
            case Combos.Horizontal:
                ComboEffectH.transform.position = LastHitNPC_Pos;
                ComboEffectH.Play();
                break;
            case Combos.Vertical:
                ComboEffectV.transform.position = LastHitNPC_Pos;
                ComboEffectV.Play();
                break;
            case Combos.DiagonalPos:
                ComboEffectDP.transform.position = LastHitNPC_Pos;
                ComboEffectDP.Play();
                break;
            case Combos.DiagonalNeg:
                ComboEffectDN.transform.position = LastHitNPC_Pos;
                ComboEffectDN.Play();
                break;
        }
    }

    //------------------CO-Routines-------------------//

    private void SpawnNPC(float timer, GameObject obj)
    {
        Debug.Log("SpawnNPC");
        obj.transform.position = SpawnPoint.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1f, 1f), 0);

        AddToActive(obj);


        obj.GetComponent<DC_WojackScript>().ActivateNPC();



    }

    IEnumerator DoSlash(float timer,GameObject SlashObj)
    {
        DoSwordCutAnim();
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            yield return null;
        }
        if (IsSlashing & ActiveSlashObject == SlashObj )
        {
            ActiveSlashObject.SetActive(false);
            IsSlashing = false;
            InactiveSlashPool.Add(ActiveSlashObject);
            VisibleSlashPool.Remove(ActiveSlashObject);
            End_Pos = pos;
            CalculateSlashLine();
            DoIdleAnim();
        }
    }


    public void CallSlashCoRoutine(float timer, GameObject SlashObj_1, GameObject SlashObj_2)
    {
        StartCoroutine(SlashRoutine(timer, SlashObj_1, SlashObj_2));
    }
     IEnumerator SlashRoutine(float timer, GameObject SlashObj_1, GameObject SlashObj_2)
    {

        Debug.Log("SlashRoutine");


        SlashObj_1.SetActive(true);
        SlashObj_2.SetActive(true);

        float Timer = timer;
        while (Timer > 0)
        {

            Timer -= Time.deltaTime;
         
            yield return null;

        }

        SlashObj_1.SetActive(false);
        SlashObj_2.SetActive(false);

        AddObjectToSlashPool(SlashObj_1, SlashObj_2);
        Debug.Log("CoRoutineEnd");


    }


    public void DoWeaponDropCoRoutine(GameObject Weapon, GameObject Shield)
    {
        Debug.Log("WeapCoRot");
        if(Weapon != null && Shield != null)
        {
            StartCoroutine(WeaponDropRoutine(1, Weapon, Shield));
        }
        else if(Weapon == null && Shield != null)
        {
            StartCoroutine(WeaponDropRoutine(1, null, Shield));
        }
        else if (Weapon != null && Shield == null)
        {
            StartCoroutine(WeaponDropRoutine(1, Weapon, null));
        }

    }

    IEnumerator WeaponDropRoutine(float Timer, GameObject Weapon, GameObject Shield)
    {
        SpriteRenderer SR_1 = null, SR_2 = null;
        if(Weapon!= null)
        SR_1 = Weapon.GetComponent<SpriteRenderer>();
        if(Shield != null)
        SR_2 = Shield.GetComponent<SpriteRenderer>();


        while (Timer > 0)
        {
            Timer -= Time.deltaTime;
            if(SR_1 != null)
            SR_1.color = new Color(1, 1, 1, SR_1.color.a - Time.deltaTime);
            
            if(SR_2 != null)
            SR_2.color = new Color(1, 1, 1, SR_1.color.a - Time.deltaTime);
            yield return null;
        }

        if (Weapon != null)
        {
            Weapon.SetActive(false);
        }
        if (Shield != null)
        {
            Shield.SetActive(false);
        }
    }

    IEnumerator DoExitRoutine()
    {
        Debug.LogError("DoingExitRoutine");
        float ExitTime = 3;

        while(ExitTime > 0)
        {
            ExitTime -= Time.deltaTime;

            yield return null;
        }
        Debug.LogError("Waiting for wojaks to despawn");
        while (!NPCPoolIsEmpty())
            yield return null;
        Debug.LogError("PoolIsEmpty");

        Stop();
    }
    private bool NPCPoolIsEmpty()
    {
        if(ActiveObjects.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //--------------BombSpawn---------------------//

    [SerializeField]
    float BombSpawnChance;

    [SerializeField]
    GameObject[] BombObj;

    [SerializeField]
    Transform[] BombSpawns;



    public void ForceStop()
    {
        Stop();
    }


}






