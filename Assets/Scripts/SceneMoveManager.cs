using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMoveManager : MonoBehaviour
{
    [Header("이동할 씬 이름 설정")]
    public string titleSceneName = "TitleScene";
    public string capsuleSceneName = "CapsuleScene";
    public string costumeSceneName = "CostumeScene";

    [Header("씬 이동 UI 버튼 연결")]
    public Button titleButton;
    public Button capsuleButton;
    public Button costumeButton;


    private static bool isSceneLoading = false;

    void Awake()
    {
        isSceneLoading = false; 
    }

    void Start()
    {
        if (titleButton != null)
            titleButton.onClick.AddListener(() => ChangeScene(titleSceneName));

        if (capsuleButton != null)
            capsuleButton.onClick.AddListener(() => ChangeScene(capsuleSceneName));

        if (costumeButton != null)
            costumeButton.onClick.AddListener(() => ChangeScene(costumeSceneName));
    }

    
    public void ChangeScene(string targetSceneName)
    {

        if (isSceneLoading) return;

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[SceneMoveManager] 이동할 씬 이름이 Inspector에 설정되지 않았습니다!");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == targetSceneName)
        {
            Debug.Log($"[SceneMoveManager] 이미 현재 씬({currentSceneName})에 위치해 있어 이동하지 않습니다.");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            isSceneLoading = true;
            Debug.Log($"[SceneMoveManager] {targetSceneName} 씬으로 이동합니다.");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"[SceneMoveManager] '{targetSceneName}' 씬이 Build Profiles (Build Settings)에 등록되어 있지 않거나 이름이 다릅니다!");
        }
    }

    public void SetNavigationButtonsInteractable(bool isInteractable)
    {
        if (titleButton != null) titleButton.interactable = isInteractable;
        if (capsuleButton != null) capsuleButton.interactable = isInteractable;
        if (costumeButton != null) costumeButton.interactable = isInteractable;
    }
}