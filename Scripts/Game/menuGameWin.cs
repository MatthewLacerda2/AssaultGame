using UnityEngine;
using TMPro;

public class menuGameWin : MonoBehaviour {

    public TextMeshProUGUI tempo;
    public TextMeshProUGUI pizzaTime;

    void Awake() {
        transform.localScale = Vector3.zero;
    }

    public void levelWinCanvas() {

        float time = lendaLib.mainCanvas.GetComponentInChildren<cronometro>().timer;
        tempo.text = "Tempo: " + time.ToString("n3");

        ProbeSystem proSys = lendaLib.probeSystem;
        pizzaTime.gameObject.SetActive(proSys.targetTime >= time);

        GetComponent<ScaleLerp>().enabled = true;
    }
}