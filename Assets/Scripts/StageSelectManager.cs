using UnityEngine;
using System.Collections;   
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageName;

    public void AssignStage(string stage) {
        if (stage == "Random") {
            stage = RandomizedStagePick();
        } 
        
        stageName.text = stage.ToUpper();
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
