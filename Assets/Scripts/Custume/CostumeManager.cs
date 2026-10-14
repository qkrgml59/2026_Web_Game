using System.Collections.Generic;
using UnityEngine;

public class CostumeManager : MonoBehaviour
{
    public static CostumeManager Instance { get; private set; }

    [Header("현재 보유 중인 의상 목록")]
    [SerializeField] private List<CostumeData> ownedCostumes = new List<CostumeData>();

    private HashSet<string> ownedCostumeIDs = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AcquireCostume(CostumeData costume)
    {
        if (costume == null)
        {
            Debug.LogWarning("[CostumeManager] 획득할 의상 데이터가 없습니다.");
            return false;
        }

        if (string.IsNullOrEmpty(costume.costumeID))
        {
            Debug.LogWarning($"[CostumeManager] '{costume.costumeName}' 의상의 costumeID가 설정되지 않았습니다.");
            return false;
        }

        if (ownedCostumeIDs.Contains(costume.costumeID))
        {
            Debug.Log($"[CostumeManager] 이미 보유 중인 의상입니다! (ID: {costume.costumeID}, 이름: {costume.costumeName})");
            return false;
        }


        ownedCostumeIDs.Add(costume.costumeID);
        ownedCostumes.Add(costume);
        Debug.Log($"[CostumeManager] 새로운 의상 획득 성공! (ID: {costume.costumeID}, 이름: {costume.costumeName})");
        return true;
    }

    public bool HasCostume(string costumeID)
    {
        return ownedCostumeIDs.Contains(costumeID);
    }

    public List<CostumeData> GetOwnedCostumes()
    {
        return ownedCostumes;
    }
}