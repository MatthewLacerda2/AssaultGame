using UnityEngine;

public class mainCanvas : MonoBehaviour {

    public GameObject gameplay;
    public GameObject pause;

    public GameObject mira;
    public GameObject sniperMira;

    public GameObject levelCanvas;

    void Awake() {
        gameObject.SetActive(true);
        lendaLib.mainCanvas = this;

        Instantiate(levelCanvas);

        gameObject.SetActive(true);
        pause.SetActive(false);

        playerLife pl = lendaLib.playerTransf.GetComponent<playerLife>();
        pl.gameplayCanvas = gameplay;
        pl.pauseCanvas = pause;

        gameplay.SetActive(true);
        pause.SetActive(false);
    }

    void Start() {
        foreach(Transform child in mira.transform) {
            child.gameObject.SetActive(false);
        }
    }
}