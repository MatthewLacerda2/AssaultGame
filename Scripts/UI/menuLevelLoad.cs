using UnityEngine;
using System.Collections;
using TMPro;

public class menuLevelLoad : MonoBehaviour {

    public AnimationCurve anime;
    public GameObject trilhaSonora;

    public TextMeshProUGUI level;
    public TextMeshProUGUI levelAdjective;

    // Start is called before the first frame update
    void Start() {
        if (ProbeSystem.isChapterFirstLoad) {
            Instantiate(trilhaSonora, lendaLib.cameraPos, Quaternion.identity);

            ProbeSystem.isChapterFirstLoad = false;
        }

        //É melhor fazer no Start pra não dar RaceCondition com outros scripts
        if (ProbeSystem.isSceneFirstLoad) {
            StartCoroutine(rotina());

            ProbeSystem.isSceneFirstLoad = false;
        } else {
            gameObject.SetActive(false);
        }

        levelAdjective.text = lendaLib.probeSystem.levelAdjective;

        if (level.text == null) {
            Debug.LogError("Este level nao está na build settings! Sem level.text");
            level.text = "";
        }

        level.text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    IEnumerator rotina() {
        float time = lendaLib.GetLastKeyTime(anime);
        float x = 0;

        yield return new WaitForSeconds(0.5f);

        while (x < time) {
            x += Time.unscaledDeltaTime;

            float size = anime.Evaluate(x);

            transform.localScale = new Vector3(size, size, size);

            yield return null;
        }
    }
}