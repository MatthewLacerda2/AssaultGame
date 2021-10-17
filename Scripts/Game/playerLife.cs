using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(bulletTime))]
[RequireComponent(typeof(FirstPersonController))]
[RequireComponent(typeof(Life))]
public class playerLife : MonoBehaviour {

    public GameObject hitmark;
    public AudioClip gameOver, gameWin;

    [HideInInspector] public GameObject gameplayCanvas, pauseCanvas;
    
    AudioSource sons;
    AudioArray hits;
    Animator anime;
    bulletTime bulletTimer;
    Life life;
    FirstPersonController fpsControl;
    weaponsHolder weaponsHolder;

    void Awake() {
        Time.timeScale = 1; //+EZ
        
        sons = GetComponent<AudioSource>();
        hits = GetComponent<AudioArray>();
        bulletTimer = GetComponent<bulletTime>();
        life = GetComponent<Life>();
        fpsControl = GetComponent<FirstPersonController>();

        anime = GetComponentInChildren<Animator>();
        weaponsHolder = GetComponentInChildren<weaponsHolder>();

        GetComponent<Life>().delegados += aplicarDano;

        prevPos = transform.position;
    }

    Vector3 prevPos;
    public float maxSpeed = 7f, speed;

    // Update is called once per frame
    void Update() {

        if (Time.timeScale > 0) {

            speed = Vector3.Distance(prevPos, transform.position) / Time.deltaTime;
            if (speed > maxSpeed) {
                speed = maxSpeed;
            }

            prevPos = transform.position;
        }

        if (Input.GetKey(KeyCode.T)) {
            StopAllCoroutines();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        sons.pitch = Time.timeScale;

        if (Input.GetKeyDown(KeyCode.F) && Time.timeScale != 0) {
            bulletTimer.executar();
        }

        if (Input.GetKeyDown(KeyCode.Escape)){

            if (life.vida > 0) {

                if (Time.timeScale == 0) {
                    Time.timeScale = 1;
                    Cursor.lockState = CursorLockMode.Locked;

                    togglePause(false);
                } else {
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;

                    togglePause(true);
                }

            } else {
                gameplayCanvas.SetActive(true);
            }
        }
    }

    void togglePause(bool action) {

        Cursor.visible = action;
        pararPlayer(action);
        pauseCanvas.SetActive(action);
        gameplayCanvas.SetActive(!action);
    }

    void aplicarDano(Dano dano) {
        if (life.vida > 0) {
            if (sons.isPlaying == false) {
                sons.clip = hits.GetRandomClip();
                sons.Play();
            }
        } else {
            GetComponentInChildren<CapsuleCollider>().transform.GetComponent<MeshRenderer>().enabled = false;
            Time.timeScale = 1;
            //sons.clip = gameOver;
            //sons.Play();

            anime.Play("deathPlayer");

            pararPlayer(true);

            life.delegados -= aplicarDano;

            StartCoroutine(deathRestart());
        }
    }

    IEnumerator deathRestart() {
        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void pararPlayer(bool parar) {
        fpsControl.enabled = !parar;
        weaponsHolder.gameObject.SetActive(!parar);
    }

    IEnumerator slowToWin() {
        bulletTimer.StopAllCoroutines();

        GameObject.Find("LevelCanvas(Clone)").GetComponentInChildren<menuGameWin>().levelWinCanvas();

        float pauseTime = 0.7f;

        //lerp timescale from 1 to 0.33?
        Time.timeScale = 0.4f;

        yield return new WaitForSeconds(pauseTime);

        Time.timeScale = 0;

        pararPlayer(true);

        while (!Input.GetKeyDown(KeyCode.Return)) {
            yield return null;
        }

        ProbeSystem.isSceneFirstLoad = true;
        SceneManager.LoadScene(lendaLib.probeSystem.nextSceneIndex);
    }

    //Esta função é chamada pelo ProbeSystem, pq ele que sabe qndo a fase foi vencida
    public void levelWin() {
        StartCoroutine(slowToWin());

        /*
        float aux = sons.volume * 2;
        if (aux > 1) {
            aux = 1;
        }

        sons.volume = aux;
        sons.clip = gameWin;
        sons.Play();
        */
    }
}