using UnityEngine;
using UnityEngine.UI;

public class CostumeItemUI : MonoBehaviour
{
    [Header("UI 연결")]
    public Image costumeImage;          //의상 아이콘 이미지
    public Image highlightBorder;       //선택 하이라이트
    public GameObject equippedBadge;    //[장착 중] 표시 뱃지 UI
    public Button itemButton;           //항목 선택 버튼

    public CostumeData Data { get; private set; }
    private System.Action<CostumeItemUI> onClickCallback;

    public void Setup(CostumeData costumeData, System.Action<CostumeItemUI> onClick)
    {
        Data = costumeData;
        onClickCallback = onClick;

        if (costumeImage != null)
        {
            if (costumeData != null && costumeData.costumeSprite != null)
            {
                costumeImage.sprite = costumeData.costumeSprite;
                costumeImage.gameObject.SetActive(true);
            }
            else
            {
                costumeImage.gameObject.SetActive(false);
            }
        }

        SetSelected(false);
        UpdateEquipState();

        if(itemButton !=null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(OnItemClicked);
        }
    }

    private void OnItemClicked()
    {
        onClickCallback?.Invoke(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (highlightBorder != null)
        {
            highlightBorder.gameObject.SetActive(isSelected);
        }
    }

    public void UpdateEquipState()
    {
        if (equippedBadge != null)
        {
            bool isEquipped = CharacterEquipManager.Instance != null && CharacterEquipManager.Instance.IsEquipped(Data);
            equippedBadge.SetActive(isEquipped);
        }
    }

}
