using UnityEngine;

[RequireComponent(typeof(botInterface))]
[RequireComponent(typeof(Life))]
public class comunicador : MonoBehaviour {

    [System.Serializable] public class fala {
        public AudioClip clip;

        public bool playerVisible, playerNotVisible;
        public enumBotState[] states;   //'murica

        public float frequencia;
    }

    public static int falando, maxFaladores;

    public AudioClip[] danos;
    public fala[] falas;

    public float descanso, range, stagger;

    AudioSource source;
    botInterface botInterf;

    float timeDescanso;

    void Awake() {
        botInterf = GetComponent<botInterface>();

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.minDistance = 0;
        source.maxDistance = range;
        source.rolloffMode = AudioRolloffMode.Linear;

        GetComponent<Life>().delegados += tomarDano;
    }

    // Update is called once per frame
    void Update() {
        source.pitch = Time.timeScale;

        if (source.isPlaying) {
            return;
        }

        timeDescanso += Time.deltaTime;
        if (timeDescanso < descanso) {
            return;
        }

        if (falando >= maxFaladores) {
            return;
        }

        if (botInterf.state == enumBotState.Idle) {
            return;
        }

        if (falas.Length == 0) {
            return;
        }
        fala fl = falas[Random.Range(0, falas.Length)];
        bool temCondicoes = checarCondicoes(fl);
        bool deuSorte = lendaLib.RandomChance(Time.deltaTime, fl.frequencia * lendaLib.probeSystem.maxShooters);

        if (deuSorte && temCondicoes) {
            iniciar(fl.clip);
        }
    }

    bool checarCondicoes(fala fl) {
        if (fl.playerVisible && botInterf.isPlayerVisible == false) {
            return false;
        }
        if (fl.playerNotVisible && botInterf.isPlayerVisible == true) {
            return false;
        }

        float distToPlayer = Vector3.Distance(lendaLib.playerPos, transform.position);
        if (distToPlayer >= range) {
            return false;
        }

        if (fl.states.Length == 0) {
            return true;
        }

        foreach(enumBotState state in fl.states) {
            if (botInterf.state == state) {
                return true;
            }
        }

        return false;
    }

    void iniciar(AudioClip clip) {
        source.clip = clip;
        source.Play();

        timeDescanso = 0;

        StartCoroutine(iniciarRoutine(clip));
    }

    System.Collections.IEnumerator iniciarRoutine(AudioClip clip) {
        timeDescanso = -clip.length;
        falando++;

        yield return new WaitForSeconds(clip.length);

        falando--;
    }

    public void tomarDano(Dano damage) {
        if (damage.amount < stagger) {
            return;
        }

        source.clip = AudioArray.GetRandomClipFrom(danos);
        source.Play();

        STFU(source.clip.length);
    }

    public void STFU(float time) {
        StopAllCoroutines();
        if (timeDescanso<0) {
            falando--;
        }
        timeDescanso = -time;
    }
}