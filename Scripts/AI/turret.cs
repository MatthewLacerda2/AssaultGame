using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(botInterface))]
[RequireComponent(typeof(Life))]
public class turret : MonoBehaviour {

    //TALVEZ ESSE SCRIPT PUDESSE SER SUBSTITUIDO PELO SHOOTERAI
    //MAS FOI MAIS FACIL DE IMPLEMENTAR SEPARADO E É ESPECIFICO PRO TURRET

    public GameObject bloodSplatter;
    public GameObject metalDecal, stoneDecal, woodDecal;
    public GameObject bulletRaspao, capsulaDeBala, tracer;

    public AudioClip parado, mirano, atirano;

    public Transform cano, visaoCamera, visaoCano, ondeCapsular;

    public float dano = 10.0f, range = 50.0f;

    public bool rotX = true, rotY = true, rotZ = true;

    public float maxTimeVista = 0.7f, firerate = 0.15f, rotSpeed = 50.0f;   //No reactionCurve as this bot is an automated machine
    public Vector2 recoil2D = new Vector2(2f, 4f);

    public bool sempreMirano = false;

    Animator anime;
    AudioSource sounds;
    botInterface botInterf;
    Life playLife;
    ParticleSystem partySys;
    SkinnedMeshRenderer skRender;

    const float fov = 135.0f, raspaoDist = 2.5f, rotationSpeed = 150.0f;

    float timeNaVista;
    bool canShoot;

    void Awake() {
        anime = GetComponent<Animator>();
        sounds = GetComponent<AudioSource>();
        botInterf = GetComponent<botInterface>();
        skRender = GetComponentInChildren<SkinnedMeshRenderer>();
        partySys = GetComponentInChildren<ParticleSystem>();

        playLife = lendaLib.playerTransf.GetComponent<Life>();

        GetComponent<Life>().delegados += die;
    }

    void Start() {
        if (lendaLib.probeSystem.transform == transform.root) {
            lendaLib.probeSystem.addBot(botInterf);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Time.timeScale == 0) {
            return;
        } else if (playLife.vida <= 0) {
            this.enabled = false;
            return;
        }

        sounds.pitch = Time.timeScale;

        RefreshInterf();

        Something();
    }

    void RefreshInterf() {
        bool amVisible = skRender.isVisible;
        amVisible = lendaLib.isInViewFrustum(lendaLib.cameraPos, Camera.main.transform.forward, 90.0f, transform.position) && amVisible;

        bool playerIsVisible = false;
        float distToPlayer = Vector3.Distance(lendaLib.playerPos, visaoCano.position);

        if (distToPlayer > range) {
            playerIsVisible = false;
        } else {
            visaoCano.LookAt(lendaLib.cameraPos);

            playerIsVisible = lendaLib.RaycastVisibility(visaoCano.position, lendaLib.playerTransf, range);

            if (visaoCamera) {
                visaoCamera.LookAt(lendaLib.cameraPos);

                playerIsVisible = lendaLib.RaycastVisibility(visaoCamera.position, lendaLib.playerTransf, range) && playerIsVisible;
            }
        }

        if (playerIsVisible) {
            if (timeNaVista < 0) {
                timeNaVista = Time.deltaTime;
            } else {
                timeNaVista += Time.deltaTime;
            }
        } else {
            timeNaVista -= Time.deltaTime;
        }

        canShoot = (playerIsVisible && !canShoot && (timeNaVista >= maxTimeVista + firerate));

        botInterf.Refresh(amVisible, canShoot, playerIsVisible,distToPlayer<=range, timeNaVista);
    }

    void Something() {
        if (timeNaVista >= maxTimeVista + firerate) {
            Fire();
        } else if (botInterf.isPlayerVisible) {
            Rotacionar();
        } else if (parado != null) {
            if (sounds.isPlaying == false) {
                if (sounds.clip != parado) {
                    sounds.clip = parado;
                }
                sounds.Play();
            }
        }
    }

