using UnityEngine;
using System.Collections;   
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageName;

    [SerializeField] Image stageArt;

    [SerializeField] Sprite testStageArt;
    [SerializeField] Sprite seaStageArt;
    [SerializeField] Sprite natureStageArt;

    public void AssignStage(string stage) {
        if (stage == "Random") {
            stage = RandomizedStagePick();
        } 
        
        stageName.text = stage.ToUpper();

        if (stage == "TestStage") {
            stageArt.sprite = testStageArt;
        } else if (stage == "SeaStage") {
            stageArt.sprite = seaStageArt;
        } else if (stage == "NatureStage") {
            stageArt.sprite = natureStageArt;
        }

        StartCoroutine(StartTimer(stage));
    }

    private string RandomizedStagePick() {
        int rnd = Random.Range(1,3);

        switch (rnd) {
            case 1: 
                return "TestStage";
            case 2: 
                return "SeaStage";
            case 3: 
                return "NatureStage";
            default:
                return "TestStage";
        }
    }

    private IEnumerator StartTimer(string stage) {
        yield return new WaitForSeconds(3f);

        ChangeScene(stage);
    }
    
    public void ChangeScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
