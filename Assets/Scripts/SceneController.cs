
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonController : MonoBehaviour
{
    public void GoToCapsule()
    {
        SceneManager.LoadScene("CapsuleScene");
    }

    public void GoToDressUp()
    {
        SceneManager.LoadScene("CostumeScene");
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