    void Rotacionar() {
        if (mirano != null) {
            if (sounds.isPlaying == false) {
                if (sounds.clip != mirano) {
                    sounds.clip = mirano;
                }
                sounds.Play();
            }
        }

        Vector3 auxVec3 = lendaLib.playerPos;
        if (rotX == false) {    //Pra cima e pra baixo
            auxVec3.x = cano.transform.position.x;
        }
        if (rotY == false) {    //Perpendicular ao solo
            auxVec3.y = cano.transform.position.y;
        }
        if (rotZ == false) {    //Pra nada
            auxVec3.z = cano.transform.position.z;
        }

        Quaternion rotationAngle = Quaternion.LookRotation(auxVec3 - cano.transform.position);
        cano.transform.rotation = Quaternion.Lerp(cano.transform.rotation, rotationAngle, rotationSpeed * Time.deltaTime / 25.0f);
    }

    void Fire() {
        anime.SetTrigger("atirar");

        timeNaVista = maxTimeVista;
        canShoot = false;

        visaoCano.LookAt(lendaLib.playerPos);
        float x = Random.Range(-recoil2D.x, recoil2D.x);
        float y = Random.Range(-recoil2D.y, recoil2D.y);

        visaoCano.Rotate(x, y, 0, Space.Self);

        if (Physics.Raycast(visaoCano.position, visaoCano.forward, out RaycastHit hit, range)) {

            Rigidbody rigidbuddy = hit.transform.GetComponentInChildren<Rigidbody>();
            if (rigidbuddy != null) {
                rigidbuddy.AddForce(visaoCano.forward * dano, ForceMode.Impulse);
            }

            //pra dar imunidade a uma certa parte, basta add um Life a ela e deixar sem sangue
            Life lie = hit.transform.GetComponent<Life>();
            if (lie == null) {
                lie = hit.transform.GetComponentInParent<Life>();
            }

            if (lie != null) {
                lie.AddDamage(dano, dano * 3, visaoCano.position, visaoCano.forward, hit.collider, Dano.danoType.tiro);

                InstDecal(hit, lie.hasBlood);
            } else {
                InstDecal(hit, false);
            }
        } else {
            instRaspao();
        }

        Instantiate(capsulaDeBala, ondeCapsular.position, ondeCapsular.rotation);

        if (partySys != null) {
            partySys.Play();
        }

        if (tracer != null) {
            Instantiate(tracer, visaoCano.transform.position, visaoCano.rotation);
        }

        if (sounds.clip != atirano) {
            sounds.clip = atirano;
            sounds.Play();
        } else if (sounds.isPlaying == false) {
            sounds.Play();
        }
    }

    void instRaspao() {
        Vector3 point = lendaLib.NearestPointInLine(visaoCano.position, visaoCano.forward, lendaLib.cameraPos);
        float dist = Vector3.Distance(point, lendaLib.cameraPos);

        if (dist <= 2.5f) {
            GameObject go = Instantiate(bulletRaspao, point, Quaternion.identity);

            AudioSource source = go.GetComponent<AudioSource>();
            source.clip = go.GetComponent<AudioArray>().GetRandomClip();
            source.Play();
        }
    }

    void InstDecal(RaycastHit hit, bool hasBlood) {
        if (hit.transform.root == lendaLib.playerTransf) {
            return;
        }

        if (hasBlood) {
            Instantiate(bloodSplatter, hit.point, Quaternion.Euler(hit.normal));
            return;
        }

        string material = hit.collider.material.name;
        GameObject go;

        if (material.Contains("Metal")) {
            go = Instantiate(metalDecal, hit.point, Quaternion.Euler(hit.normal), hit.transform);
        } else if (material.Contains("Wood")) {
            go = Instantiate(woodDecal, hit.point, Quaternion.Euler(hit.normal), hit.transform);
        } else {
            go = Instantiate(stoneDecal, hit.point, Quaternion.Euler(hit.normal), hit.transform);
        }

        go.transform.SetParent(hit.transform, true);
    }

    void die(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        sounds.Stop();

        anime.Play("quebrano");

        this.enabled = false;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 0.5f, 0, 1);
        Gizmos.matrix = Matrix4x4.TRS(visaoCano.position, visaoCano.rotation, visaoCano.lossyScale);
        Gizmos.DrawFrustum(Vector3.zero, fov, range, 0.01f, 16.0f / 9.0f);

        Gizmos.color = new Color(0, 0.33f, 0, 1);
        Gizmos.matrix = Matrix4x4.TRS(visaoCano.position, visaoCano.rotation, visaoCano.lossyScale);
        Gizmos.DrawFrustum(Vector3.zero, recoil2D.y * 2, range, 0.01f, 1);
    }
}