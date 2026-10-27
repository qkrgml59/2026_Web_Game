using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterEquipManager : MonoBehaviour
{
    public static CharacterEquipManager Instance { get; private set; }

    [System.Serializable]
    public struct SlotUI
    {
        public CostumeSlot slot;
        public Image avatarImage; // 해당 씬 캐릭터의 의상/장신구 Image 컴포넌트
    }

    [Header("현재 씬의 캐릭터 부위별 UI Image 연결")]
    public List<SlotUI> characterSlots = new List<SlotUI>();

    // 장착 중인 의상/장신구 저장 (부위별 1개)
    private Dictionary<CostumeSlot, CostumeData> equippedCostumes = new Dictionary<CostumeSlot, CostumeData>();

    // 매력도 변경 이벤트
    public event Action<int> OnTotalCharmChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            // 다른 씬에서 넘어온 인스턴스가 있을 경우, 현재 씬에 배치된 Avatar Image 참조들을 기존 싱글톤으로 인계
            if (Instance.characterSlots != null && this.characterSlots != null && this.characterSlots.Count > 0)
            {
                Instance.characterSlots = new List<SlotUI>(this.characterSlots);
            }
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Start()
    {
        LoadEquipState();
    }

    // 씬이 새로 로드될 때 자동 실행 (CapsuleScene ↔ CostumeScene 이동 시 외형 최신화)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndBindSceneAvatarImages();
        UpdateCharacterAvatar();
    }

    // 씬 변경 시 참조가 누락된 경우 Canvas 내 Avatar Image 자동 검색 및 복구
    private void FindAndBindSceneAvatarImages()
    {
        for (int i = 0; i < characterSlots.Count; i++)
        {
            if (characterSlots[i].avatarImage == null)
            {
                // "Ch" 또는 "Avatar"라는 이름의 오브젝트 하위에서 Image 자동 탐색
                GameObject avatarObj = GameObject.Find("Ch") ?? GameObject.Find("Avatar");
                if (avatarObj != null)
                {
                    Image[] images = avatarObj.GetComponentsInChildren<Image>(true);
                    if (characterSlots[i].slot == CostumeSlot.Costume && images.Length > 0)
                    {
                        var updated = characterSlots[i];
                        updated.avatarImage = images[0];
                        characterSlots[i] = updated;
                    }
                    else if (characterSlots[i].slot == CostumeSlot.Accessory && images.Length > 1)
                    {
                        var updated = characterSlots[i];
                        updated.avatarImage = images[1];
                        characterSlots[i] = updated;
                    }
                }
            }
        }
    }

    // 특정 의상/장신구 장착
    public void Equip(CostumeData costume)
    {
        if (costume == null) return;

        equippedCostumes[costume.equipSlot] = costume; // 동일 부위 덮어쓰기

        UpdateCharacterAvatar();
        SaveEquipState();
        NotifyCharmChanged();

        Debug.Log($"[EquipManager] {costume.costumeName} 장착 완료! (부위: {costume.equipSlot})");
    }

    // 장착 해제
    public void Unequip(CostumeSlot slot)
    {
        if (equippedCostumes.ContainsKey(slot))
        {
            Debug.Log($"[EquipManager] {equippedCostumes[slot].costumeName} 해제 완료! (부위: {slot})");
            equippedCostumes.Remove(slot);

            UpdateCharacterAvatar();
            SaveEquipState();
            NotifyCharmChanged();
        }
    }

    // 장착 여부 확인
    public bool IsEquipped(CostumeData costume)
    {
        if (costume == null) return false;
        if (equippedCostumes.TryGetValue(costume.equipSlot, out CostumeData current))
        {
            return current != null && current.costumeID == costume.costumeID;
        }
        return false;
    }

    // 총 매력도 계산 (Null 방지 처리 완료)
    public int CalculateTotalCharm()
    {
        int totalCharm = 0;
        foreach (var pair in equippedCostumes)
        {
            if (pair.Value != null)
            {
                totalCharm += pair.Value.charmPoint;
            }
        }
        return totalCharm;
    }

    // 캐릭터 외형 UI 갱신 (스프라이트 적용 및 Alpha 보장)
    public void UpdateCharacterAvatar()
    {
        foreach (var slotUI in characterSlots)
        {
            if (slotUI.avatarImage == null) continue;

            if (equippedCostumes.TryGetValue(slotUI.slot, out CostumeData costume) && costume != null && costume.costumeSprite != null)
            {
                slotUI.avatarImage.sprite = costume.costumeSprite;

                // Color Alpha 값(투명도) 100% 보장
                Color color = slotUI.avatarImage.color;
                color.a = 1f;
                slotUI.avatarImage.color = color;

                slotUI.avatarImage.enabled = true;
                slotUI.avatarImage.gameObject.SetActive(true);
            }
            else
            {
                slotUI.avatarImage.gameObject.SetActive(false); // 미장착 시 비활성화
            }
        }
    }

    private void NotifyCharmChanged()
    {
        int currentTotalCharm = CalculateTotalCharm();
        Debug.Log($"[EquipManager] 현재 총 매력도: {currentTotalCharm}");
        OnTotalCharmChanged?.Invoke(currentTotalCharm);
    }

    // 장착 상태 저장 (PlayerPrefs)
    private void SaveEquipState()
    {
        foreach (CostumeSlot slot in Enum.GetValues(typeof(CostumeSlot)))
        {
            string key = $"EquippedSlot_{slot}";
            if (equippedCostumes.TryGetValue(slot, out CostumeData costume) && costume != null)
            {
                PlayerPrefs.SetString(key, costume.costumeID);
            }
            else
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        PlayerPrefs.Save();
    }

    // 저장된 장착 상태 불러오기
    private void LoadEquipState()
    {
        equippedCostumes.Clear();
        List<CostumeData> owned = CostumeManager.Instance != null ? CostumeManager.Instance.GetOwnedCostumes() : null;
        if (owned == null) return;

        foreach (CostumeSlot slot in Enum.GetValues(typeof(CostumeSlot)))
        {
            string key = $"EquippedSlot_{slot}";
            if (PlayerPrefs.HasKey(key))
            {
                string savedID = PlayerPrefs.GetString(key);
                CostumeData matchedCostume = owned.Find(c => c.costumeID == savedID);
                if (matchedCostume != null)
                {
                    equippedCostumes[slot] = matchedCostume;
                }
            }
        }

        UpdateCharacterAvatar();
        NotifyCharmChanged();
    }
}