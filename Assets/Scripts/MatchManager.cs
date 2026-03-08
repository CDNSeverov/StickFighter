using UnityEngine;
using System.Collections;   
using UnityEngine.UI;
using TMPro;

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
    float player1RoundsWon;
    float player2RoundsWon;

    float currentTime;
    public float startingTime = 99f;

    bool roundInProgress;

    void Start()
    {
        player1 = GameObject.FindWithTag("Player1");
        player2 = GameObject.FindWithTag("Player2");
        player1State = player1.GetComponent<PlayerState>();
        player2State = player2.GetComponent<PlayerState>();
        currentTime = startingTime;
        roundInProgress = true;
    }

    void Update()
    {
        if (!roundInProgress) {
            return;
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

        if (player1RoundsWon == 2 || player2RoundsWon == 2) {
            EndGame();
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

    private void EndRound(int player) {
        roundInProgress = false;

        if (player == 1) {
            player2State.Defeated();
            player1State.Won();

            AddRound(1);
        } else if (player == 2) {
            player1State.Defeated();
            player2State.Won();

            AddRound(2);
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

            player1RoundsWon++;
        } else if (player == 2) {
            if (player2RoundsWon == 0) {
                Round21.color = new Color32(186, 172, 0, 255);
            } else if (player2RoundsWon == 1) {
                Round22.color = new Color32(186, 172, 0, 255);
            }
            player2RoundsWon++;
        }
    }

    private void StartRound() {
        StartCoroutine(RoundStartTimer());

    }

    private IEnumerator RoundEndTimer() {
        yield return new WaitForSeconds(4f);
        
        StartRound();
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

        player1State.ResetPosition(1);
        player2State.ResetPosition(2);
        
        currentTime = startingTime;
    }

    private void EndGame() {

    }
}
