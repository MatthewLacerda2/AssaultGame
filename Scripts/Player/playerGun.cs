using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(swing))]
public class playerGun : MonoBehaviour {

    public Vector2Int scenes;

    [Space] public AudioClip outtaBullets, shootClip;

    [HideInInspector] public float originFOV = 90.0f;

    public float damage = 20f;
    public float range = 40f;
    public float impulso = 10.0f;
    public AnimationCurve damageCurve;

    public GameObject bloodSplatter, stoneDecal, metalDecal, woodDecal;
    public Transform origem;
    public ParticleSystem partySis;

    [Range(1, 24)] public int tracesPerShot;

    public float firerate = 0.3f, enableTime = 0.35f;
    public int maxBullets = 100;
    public int numBullets = 20;
    public bool isTapFire;

    [Space] public float kickSpeed = 0.7f;
    public float kickAmount;
    public Vector2 spread2D = new Vector2(1, 1);

    public delegate void ouvinte(float valor);
    public ouvinte delegados;

    Animator anime;
    AudioSource source;
    GameObject myAim;

    Camera mainCam, subCam;

    float timer;

    void Awake() {

        anime = GetComponent<Animator>();

        if (partySis == null) {
            partySis = GetComponentInChildren<ParticleSystem>();
        }

        source = transform.parent.GetComponent<AudioSource>();
        origem = Camera.main.transform;

        Transform auxTransf = Camera.main.transform;
        mainCam = auxTransf.GetComponent<Camera>();
        subCam = auxTransf.GetChild(0).GetComponent<Camera>();

        myAim = GameObject.Find("Canvas").GetComponent<mainCanvas>().mira;
        foreach (Transform tr in myAim.transform) {
            if (tr.name == transform.name) {
                myAim = tr.gameObject;
                break;
            }
        }
    }

    void Start() {
        OnEnable();
    }

    void OnEnable() {
        timer = firerate - enableTime;

        myAim.SetActive(true);
    }

    void OnDisable() {
        if (myAim != null) {
            myAim.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        numBullets = (int)Mathf.Clamp(numBullets, 0, maxBullets);    //that    is    N O T    a    s o l u t i o n

        if (Time.deltaTime == 0) {
            return;
        }

        mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView - (Time.deltaTime * kickSpeed), originFOV, 130.0f);
        subCam.fieldOfView = Mathf.Clamp(subCam.fieldOfView - (Time.deltaTime * kickSpeed), originFOV, 130.0f);

        source.pitch = Time.timeScale;

        if (timer < 0) {
            timer += Time.unscaledDeltaTime;
        } else {
            timer += Time.deltaTime;
        }

        bool shouldShoot = isTapFire && Input.GetKeyDown(KeyCode.Mouse0);
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

    void actualTrace() {
        Vector3 originRot = origem.localRotation.eulerAngles;

        float x = Random.Range(-spread2D.x, spread2D.x);
        float y = Random.Range(-spread2D.y, spread2D.y);

        origem.Rotate(new Vector3(x, y, 0), Space.Self);

        if (Physics.Raycast(origem.position + (origem.forward * 0.2f), origem.forward, out RaycastHit hit, range)) {

            float auxDamage = damage * damageCurve.Evaluate(hit.distance/range) / tracesPerShot;
            Vector3 forca = origem.forward.normalized * impulso / tracesPerShot;

            Life lie = hit.transform.GetComponent<Life>();
            if (lie == null) {
                lie = hit.transform.GetComponentInParent<Life>();
            }

            if (lie != null) {
                if (hit.transform.name == "Head") {
                    auxDamage *= 2f;
                }

                lie.AddDamage(auxDamage, forca.magnitude, origem.position, origem.forward, hit.collider, Dano.danoType.tiro);

                if (lie.hasBlood) {
                    Instantiate(bloodSplatter, hit.point, Quaternion.Euler(hit.normal), hit.transform);
                } else {
                    InstDecal(hit);
                }
            } else {
                InstDecal(hit);

                Rigidbody rigidbuddy = hit.transform.GetComponentInChildren<Rigidbody>();
                if (rigidbuddy != null) {
                    rigidbuddy.AddForce(forca, ForceMode.Impulse);
                }
            }
        }

        origem.localRotation = Quaternion.Euler(originRot);
    }

    void Fire() {

        timer = 0;
        numBullets--;

        source.clip = shootClip;
        source.Play();

        anime.SetTrigger("atirano");

        if (partySis != null) {
            partySis.Play();
        }

        float auxFov = mainCam.fieldOfView;
        if (auxFov < originFOV + kickAmount) {
            mainCam.fieldOfView = originFOV + kickAmount;
            subCam.fieldOfView = originFOV + kickAmount;
        }

        for (int i = 0; i < tracesPerShot; i++) {
            actualTrace();
        }

        if (delegados != null) {
            delegados(firerate);
        }
    }

    //nitpicking: essa funcao nao considera mudanca de FOV entre tiros
    System.Collections.IEnumerator kicker() {
        float dif = kickAmount;

        while (dif >= 0) {
            if (dif - Time.deltaTime < 0) {
                dif = 0;
            }

            mainCam.fieldOfView = originFOV + dif;
            subCam.fieldOfView = originFOV + dif;

            dif -= Time.deltaTime * kickSpeed;

            yield return null;
        }
    }

    void InstDecal(RaycastHit hit) {
        string material = hit.collider.material.name;

        GameObject go;

        if (material.Contains("Metal")) {
            go = Instantiate(metalDecal, hit.point, Quaternion.LookRotation(hit.normal));
        } else if (material.Contains("Wood")) {
            go = Instantiate(woodDecal, hit.point, Quaternion.LookRotation(hit.normal));
        } else {
            go = Instantiate(stoneDecal, hit.point, Quaternion.LookRotation(hit.normal));
        }

        go.transform.SetParent(hit.transform, true);
    }
}