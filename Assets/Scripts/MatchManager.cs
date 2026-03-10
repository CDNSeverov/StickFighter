using UnityEngine;
using System.Collections;   
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{
    [SerializeField] Image Round11;
    [SerializeField] Image Round12;
    [SerializeField] Image Round21;
    [SerializeField] Image Round22;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI countdownTimer;

    GameObject player1;
    GameObject player2;
    PlayerState player1State;
    PlayerState player2State;
    float player1Health;
    float player2Health;
    int player1RoundsWon = 0;
    int player2RoundsWon = 0;

    float currentTime;
    public float startingTime = 99f;

    bool roundInProgress = false;
    bool gameFinished = false;

    void Start()
    {
        player1 = GameObject.FindWithTag("Player1");
        player2 = GameObject.FindWithTag("Player2");
        player1State = player1.GetComponent<PlayerState>();
        player2State = player2.GetComponent<PlayerState>();

        StartCoroutine(GameStartRoutine());
    }

    IEnumerator GameStartRoutine() {
        ResetPlayers();

        player1State.StartingMatch();
        player2State.StartingMatch();

        yield return StartCoroutine(RoundStartTimer());
    }

    void Update()
    {
        if (!roundInProgress || gameFinished) {
            return;
        }
        
        if (player1RoundsWon == 2 || player2RoundsWon == 2) {
            EndGame();
        }


        player1Health = player1State.GetHealthFromManager();
        player2Health = player2State.GetHealthFromManager();
        currentTime -= 1 * Time.deltaTime;
        timerText.text = currentTime.ToString("0");

        if (currentTime <= 0) {
            if (player1Health > player2Health) {
                EndRound(1);
            } else {
                EndRound(2);
            }
        }

        if (player1Health <= 0f) {
            player1State.ResetHealthInManager();
            player2State.ResetHealthInManager();
            EndRound(2);
        } else if (player2Health <= 0f) {
            player1State.ResetHealthInManager();
            player2State.ResetHealthInManager();
            EndRound(1);
        }
    }

    private void EndRound(int winner) {
        if (!roundInProgress) {
            return;
        }

        roundInProgress = false;

        if (winner == 1) {
            player2State.Defeated();
            player1State.Won();

            AddRound(1);
            player1RoundsWon++;
        } else if (winner == 2) {
            player1State.Defeated();
            player2State.Won();

            AddRound(2);
            player2RoundsWon++;
        }

        StartCoroutine(RoundEndTimer());
    }

    private void AddRound(int player) {
        if (player == 1) {
            if (player1RoundsWon == 0) {
                Round11.color = new Color32(186, 172, 0, 255);
            } else if (player1RoundsWon == 1) {
                Round12.color = new Color32(186, 172, 0, 255);
            }
        } else if (player == 2) {
            if (player2RoundsWon == 0) {
                Round21.color = new Color32(186, 172, 0, 255);
            } else if (player2RoundsWon == 1) {
                Round22.color = new Color32(186, 172, 0, 255);
            }
        }
    }

    private IEnumerator RoundEndTimer() {
        if (player1RoundsWon == 2 || player2RoundsWon == 2) {
            EndGame();
            yield break;
        }

        yield return new WaitForSeconds(4f);
        
        ResetPlayers();

        yield return StartCoroutine(RoundStartTimer());
    }

    private IEnumerator RoundStartTimer() {
        countdownTimer.text = "3";
        yield return new WaitForSeconds(1f);
        countdownTimer.text = "2";
        yield return new WaitForSeconds(1f);
        countdownTimer.text = "1";
        yield return new WaitForSeconds(1f);
        countdownTimer.text = "FIGHT!";
        yield return new WaitForSeconds(0.2f);
        countdownTimer.text = "";

        roundInProgress = true;

        player1State.StartMatch();
        player2State.StartMatch();
        
        currentTime = startingTime;
    }

    void ResetPlayers() {
        player1State.ResetHealthInManager();
        player2State.ResetHealthInManager();

        player1State.ResetPosition(1);
        player2State.ResetPosition(2);
    }

    private void EndGame() {
        gameFinished = true;

        if (player1RoundsWon == 2) {
            countdownTimer.text = "PLAYER 1 WINS!";
            player2State.Defeated();
            player1State.Won();
            GameData.winner = 1;
        } else if (player2RoundsWon == 2) {
            countdownTimer.text = "PLAYER 2 WINS!";
            player1State.Defeated();
            player2State.Won();
            GameData.winner = 2;
        } 

        ChangeScene("EndScreen");
    }

    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
