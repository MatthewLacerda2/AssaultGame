using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Rigidbody))]
public class minas : MonoBehaviour {

    [Range(0.5f, 5.0f)] public float delay;

    public AnimationCurve damageDropoff;
    public GameObject explosion;
    public GameObject deathParticles;
    public Vector3 jumpVector;

    public float radius = 5.0f, damage = 70.0f;

    AudioSource source;

    void Awake() {
        source = GetComponent<AudioSource>();

        GetComponent<Life>().delegados += morrer;
    }

    // Start is called before the first frame update
    void Update() {
        if(Vector3.Distance(lendaLib.playerPos, transform.position) <= radius) {
            StartRoutine();
        }
    }

    public void StartRoutine() {
        if (jumpVector.magnitude > 0) {
            Rigidbody rigidbuddy = GetComponent<Rigidbody>();
            rigidbuddy.isKinematic = false;
            rigidbuddy.AddForce(jumpVector * 2.57f / rigidbuddy.mass, ForceMode.Impulse);
        }

        source.Play();

        StartCoroutine(explosionRoutine());

        this.enabled = false;
    }

    void morrer(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        StopAllCoroutines();

        deathParticles.transform.SetParent(null);
        deathParticles.SetActive(true);

        Destroy(gameObject);
    }

    IEnumerator explosionRoutine() {
        yield return new WaitForSeconds(delay);

        lendaLib.explosion(transform.position, radius, damage, damageDropoff);

        explosion.transform.SetParent(null);
        explosion.SetActive(true);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 0.05f, 0.05f, 0.1f);
        Gizmos.DrawSphere(transform.position, radius);

        Gizmos.color = new Color(1, 0.05f, 0.05f, 1);
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + jumpVector);
    }
}