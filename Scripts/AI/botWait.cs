using UnityEngine;
using UnityEngine.AI;

public class botWait : MonoBehaviour {

    [Range(0f, 60f)] public float timer = 0;
    [Range(2f, 30f)] public float range = 2f;

    public RuntimeAnimatorController animeController;

    public Life protegido;
    public Vector3 target;

    public bool aimAtStart = true;
    public bool onTrigger;
    public bool enxerga = true;

    public Bounds bound = new Bounds(new Vector3(0, 2.5f, 0), new Vector3(5.0f, 5.0f, 5.0f));

    comunicador comunic;
    NavMeshAgent agent;
    shooterAI shooter;  //a ideia seria tirar isso e deixar o state em Idle
    Animator anime;
    RuntimeAnimatorController auxAnime;

    float originalRange;

    void Awake() {
        shooter = GetComponent<shooterAI>();
        shooter.enabled = false;

        GetComponent<Life>().delegados += tomarDano;

        if (target.magnitude>1) {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        comunic = GetComponent<comunicador>();
        if (comunic) {
            comunic.enabled = false;
        }

        anime = GetComponent<Animator>();

        if (animeController != null) {
            auxAnime = anime.runtimeAnimatorController;
            anime.runtimeAnimatorController = animeController;
        }

        if (enxerga==false) {
            anime.speed = 0;
        }
    }

    void Start() {
        if (lendaLib.probeSystem.transform == transform.root) {
            lendaLib.probeSystem.addWaiter(this);
        }

        originalRange = shooter.range;
        //shooter.range = 50;

        if (timer > 0) {
            //StartCoroutine(wait());
        }

        if (target.magnitude>1) {
            agent.SetDestination(target + transform.position);
        }
    }

    System.Collections.IEnumerator wait() {
        yield return new WaitForSeconds(timer);
        acordar(" wait");
    }

    // Update is called once per frame
    void Update() {
        if (Time.timeScale == 0) {
            return;
        }

        shooter.dueProcess();

        if (aimAtStart) {
            mirar();
        }

        if (protegido) {    //gambiarra leve, use delegado instead
            if (protegido.vida <= 0) {
                acordar(" vai se vingar");
                return;
            }
        }

        if (onTrigger && Input.GetKeyDown(KeyCode.Mouse0)) {
            acordar(" levou susto");
            return;
        }

        bool completedPath = target.magnitude > 1 && (agent.pathStatus == NavMeshPathStatus.PathComplete);

        bool inRange = Vector3.Distance(lendaLib.playerPos, transform.position) <= range;
        inRange = lendaLib.BoundsContains(transform.position + bound.center, bound.extents, lendaLib.playerPos) || inRange;

        if (completedPath) {
            acordar(" path complete");
            return;
        }else if (inRange) {
            acordar(" in dist");
            return;
        }

        if (enxerga) {
            ProbeSample pos = lendaLib.probeSystem.NearestProbe(transform.position , true);
            float sampleDist = Vector3.Distance(transform.position, pos.position);
            bool sampleVisible = sampleDist <= 3f && pos.amInFrustum == true;

            if (sampleVisible) {
                acordar(" ta com sight");
                return;
            }

            bool playerVisible = shooter.botInterf.isPlayerVisible; // && shooter.botInterf.timeNaVista >= shooter.maxTimeVista / 3;

            if (playerVisible) {
                acordar(" viu você");
            }
        }
    }

    void mirar() {
        Vector3 auxVec3 = lendaLib.playerPos;
        auxVec3.y = transform.position.y;

        Quaternion rotationAngle = Quaternion.LookRotation(auxVec3 - transform.position);
        transform.rotation = rotationAngle;
    }

    void tomarDano(Dano dano) {
        acordar(" tomou dano");
    }

    public void acordar(string strange) {
        //Debug.Log(transform.name + strange);

        StopAllCoroutines();
        anime.enabled = true;
        GetComponent<Life>().delegados -= tomarDano;

        if (comunic) {
            comunic.enabled = true;
        }

        anime.speed = 1;

        if (animeController) {
            StartCoroutine(animeRoutine());
        }

        lendaLib.probeSystem.removeWaiter(this);
        this.enabled = false;

        shooter.range = originalRange;
        shooter.enabled = true;
    }

    System.Collections.IEnumerator animeRoutine() {

        anime.SetTrigger("acordar");

        float animationLength = anime.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        anime.runtimeAnimatorController = auxAnime;
    }

    void OnDrawGizmosSelected() {
        if (this.enabled == false) {
            return;
        }

        if (target.magnitude > 1) {
            Gizmos.color = new Color(0, 1, 0, 0.4f);

            Gizmos.DrawSphere(target + transform.position, 0.2f);
        }

        //Gizmos.color = new Color(0, 1, 1, 1);
        Gizmos.color = Color.yellow;

        if (range > 2f) {
            Gizmos.DrawWireSphere(transform.position, range);
        }

        if (bound.extents.magnitude >= 4f) {
            Gizmos.DrawWireCube(transform.position + bound.center, bound.extents);
        }
    }
}