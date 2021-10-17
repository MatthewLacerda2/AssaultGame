using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioArray))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(botInterface))]
[RequireComponent(typeof(CollisionSounds))]
[RequireComponent(typeof(DonaMorte))]
[RequireComponent(typeof(Life))]
//[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(ProbeAgent))]
[RequireComponent(typeof(Rigidbody))]
public class shooterAI : MonoBehaviour {

    public AnimationCurve speedDropoff;

    public AnimationCurve damageDropoff;
    public float dano = 30.0f;

    public AnimationCurve reactionFactor;
    public float maxTimeVista = 0.7f;

    [Space] public GameObject bloodSplatter;
    public GameObject metalDecal, stoneDecal, woodDecal, raycastInst;
    public GameObject bulletRaspao, capsulaDeBala;

    //public GameObject meshDecal; // Um dia, amigo...

    public Transform arma, visao, ondeCapsular;
    public AudioClip weaponSound;
    public ParticleSystem muzzleFlash;

    [HideInInspector] public botInterface botInterf;
    Animator anime;
    AudioArray walkClips;
    AudioSource sounds;
    CollisionSounds colSound;
    comunicador comunica;
    DonaMorte donaMorte;
    Life myLife, lifePlayer;
    playerLife pLife;
    ProbeAgent probeAgent;
    Rigidbody rigidbuddy;
    SkinnedMeshRenderer skRender;

    [Range(1, 20)] public int tracesPerShot = 1;

    public float firerate = 0.33f, range = 30.0f, rotationSpeed = 120.0f;
    public float staggerDano = 2.0f, twitchDano = 1.0f, fov = 60.0f, reactionReflex = 0.1f;

    public Vector2 spread2D = new Vector2(2, 4);
    Vector2 spreadTime;

    [Space] public float meleeRange = 0.6f;
    public float meleeFirate = 0.83f, meleeDamage = 40f;
    public AudioClip meleeClip;

    float timeNaVista = -0.5f, distToPlayer = 30.0f;
    bool canShoot;

    void Awake() {
        anime = GetComponent<Animator>();
        botInterf = GetComponent<botInterface>();
        colSound = GetComponent<CollisionSounds>();
        comunica = GetComponent<comunicador>();
        donaMorte = GetComponent<DonaMorte>();
        sounds = GetComponent<AudioSource>();
        myLife = GetComponent<Life>();
        probeAgent = GetComponent<ProbeAgent>();
        rigidbuddy = GetComponent<Rigidbody>();
        walkClips = GetComponent<AudioArray>();

        skRender = GetComponentInChildren<SkinnedMeshRenderer>();
        lifePlayer = lendaLib.playerTransf.GetComponent<Life>();
        pLife = lendaLib.playerTransf.GetComponent<playerLife>();

        escudao escudo = GetComponentInChildren<escudao>();
        if (escudo) {
            escudo.fuck(myLife);
        }

        if (muzzleFlash == null) {
            muzzleFlash = arma.GetComponentInChildren<ParticleSystem>();
        }

        myLife.delegados += tomarDano;

        colSound.enabled = false;
    }

    void Start() {
        bool isRooted = lendaLib.probeSystem.transform == transform.root;
        bool isHologram = GetComponent<hologramAI>() != null;
        
        if (isHologram == false && isRooted) {
            lendaLib.probeSystem.addBot(botInterf);
        }

        if (muzzleFlash != null) {
            muzzleFlash.Stop();
            muzzleFlash.Clear();
        }
    }

    // Update is called once per frame
    void Update() {
        
        dueProcess();

        DoSomething();
    }

