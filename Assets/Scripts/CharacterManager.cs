using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] GameObject Brawler;
    [SerializeField] GameObject Swordsman;
    [SerializeField] GameObject Gunslinger;

    [SerializeField] Image player1HealthBar;
    [SerializeField] Image player2HealthBar;

    Vector3 player1SpawnPos = new Vector3(-3f, 0.8f, -2.5f);
    Vector3 player2SpawnPos = new Vector3(3f, 0.8f, -2.5f);
    Vector3 player1Rotation = new Vector3(0f, -90f, 0f);
    Vector3 player2Rotation = new Vector3(0f, 90f, 0f);

    void Awake()
    {
        SpawnCharacters();
    }

    void SpawnCharacters() {
        string p1Selection = GameData.player1Character;
        string p2Selection = GameData.player2Character;

        GameObject p1Prefab = GetPrefab(p1Selection);
        GameObject p2Prefab = GetPrefab(p2Selection);

        if (p1Prefab != null) {
            SpawnPlayer(p1Prefab, 1, player1SpawnPos, player1Rotation);
        }
        
        if (p2Prefab != null) {
            SpawnPlayer(p2Prefab, 2, player2SpawnPos, player2Rotation);
        }
    }

    GameObject GetPrefab(string characterName) {
        switch (characterName) {
            case "Brawler":
                return Brawler;
            case "Swordsman":
                return Swordsman;
            case "Gunslinger":
                return Gunslinger;
            default:
                Debug.LogWarning($"Unknown character: {characterName}");
                return null;
        }
    }

    void SpawnPlayer(GameObject prefab, int playerNumber, Vector3 spawnPos, Vector3 spawnRot) {
        GameObject player = Instantiate(prefab, spawnPos, Quaternion.Euler(spawnRot), this.transform);
        player.tag = playerNumber == 1 ? "Player1" : "Player2";

        HealthManagerScript hmanager = player.GetComponent<HealthManagerScript>();
        if (hmanager != null) {
            hmanager.healthBar = playerNumber == 1 ? player1HealthBar : player2HealthBar;
        }

        PlayerInput input = player.GetComponent<PlayerInput>();
        if (input == null) {
            return;
        }

        if (playerNumber == 1) {
            input.moveLeft   = KeyCode.A;
            input.moveRight  = KeyCode.D;
            input.jumpButton = KeyCode.W;
            input.attack     = KeyCode.F;
            input.special    = KeyCode.G;
        } else {
            input.moveLeft   = KeyCode.LeftArrow;
            input.moveRight  = KeyCode.RightArrow;
            input.jumpButton = KeyCode.UpArrow;
            input.attack     = KeyCode.Keypad1;
            input.special    = KeyCode.Keypad2;
        }
    }
}
