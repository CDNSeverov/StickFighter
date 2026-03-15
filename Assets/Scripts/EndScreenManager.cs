using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winnerText;

    [SerializeField] Image winnerImage;
    
    [SerializeField] Sprite brawlerArt;
    [SerializeField] Sprite swordsmanArt;
    [SerializeField] Sprite gunslingerArt;

    void Start()
    {
        if (GameData.winner == 1) {
            winnerText.text = "PLAYER 1 WINS!";
            AssignImage(GameData.player1Character);
        } else if (GameData.winner == 2) {
            winnerText.text = "PLAYER 2 WINS!";
            AssignImage(GameData.player2Character);
        }
    }

    private void AssignImage(string character) {
        if (character == "Brawler") {
            winnerImage.sprite = brawlerArt;
        } else if (character == "Swordsman") {
            winnerImage.sprite = swordsmanArt;
        } else if (character == "Gunslinger") {
            winnerImage.sprite = gunslingerArt;
        }
    }

    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
