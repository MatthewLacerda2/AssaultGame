using UnityEngine;

public class Sparkles : MonoBehaviour {

    public Vector2 spread = new Vector2(0.3f, 1.2f);

    public float range = 30f;

    public AudioClip[] clips;

    public Vector3[] positions;

    AudioSource source;
    ParticleSystem p;

    float timer;

    void Awake() {
        source = GetComponent<AudioSource>();
        p = GetComponent<ParticleSystem>();

        timer = Random.Range(spread.x, 0);
    }

    void Update() {
        source.pitch = Time.timeScale;

        if (timer > 0) {
            timer -= Time.deltaTime;
            return;
        }

        float dist = Vector3.Distance(lendaLib.playerPos, transform.position);
        if (dist > range) {
            return;
        }

        AudioClip clipe = AudioArray.GetRandomClipFrom(clips);
        timer = (clipe.length*0.8f) + Random.Range(spread.x, spread.y);
        source.clip = clipe;
        source.Play();
        p.Play();

        int index = Random.Range(0, positions.Length - 1);
        transform.LookAt(transform.position + positions[index]);
    }

    void OnDrawGizmosSelected() {
        foreach(Vector3 v3 in positions) {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawSphere(transform.position + v3, 0.3f);
        }
    }
}