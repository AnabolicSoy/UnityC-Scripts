using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DC_UIMDS : MonoBehaviour
{
   //------------------------------UI MANAGER DATA STORAGE---------------------------//
   public static DC_UIMDS UIMDS;
    DC_SpawnerSystem DC_SS;

    [SerializeField]
    float SlowMotionTimeScale;
    float FixelDetaTimeDefault;
    float TimeScaleDefault;

    private void Awake()
    {
        UIMDS = GetComponent<DC_UIMDS>();

        FixelDetaTimeDefault = Time.fixedDeltaTime;
        TimeScaleDefault = Time.timeScale;

        Time.timeScale = SlowMotionTimeScale;
        Time.fixedDeltaTime = FixelDetaTimeDefault * SlowMotionTimeScale;

       
    }  
    private void Start()
    {
        LoadData();
        Debug.Log(StoredCoins);
        StoredCoins_Txt.text = StoredCoins.ToString();
        DC_SS = DC_SpawnerSystem.DC_SS;
        UpdateDogeCharacterDisplay();
        UpdateBackgroudMapAndParticleSystems();
        SelectMap(SelectedMap);
    }
 
    public void CallStart()
    {
        DC_SS.StartUp(15);
        OpenCloseMainPanel(false);
        OpenCloseLowerPanel(false);
        
    }

    [SerializeField]
    DC_DogeData[] DogeDataArray;
    public DC_DogeData GetSelectedDogeData() 
    { return  DogeDataArray[SelectedDogeCharacter]; }


    [SerializeField]
    int SelectedDogeCharacter;
    public int GetSelectedDoge() 
    { return SelectedDogeCharacter; }


    [SerializeField]
    Text CoinsCounter;
    [SerializeField]
    Image CoinIcon;
    [SerializeField]
    Image CoinIconColor;
    float CoinEffectTimer;
    private IEnumerator CoinIconEffect()
    {
        while(CoinEffectTimer > 0)
        {
            CoinEffectTimer -= Time.deltaTime;

            CoinIcon.transform.localScale = new Vector3(1 + (0.25f * Mathf.Sin(CoinEffectTimer*2)), 1+ (0.25f * Mathf.Sin(CoinEffectTimer*2)), 1);

            CoinIconColor.color = new Color(1, 1, 1, Mathf.Sin(CoinEffectTimer * 2));
            yield return null;
        }
        CoinIcon.transform.localScale = new Vector3(1, 1, 1);
    }

    private int PlayerCoins;
    
    public bool AddRemoveCoins(int Delta)
    {
        bool Successful = false;

        if(PlayerCoins + Delta >= 0)
        {
            PlayerCoins += Delta;
            Successful = true;

            if(CoinEffectTimer <= 0 )
            {
                CoinEffectTimer = 0.5f;
                StartCoroutine(CoinIconEffect());

            }
            else
            {
                CoinEffectTimer += 0.5f;
            }

            CoinsCounter.text = PlayerCoins.ToString();
        }

        return Successful;
    }

    //---------------------------------------------------------------------//

    [SerializeField]
    Text StoredCoins_Txt;
   
    [SerializeField]
    int StoredCoins;                                                      //SAVE//

    public bool AddRemoveStorageCoins(int value)
    {
        bool a = false;
        if (StoredCoins + value >= 0)
        {
            StoredCoins += value;
            a = true;
        }

        StoredCoins_Txt.text = StoredCoins.ToString();


        return a;
    }

    public void TransferCoinsToStorage()
    {
        AddRemoveStorageCoins(PlayerCoins);
        AddRemoveCoins(-PlayerCoins);
        SaveData();
    }

    [SerializeField]
    bool[] UnlockedCharacters;                                            //SAVE//

    [SerializeField]
    Image[] DogeSlot_Mask;
    [SerializeField]
    Text[] DogeCharacterBTNText;
    [SerializeField]
    Image[] DogeCharacterImage;

    private void UpdateDogeCharacterDisplay()
    {
        for (int i = 0; i < DogeCharacterBTNText.Length; i++)
        {
            if(i == SelectedDogeCharacter) 
            {
                DogeSlot_Mask[i].color = Color.clear;
            }
            else
            {
                DogeSlot_Mask[i].color = new Color(0f,0f,0f,0.5f);
            }
            if(!UnlockedCharacters[i])
            {
                DogeCharacterBTNText[i].text = "Unlock " + CharacterCost[i].ToString() + "#";
            }
            else
            {
                DogeCharacterBTNText[i].text = "Select ";
            }
            if(SelectedDogeCharacter == i)
            {
                DogeCharacterBTNText[i].text = "Selected ";
            }
 
        }
    }

    [SerializeField]
    int[] CharacterCost;

    public void SelectDoge(int ID)
    {
        if(UnlockedCharacters[ID])
        {
            SelectedDogeCharacter = ID;
            UpdateDogeCharacterDisplay();
            DC_SS.SetUpCharacter();
        }
        else if(AddRemoveStorageCoins(-CharacterCost[ID]))
        {
            UnlockedCharacters[ID] = true;
            SelectedDogeCharacter = ID;
                UpdateDogeCharacterDisplay();
                DC_SS.SetUpCharacter();
                SaveData();
        }
    }

    //---------------------------UI------------------------//

    [SerializeField]
    GameObject UI_MainPanel;
    [SerializeField]
    GameObject UI_LowerPanel;

    public void OpenCloseMainPanel(bool OpenClose)
    {
        UI_MainPanel.SetActive(OpenClose);
    }
    public void OpenCloseLowerPanel(bool OpenClose)
    {
        UI_LowerPanel.SetActive(OpenClose);
    }


    //----------------------MAPS-------------------------//

    [SerializeField]
    GameObject[] MapParticleEffects;
    [SerializeField]
    GameObject[] MapImages;
    [SerializeField]
    GameObject[] TerrainImages;

    [SerializeField]
    Image[] MapSlot_Mask;

    [SerializeField]
    Text[] MapSlot_Text;

    int SelectedMap; public int GetSelectedMap() { return SelectedMap; }

    [SerializeField]
    bool[] UnlockedMaps;                      //SAVE

    [SerializeField]
    int[] MapCost;

    public void SelectMap(int ID)
    {
            if (UnlockedMaps[ID])
            {
                SelectedMap = ID;
                UpdateMapSlotDisplay(); UpdateBackgroudMapAndParticleSystems();
            }
            else if (AddRemoveStorageCoins(-MapCost[ID]))
            {
                SelectedMap = ID;
                UnlockedMaps[ID] = true;
                UpdateMapSlotDisplay(); UpdateBackgroudMapAndParticleSystems();
                SaveData();
            }
    }
    private void UpdateMapSlotDisplay()
    {
        for (int i = 0; i < UnlockedMaps.Length; i++)
        {
            if(i  == SelectedMap)
            {
                MapSlot_Mask[i].color = Color.clear;
                MapSlot_Text[i].text = "Selected";
            }
            else
            {
                if(UnlockedMaps[i])
                {
                    MapSlot_Text[i].text = "Select";
                }
                else
                {
                    MapSlot_Text[i].text = MapCost[i].ToString() + "#";
                }
            
                MapSlot_Mask[i].color = new Color(0, 0, 0, 0.5f);
            } 
        }
    }
    private void UpdateBackgroudMapAndParticleSystems()
    {
        for (int i = 0; i < MapParticleEffects.Length; i++)
        {
            if(i == SelectedMap)
            {
                MapParticleEffects[i].SetActive(true);
                 MapImages[i].SetActive(true);
                TerrainImages[i].SetActive(true);
            }
            else
            {
                MapImages[i].SetActive(false);
                TerrainImages[i].SetActive(false);
                MapParticleEffects[i].SetActive(false);
            }
        }
    }

    [SerializeField]
    GameObject[] Map_1NPCs; public GameObject[] GetMap_1_NPCs() { return Map_1NPCs; }
    [SerializeField]
    GameObject[] Map_2NPCs; public GameObject[] GetMap_2_NPCs() { return Map_2NPCs; }
    [SerializeField]
    GameObject[] Map_3NPCs; public GameObject[] GetMap_3_NPCs() { return Map_3NPCs; }

    //-----------------------------SaveSystem-----------------------//
    [SerializeField]
    string SavePath;
    private void SaveData()
    {
        Debug.Log("Saving");
        StreamWriter SW = new StreamWriter(Application.dataPath + SavePath);

        //Save Player Coins
        SW.WriteLine(StoredCoins.ToString());
        //Save Unlocked Maps
        for (int i = 0; i < UnlockedMaps.Length; i++)
        {
            if(UnlockedMaps[i])
            {
                SW.WriteLine("1");
            }
            else
            {
                SW.WriteLine("0");
            }
        }
        //Save Unlocked Doges
        for (int i = 0; i < UnlockedCharacters.Length; i++)
        {
            if (UnlockedCharacters[i])
            {
                SW.WriteLine("1");
            }
            else
            {
                SW.WriteLine("0");
            }
        }
        SW.Close();
    }
    private void LoadData()
    {
        Debug.Log("Loading");
        StreamReader SR = new StreamReader(Application.dataPath + SavePath);

        StoredCoins = int.Parse(SR.ReadLine());

        for (int i = 0; i < UnlockedMaps.Length; i++)
        {
            if(int.Parse(SR.ReadLine()) == 0)
            {
                UnlockedMaps[i] = false;
            }
            else
            {
                UnlockedMaps[i] = true;
            }
        }

        for (int i = 0; i < UnlockedCharacters.Length; i++)
        {
            if (int.Parse(SR.ReadLine()) == 0)
            {
                UnlockedCharacters[i] = false;
            }
            else
            {
                UnlockedCharacters[i] = true;
            }
        }

        SR.Close();
    }
}



