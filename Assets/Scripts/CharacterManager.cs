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

    [SerializeField] Material Brawler2Material;
    [SerializeField] Material Swordsman2Material;
    [SerializeField] Material Gunslinger2Material;

    bool changeMat = false;

    void Awake()
    {
        SpawnCharacters();
    }

    void SpawnCharacters() {
        string p1Selection = GameData.player1Character;
        string p2Selection = GameData.player2Character;

        if (p1Selection == p2Selection) {
            changeMat = true;
        }

        GameObject p1Prefab = GetPrefab(p1Selection);
        GameObject p2Prefab = GetPrefab(p2Selection);

        if (p1Prefab != null) {
            SpawnPlayer(p1Prefab, 1, player1SpawnPos, player1Rotation, false, p1Selection);
        }

        if (p2Prefab != null) {
            SpawnPlayer(p2Prefab, 2, player2SpawnPos, player2Rotation, changeMat, p2Selection);
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

    void SpawnPlayer(GameObject prefab, int playerNumber, Vector3 spawnPos, Vector3 spawnRot, bool changeMaterial, string charName) {
        GameObject player = Instantiate(prefab, spawnPos, Quaternion.Euler(spawnRot), this.transform);
        player.tag = playerNumber == 1 ? "Player1" : "Player2";

        if (changeMaterial) {
            Material player2Material;
            
            switch(charName) {
                case "Brawer":
                    player2Material = Brawler2Material;
                    break;
                case "Swordsman":
                    player2Material = Swordsman2Material;
                    break;
                case "Gunslinger":
                    player2Material = Gunslinger2Material;
                    break;
                default:
                    player2Material = Brawler2Material;
                    break;
            }

            ChangeBodyMeshMaterial(player, player2Material);
        }

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

    void ChangeBodyMeshMaterial(GameObject player, Material newMaterial) {
        Transform bodyMesh = null;

        Transform graphics = player.transform.Find("Graphics");
        if (graphics != null) {
            foreach (Transform child in graphics.GetComponentsInChildren<Transform>()) {
                if (child.name == "BodyMesh") {
                    bodyMesh = child;
                    break;
                }
            }
        }

        if (bodyMesh == null) {
            Debug.LogWarning("BodyMesh not found under Graphics!");
            return;
        }

        SkinnedMeshRenderer smr = bodyMesh.GetComponent<SkinnedMeshRenderer>();
        if (smr != null) {
            smr.material = newMaterial;
        }
    }   
}
