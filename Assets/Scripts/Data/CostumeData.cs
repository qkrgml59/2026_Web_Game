using UnityEngine;

[CreateAssetMenu(fileName = "NewCostumeData", menuName = "IdolClicker/Costume Data")]
public class CostumeData : ScriptableObject
{
    [Header("의상 기본 정보")]
    public string costumeID;      
    public string costumeName;    
    public Sprite costumeSprite;  
}