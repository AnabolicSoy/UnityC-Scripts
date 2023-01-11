using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_DogeData : MonoBehaviour
{
    [SerializeField]
    Sprite DogeImg;
    [SerializeField]
    Sprite[] DogeAtackAnimations;
    [SerializeField]
    Gradient DogeSlashColor;

    public Sprite GetDogeImg()
    {
        return DogeImg;
    }
    public Sprite[] GetDogeAtackAnimations()
    {
        return DogeAtackAnimations;
    }
    public Gradient GetDogeSlashGradient()
    {
        return DogeSlashColor;
    }

}
