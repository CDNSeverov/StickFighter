using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
