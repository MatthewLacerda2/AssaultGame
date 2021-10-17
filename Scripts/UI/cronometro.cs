using UnityEngine;
using TMPro;

public class cronometro : MonoBehaviour {

    public float timer;

    TextMeshProUGUI textM;

    void Start() {
        textM = GetComponent<TextMeshProUGUI>();

        TextMeshProUGUI child = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        child.text = lendaLib.probeSystem.targetTime.ToString("n2");
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;

        textM.text = timer.ToString("n3").ToString();
    }
}