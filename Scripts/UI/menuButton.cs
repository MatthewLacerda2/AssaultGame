using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuButton : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        Dropdown drop = GetComponent<Dropdown>();
        if (drop != null) {
            drop.value = QualitySettings.GetQualityLevel();
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void LoadLevelByName(string stringa) {
        if (stringa == "MainMenu") {
            ProbeSystem.isChapterFirstLoad = true;
        }

        ProbeSystem.isSceneFirstLoad = true;

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(stringa);
    }

    public void QuitTheFuckingGame() {
        Application.Quit();
    }

    public void QualitySettingsChange(int index) {
        QualitySettings.SetQualityLevel(index);
    }
}