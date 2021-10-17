using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class musicBackground : MonoBehaviour {

    public AudioClip[] musicas;

    AudioSource source;

    float initVol = 1;

    void Awake() {
        if (Application.isEditor == false) {
            this.enabled = true;
        }

        DontDestroyOnLoad(this.gameObject);

        source = GetComponent<AudioSource>();

        initVol = source.volume / 2;

        int x = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log(x);

        if (x < 17) {
            source.clip = musicas[0];
        } else {
            source.clip = musicas[1];
        }

        source.enabled = false;

        //SceneManager.sceneLoaded += OnLevelLoad;
    }

    void Start() {
        source.enabled = true;
    }

    // Update is called once per frame
    void Update() {
        transform.position = lendaLib.cameraPos;

        if (Time.timeScale > 0) {
            source.volume = initVol * Time.timeScale * 2;
        }
    }
}