    public void dueProcess() {
        sounds.pitch = Time.timeScale;

        if (Time.timeScale == 0) {
            return;
        } else if (lifePlayer.vida <= 0) {
            probeAgent.toggleAndar(false);
            this.enabled = false;
            return;
        }

        distToPlayer = Vector3.Distance(lendaLib.playerPos, transform.position);

        bool amVisible = true, playerIsVisible=false;

        if (skRender) {
            amVisible = skRender.isVisible; //Pode dar true se fizer sombra ou o occlusion culling falhar   //Frustum() é uma sobreguarda
        }

        amVisible = lendaLib.isInViewFrustum(lendaLib.cameraPos, Camera.main.transform.forward, 90.0f, transform.position) && amVisible;

        if (distToPlayer > range) {
            playerIsVisible = false;
        } else if (distToPlayer <= meleeRange) {
            playerIsVisible = true;
        } else {
            visao.LookAt(lendaLib.playerPos);

            Vector3 euler = visao.localRotation.eulerAngles;
            euler.y = Mathf.Clamp(euler.y, -fov / 2, fov / 2);
            visao.localRotation = Quaternion.Euler(euler);

            if (Mathf.Abs(visao.localRotation.y) > fov) {
                playerIsVisible = false;
            } else {
                playerIsVisible = lendaLib.RaycastVisibility(visao.position, lendaLib.playerTransf, range);
            }
        }

        if (playerIsVisible) {

            if (timeNaVista < 0) {
                timeNaVista = Time.deltaTime;
            } else if (timeNaVista < firerate) {
                timeNaVista += (Time.deltaTime * reactionFactor.Evaluate(distToPlayer / range));
            } else {
                timeNaVista += Time.deltaTime;
            }
        } else {
            if (timeNaVista >= maxTimeVista - reactionReflex) {
                timeNaVista += Time.deltaTime;
            } else {
                timeNaVista -= Time.deltaTime * 2;
            }
        }

        canShoot = playerIsVisible && !canShoot && (timeNaVista >= maxTimeVista + firerate) && visao.localRotation.y <= spread2D.y;

        botInterf.Refresh(amVisible, canShoot, playerIsVisible, distToPlayer <= range, timeNaVista);
    }

    void DoSomething() {
        switch (botInterf.state) {

            case enumBotState.Covering:
                MirarTiro();
                break;

            case enumBotState.Flank:
                walk(true);
                probeAgent.Flankear();
                break;

            case enumBotState.Idle:
                walk(false);
                timeNaVista = 0;
                break;

            case enumBotState.MirarTiro:
                MirarTiro();
                break;

            case enumBotState.ShortestSight:
                walk(true);
                probeAgent.ShortestSight();
                break;

            case enumBotState.Stagger:
                walk(false);
                timeNaVista = maxTimeVista - 1.3f;
                break;

            case enumBotState.StraightPath:
                walk(true);
                probeAgent.SetDestination(lendaLib.playerPos);
                break;

            case enumBotState.Melee:
                Melee();
                break;

            case enumBotState.Repositioning:
                break;

            default:
                Debug.Log("Nao tem state aqui");
                break;
        }
    }

    void MirarTiro() {

        if (canShoot) {
            Fire();
        } else {
            walk(false);
        }
    }

    void Melee() {

        if (distToPlayer > meleeRange) {
            probeAgent.ShortestSight();
            return;
        }

        mirar(rotationSpeed * 2);

        if (timeNaVista < meleeFirate) {
            return;
        }

        Vector3 pos = transform.position + Vector3.up;
        if (arma != null) {
            pos = arma.transform.position;
        }

        Vector3 dir = lendaLib.cameraPos - pos;

        if (meleeClip != null) {
            sounds.clip = meleeClip;
            sounds.Play();
        }

        anime.Play(enumBotState.Melee.ToString());

        if (distToPlayer <= meleeRange) {
            lifePlayer.AddDamage(meleeDamage, meleeDamage / 2.5f, pos, dir, null, Dano.danoType.fisico);
        }

        timeNaVista = -meleeFirate;
    }

    void walk(bool andar) {
        probeAgent.toggleAndar(andar);

        if (andar) {
            if (!sounds.isPlaying) {
                sounds.clip = walkClips.GetRandomClip();
                sounds.Play();
            }
        } else {
            mirar(rotationSpeed);
        }
    }

    void Fire() {

        for (int i = 0; i < tracesPerShot; i++) {

            bool playerou = false;

            visao.LookAt(lendaLib.playerPos);

            float x = Random.Range(-spread2D.x, spread2D.x);
            float y = Random.Range(-spread2D.y, spread2D.y);

            float speedRatio = 1 / (speedDropoff.Evaluate(pLife.speed / pLife.maxSpeed));

            x *= speedRatio;
            y *= speedRatio;

            if (botInterf.state == enumBotState.Covering) {
                x *= 3.5f;
                y *= 3.5f;
            }

            visao.Rotate(x, y, 0, Space.Self);

            if (Physics.Raycast(visao.position, visao.forward, out RaycastHit hit, range)) {

                Rigidbody rigidbuddy = hit.transform.GetComponentInChildren<Rigidbody>();
                if (rigidbuddy != null) {
                    rigidbuddy.AddForce(visao.forward * dano, ForceMode.Impulse);
                }

                //pra dar imunidade a uma certa parte, basta add um Life a ela e deixar sem sangue
                Life lie = hit.transform.GetComponent<Life>();
                if (lie == null) {
                    lie = hit.transform.GetComponentInParent<Life>();
                }

                if (lie != null) {
                    float auxDano = dano * damageDropoff.Evaluate(distToPlayer / range);

                    if (botInterf.state != enumBotState.Covering) {
                        lie.AddDamage(auxDano / tracesPerShot, auxDano, visao.position, visao.forward, hit.collider, Dano.danoType.tiro);
                    }

                    playerou = hit.transform.root == lendaLib.playerTransf;

                    InstDecal(hit, lie.hasBlood);
                } else {
                    InstDecal(hit, false);
                }
            } else {
                instRaspao();
            }

            if (raycastInst != null) {
                Instantiate(raycastInst, muzzleFlash.transform.position, visao.rotation);
            }

        }

        if (capsulaDeBala != null) {
            Instantiate(capsulaDeBala, ondeCapsular.position, ondeCapsular.rotation);
        }

        if (muzzleFlash != null) {
            muzzleFlash.Play();
        }

        if (sounds.isPlaying == false) {
            sounds.clip = weaponSound;
            sounds.Play();
        }

        canShoot = false;
        timeNaVista = maxTimeVista;
    }

