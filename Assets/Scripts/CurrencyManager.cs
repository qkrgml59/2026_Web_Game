using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("UI 연결")]
    public TextMeshProUGUI goldText;

    private int currentGold = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 바뀔 때마다 UI 자동 다시 연결
            LoadGold();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // 씬이 새로 로드될 때 실행되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. 만약 UI 참조가 끊어졌거나 없으면 씬에서 "CoinText"라는 이름의 TextMeshProUGUI 자동 검색
        if (goldText == null)
        {
            FindAndBindCoinText();
        }

        // 2. 새로운 씬 UI에 현재 골드 즉시 표시!
        UpdateGoldUI();
    }

    // 씬 내의 CoinText UI 자동 찾기
    private void FindAndBindCoinText()
    {
        GameObject coinObj = GameObject.Find("CoinText");
        if (coinObj != null)
        {
            goldText = coinObj.GetComponent<TextMeshProUGUI>();
        }
    }

    public int GetGold() => currentGold;

    // 골드 추가
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        currentGold += amount;
        UpdateGoldUI();
        SaveGold();
    }

    // 골드 차감
    public bool TrySpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            SaveGold();
            return true;
        }
        return false;
    }

    // UI 텍스트 갱신 (Null 및 Missing 안전 보장)
    public void UpdateGoldUI()
    {
        if (goldText == null)
        {
            FindAndBindCoinText();
        }

        if (goldText != null)
        {
            goldText.text = currentGold.ToString("N0");
        }
    }

    private void SaveGold()
    {
        PlayerPrefs.SetInt("User_Gold", currentGold);
        PlayerPrefs.Save();
    }

    private void LoadGold()
    {
        currentGold = PlayerPrefs.GetInt("User_Gold", 0);
        UpdateGoldUI();
    }
}