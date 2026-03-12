using UnityEngine;
using System.Collections;   
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI player1CharacterText;
    [SerializeField] TextMeshProUGUI player2CharacterText;

    bool player1Selected = false;
    bool player2Selected = false;

    public void AssignPlayer(string character) {
        if (!player1Selected) {
            player1Selected = true;
            
            if (character == "Random") {
                character = RanomizedCharacterPick();
            }

            GameData.player1Character = character;
            player1CharacterText.text = character.ToUpper();
        } else if (!player2Selected) {
            player2Selected = true;

            if (character == "Random") {
                character = RanomizedCharacterPick();
            }

            GameData.player2Character = character;
            player2CharacterText.text = character.ToUpper();
        }
        
        StartCoroutine(StartTimer());
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
