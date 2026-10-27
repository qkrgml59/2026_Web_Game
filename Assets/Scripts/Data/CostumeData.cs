using UnityEngine;
using System.Collections.Generic;

public enum CostumeSlot
{
    Costume,     //의상
    Accessory    //장신구
}

public enum CostumeRarity
{
    Common,      //일반
    Rare,        //희귀
    Epic,        //에픽
    Unique,      //유니크
    Legendary    //전설
}
[CreateAssetMenu(fileName = "NewCostumeData", menuName = "IdolClicker/Costume Data")]
public class CostumeData : ScriptableObject
{
    [Header("기본 정보")]
    public string costumeID = "Costume_001";         //고유 ID
    public string costumeName = "기본 의상";         //의상 이름

    [Header("장착 및 능력치 정보")]
    public CostumeSlot equipSlot = CostumeSlot.Costume;     //장착 부위
    public CostumeRarity rarity = CostumeRarity.Common;
    public int charmPoint = 1;

    [Header("의상 이미지")]
    public Sprite costumeSprite;
}