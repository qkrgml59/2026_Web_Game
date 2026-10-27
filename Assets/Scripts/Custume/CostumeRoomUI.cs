using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostumeRoomUI : MonoBehaviour
{
    [Header("목록 UI 연결")]
    public Transform contentParent;
    public GameObject costumeItemPrefab;
    public TextMeshProUGUI emptyNoticeText;

    [Header("상세 정보 UI 연결")]
    public Image detailCostumeImage;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailRarityText;
    public TextMeshProUGUI detailCharmText;
    public Button equipButton;
    public TextMeshProUGUI equipButtonText;

    private List<CostumeItemUI> spawnedItems = new List<CostumeItemUI>();
    private CostumeItemUI currentSelectedItem = null;

    void Start()
    {
        if (equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(OnClickEquipButton);
        }

        RefreshCostumeList();
    }

    public void RefreshCostumeList()
    {
        ClearList();

        List<CostumeData> ownedList = CostumeManager.Instance != null ? CostumeManager.Instance.GetOwnedCostumes() : null;

        if (ownedList == null || ownedList.Count == 0)
        {
            if (emptyNoticeText != null)
            {
                emptyNoticeText.gameObject.SetActive(true);
                emptyNoticeText.text = "아직 획득한 의상이 없습니다.";
            }
            ClearDetailPanel();
            return;
        }

        if (emptyNoticeText != null)
        {
            emptyNoticeText.gameObject.SetActive(false);
        }

        for (int i = 0; i < ownedList.Count; i++)
        {
            CostumeData data = ownedList[i];
            if (data == null) continue;

            GameObject itemObj = Instantiate(costumeItemPrefab, contentParent);
            CostumeItemUI itemUI = itemObj.GetComponent<CostumeItemUI>();

            if (itemUI != null)
            {
                itemUI.Setup(data, OnSelectCostumeItem);
                spawnedItems.Add(itemUI);
            }
        }

        if (spawnedItems.Count > 0)
        {
            OnSelectCostumeItem(spawnedItems[0]);
        }
    }

    private void OnSelectCostumeItem(CostumeItemUI selectedUI)
    {
        if (selectedUI == null || selectedUI.Data == null) return;

        if (currentSelectedItem != null)
        {
            currentSelectedItem.SetSelected(false);
        }

        currentSelectedItem = selectedUI;
        currentSelectedItem.SetSelected(true);

        UpdateDetailPanel(selectedUI.Data);
    }

    private void UpdateDetailPanel(CostumeData data)
    {
        if (detailNameText != null)
            detailNameText.text = string.IsNullOrEmpty(data.costumeName) ? "이름 없음" : data.costumeName;

        if (detailRarityText != null)
            detailRarityText.text = $"등급: {data.rarity}";

        if (detailCharmText != null)
            detailCharmText.text = $"매력도: +{data.charmPoint}";

        if (detailCostumeImage != null)
        {
            if (data.costumeSprite != null)
            {
                detailCostumeImage.sprite = data.costumeSprite;
                detailCostumeImage.gameObject.SetActive(true);
            }
            else
            {
                detailCostumeImage.gameObject.SetActive(false);
            }
        }

        UpdateEquipButtonUI(data);
    }

    private void UpdateEquipButtonUI(CostumeData data)
    {
        if (equipButton == null) return;

        bool isEquipped = CharacterEquipManager.Instance != null && CharacterEquipManager.Instance.IsEquipped(data);
        if (equipButtonText != null)
        {
            equipButtonText.text = isEquipped ? "장착 해제" : "장착하기";
        }
    }

    private void OnClickEquipButton()
    {
        if (currentSelectedItem == null || currentSelectedItem.Data == null) return;

        CostumeData data = currentSelectedItem.Data;

        if (CharacterEquipManager.Instance != null)
        {
            bool isEquipped = CharacterEquipManager.Instance.IsEquipped(data);

            if (isEquipped)
            {
                CharacterEquipManager.Instance.Unequip(data.equipSlot);
            }
            else
            {
                CharacterEquipManager.Instance.Equip(data);
            }

            // 1. 아이템 항목들의 [장착 중] 뱃지 UI 갱신
            foreach (var item in spawnedItems)
            {
                if (item != null) item.UpdateEquipState();
            }

            // 2. 상세 정보창 [장착하기]/[장착 해제] 버튼 글자 갱신
            UpdateEquipButtonUI(data);
        }
    }

    private void ClearDetailPanel()
    {
        if (detailNameText != null) detailNameText.text = "";
        if (detailRarityText != null) detailRarityText.text = "";
        if (detailCharmText != null) detailCharmText.text = "";
        if (detailCostumeImage != null) detailCostumeImage.gameObject.SetActive(false);
        if (equipButton != null) equipButton.gameObject.SetActive(false);
    }

    private void ClearList()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null) Destroy(spawnedItems[i].gameObject);
        }
        spawnedItems.Clear();
        currentSelectedItem = null;
    }
}