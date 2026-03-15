using UnityEngine;
using System.Collections;   
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI player1CharacterText;
    [SerializeField] TextMeshProUGUI player2CharacterText;

    [SerializeField] Image player1Image;
    [SerializeField] Image player2Image;

    [SerializeField] Sprite brawlerArt;
    [SerializeField] Sprite swordsmanArt;
    [SerializeField] Sprite gunslingerArt;

    bool player1Selected = false;
    bool player2Selected = false;

    public void AssignPlayer(string character) {
        if (player1Selected && player2Selected) {
            return;
        }

        if (!player1Selected) {
            player1Selected = true;
            
            if (character == "Random") {
                character = RanomizedCharacterPick();
            }

            GameData.player1Character = character;
            player1CharacterText.text = character.ToUpper();
            AssignImage(character, player1Image);
        } else if (!player2Selected) {
            player2Selected = true;

            if (character == "Random") {
                character = RanomizedCharacterPick();
            }

            GameData.player2Character = character;
            player2CharacterText.text = character.ToUpper();
            AssignImage(character, player2Image);
        }
        
        StartCoroutine(StartTimer());
    }

    private void AssignImage(string character, Image playerImage) {
        if (character == "Brawler") {
            playerImage.sprite = brawlerArt;
        } else if (character == "Swordsman") {
            playerImage.sprite = swordsmanArt;
        } else if (character == "Gunsinger") {
            playerImage.sprite = gunslingerArt;
        }
    }

    private string RanomizedCharacterPick() {
        int rnd = Random.Range(1,3);

        switch (rnd) {
            case 1: 
                return "Brawler";
            case 2: 
                return "Swordsman";
            case 3: 
                return "Gunslinger";
            default:
                return "Brawler";
        }
    }

    private IEnumerator StartTimer() {
        yield return new WaitForSeconds(3f);

        ChangeScene("StageSelect");
    }

    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
