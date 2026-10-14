using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCapsuleData", menuName = "IdolClicker/Capsule Data")]
public class CapsuleData : ScriptableObject
{
    [Header("Ä¸½¶ ±âº» Á¤º¸")]
    public string capsuleName = "±âº» Ä¸½¶";

    [Header("³»±¸µµ ¼³Á¤ (·£´ý ¹üÀ§)")]
    public int minDurability = 50;  
    public int maxDurability = 150; 

    [Header("µå·Ó ÀÇ»ó Á¤º¸")]
    public CostumeData costumeData; 

    [Header("Ä¸½¶ ·£´ý ÀÌ¹ÌÁö ÈÄº¸")]
    public List<Sprite> spriteCandidates = new List<Sprite>();

    public Sprite GetRandomSprite()
    {
        if (spriteCandidates == null || spriteCandidates.Count == 0)
        {
            return null; 
        }

        int randomIndex = Random.Range(0, spriteCandidates.Count);
        return spriteCandidates[randomIndex];
    }
}