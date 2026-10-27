using UnityEngine;
using TMPro;

public class CharmDisplayUI : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI charmText;

    void OnEnable()
    {
        if (CharacterEquipManager.Instance != null)
        {
            CharacterEquipManager.Instance.OnTotalCharmChanged += UpdateCharmUI;
            UpdateCharmUI(CharacterEquipManager.Instance.CalculateTotalCharm());
        }
    }

    void OnDisable()
    {
        if (CharacterEquipManager.Instance != null)
        {
            CharacterEquipManager.Instance.OnTotalCharmChanged -= UpdateCharmUI;
        }
    }

    private void UpdateCharmUI(int totalCharm)
    {
        if (charmText != null)
        {
            charmText.text = $"매력도: {totalCharm}";
        }
    }

}
