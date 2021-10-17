using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(swing))]
public class playerLauncher : MonoBehaviour {

    public Transform origem;    //Essa ordem fica bacana no Inspector
    public GameObject projetil;
    public AudioClip shootClip, outtaBullets;

    public float firerate = 0.3f, enableTime = 0.35f;
    public int numBullets = 20;
    public bool isTapFire = true;

    //public delegate void ouvinte(float valor);
    //public ouvinte delegados;

    Animator anime;
    AudioSource source;
    ParticleSystem partySis;

    float timer;

    void Awake() {
        anime = GetComponent<Animator>();
        partySis = GetComponentInChildren<ParticleSystem>();
        source = transform.parent.GetComponent<AudioSource>();
        origem = Camera.main.transform;
    }

    void OnEnable() {
        timer = firerate - enableTime;
    }

    void Update() {
        if (Time.deltaTime == 0) {
            return;
        }

        source.pitch = Time.timeScale;

        timer += Time.deltaTime;

        bool shouldShoot = isTapFire && Input.GetKeyDown(KeyCode.Mouse0);   // :3
        shouldShoot = shouldShoot || (!isTapFire && Input.GetKey(KeyCode.Mouse0));

        if (shouldShoot) {
            if (numBullets > 0) {
                if (timer >= firerate) {
                    Fire();
                }
            } else {
                if (source.isPlaying == false) {
                    source.clip = outtaBullets;
                    source.Play();
                }
            }
        }
    }

    void Fire() {
        GameObject p = Instantiate(projetil, origem.position, origem.rotation);

        timer = 0;

        source.clip = shootClip;
        source.Play();

        anime.SetTrigger("atirano");

        if (partySis != null) {
            partySis.Play();
        }
    }

    //Código simples, chega dá um tesão
}