    void instRaspao() {
        if (bulletRaspao == null) {
            return;
        }

        Vector3 point = lendaLib.NearestPointInLine(visao.position, visao.forward, lendaLib.cameraPos);
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

    void mirar(float rotSpeed) {
        Vector3 auxVec3 = lendaLib.playerPos;
        auxVec3.y = transform.position.y;

        Quaternion rotationAngle = Quaternion.LookRotation(auxVec3 - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationAngle, rotSpeed * Time.deltaTime / 25.0f);
    }

    void tomarDano(Dano damage) {

        if (damage.killed) {
            morreno(damage);
            return;
        }

        if (damage.amount >= staggerDano) {
            walk(false);

            timeNaVista = maxTimeVista - 1.5f;
        }

        if (damage.amount >= twitchDano) {
            timeNaVista -= (Time.deltaTime * 0.5f);
            donaMorte.Twitch(damage);
        }
    }

    void morreno(Dano damage) {
        botInterf.state = enumBotState.Idle;

        myLife.delegados -= tomarDano;

        probeAgent.enabled = false;

        bool animateDeath = donaMorte.killThisMotherfucker(damage);

        if (animateDeath) {
            colSound.enabled = false;

            sounds.enabled = false;

            foreach (Collider col in GetComponentsInChildren<Collider>()) {
                col.enabled = false;
            }
        } else {
            colSound.enabled = true;

            Ragdollar(damage);
        }

        if (comunica) {
            comunica.tomarDano(damage);
            comunica.enabled = false;
            myLife.delegados -= comunica.tomarDano;
        }

        this.enabled = false;

        lendaLib.probeSystem.removeBot(botInterf);

        if (GetComponent<hologramAI>() == null) {
            if (arma != null) {
                arma.GetComponent<ammoDrop>().enabled = true;
            }
        } else if (GetComponent<explodeOnDeath>()) {
            GetComponent<explodeOnDeath>().kaboom();
        } else {
            arma.gameObject.SetActive(false);
        }

        Rigidbody armaBody = arma.GetComponent<Rigidbody>();
        armaBody.isKinematic = false;
    }

    public void Ragdollar(Dano damage) {
        anime.enabled = false;

        rigidbuddy.isKinematic = false;

        if (damage.collider != null) {
            Rigidbody body = damage.collider.transform.GetComponentInParent<Rigidbody>();

            body.isKinematic = false;

            Vector3 forca = damage.direction * damage.amount;
            body.AddForce(forca);
        }
    }

    void OnDrawGizmosSelected() {
        /*
        //Draw Melee Range
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Vector3 pos = transform.position;
        if (arma != null) pos = arma.transform.position;
        Gizmos.DrawWireSphere(pos, meleeRange);
        */
        //Draw View Frustum
        Gizmos.color = new Color(1, 0.5f, 0, 1);
        Gizmos.matrix = Matrix4x4.TRS(visao.position, visao.rotation, visao.lossyScale);
        Gizmos.DrawFrustum(Vector3.zero, fov, range, 0.01f, 16.0f / 9.0f);
        
        //Draw targeting Frustum
        Gizmos.color = new Color(0, 0.33f, 0, 1);
        Gizmos.matrix = Matrix4x4.TRS(visao.position, visao.rotation, visao.lossyScale);
        Gizmos.DrawFrustum(Vector3.zero, spread2D.y * 2, range, 0.01f, 1);
        
#if UNITY_EDITOR
        UnityEditor.SceneView view = UnityEditor.SceneView.currentDrawingSceneView;
        if (view != null && Application.isPlaying) {
            view.pivot = gameObject.transform.position + Vector3.up;
        }
#endif
    }
}