using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winnerText;

    void Start()
    {
        if (GameData.winner == 1) {
            winnerText.text = "PLAYER 1 WINS!";
        } else if (GameData.winner == 2) {
            winnerText.text = "PLAYER 2 WINS!";
        }
    }

    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
