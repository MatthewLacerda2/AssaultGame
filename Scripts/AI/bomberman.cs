using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(botInterface))]
[RequireComponent(typeof(Life))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class bomberman : MonoBehaviour {

    //public AudioClip[] warScreams;

    public AnimationCurve damageDropoff;
    public GameObject explosao;

    public float radius, dano;

    AudioSource sounds;
    botInterface botInterf;
    NavMeshAgent agent;
    Life lie, playerLife;
    ProbeSystem proSys;
    Rigidbody rigidbuddy;

    void Awake() {
        botInterf = GetComponent<botInterface>();
        sounds = GetComponent<AudioSource>();
        lie = GetComponent<Life>();
        agent = GetComponent<NavMeshAgent>();
        rigidbuddy = GetComponent<Rigidbody>();

        lie.delegados += kaboom;

        playerLife = lendaLib.playerTransf.GetComponent<Life>();
    }

    // Start is called before the first frame update
    void Start() {
        proSys = lendaLib.probeSystem;
        proSys.addBot(botInterf);
    }

    // Update is called once per frame
    void Update() {
        if (Time.timeScale == 0) {
            return;
        }else if (playerLife.vida <= 0) {
            agent.isStopped = true;
            this.enabled = false;
            return;
        }

        sounds.pitch = Time.timeScale;

        float dist = Vector3.Distance(lendaLib.playerPos, transform.position);

        float explodistance = 2.0f;
        if (dist <= explodistance) {
            lie.AddDamage(new Dano(dano, dano/4.0f, transform.position, Vector3.zero, null, Dano.danoType.explosao));
            this.enabled = false;
            return;
        }

        //-------------

        refreshInterf();
        /*
        if (sounds.isPlaying == false) {
            if (lendaLib.RandomChance(Time.deltaTime, 3.333f)) {
                sounds.clip = AudioArray.GetRandomClipFrom(allahuAkbars);
                sounds.Play();
            }
        }
        */
        Vector3 sightProbe = proSys.NearestProbePosition(transform.position, true);
        dist = Vector3.Distance(sightProbe, transform.position);

        if (dist > explodistance) {
            agent.SetDestination(sightProbe);
        } else {
            agent.SetDestination(lendaLib.playerPos);
        }
    }

    void refreshInterf() {
        float distToPlayer = Vector3.Distance(lendaLib.playerPos, transform.position);

        botInterf.amVisible = lendaLib.isInViewFrustum(lendaLib.cameraPos, Camera.main.transform.forward, Camera.main.fieldOfView, transform.position);
        botInterf.canShoot = false;
        botInterf.isPlayerVisible = false;
        botInterf.playerWithinRange = distToPlayer < 9;
        botInterf.timeNaVista = 0;
        botInterf.distToPlayer = distToPlayer;
    }

    void kaboom(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        lie.delegados -= kaboom;

        proSys.removeBot(botInterf);

        sounds.enabled = false;
        rigidbuddy.isKinematic = false;
        agent.enabled = false;
        rigidbuddy.AddForce(damage.amount * damage.direction, ForceMode.Impulse);

        explosao.transform.parent = null;
        explosao.SetActive(true);

        lendaLib.explosion(transform.position + (Vector3.up * 1.25f), radius, dano, damageDropoff);

        Destroy(gameObject, 0.1f);

        this.enabled = false;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius*2);
    }
}