using UnityEngine;

[RequireComponent(typeof(Life))]
public class DestroyOnDeath : MonoBehaviour {

    public float delay = 0;

    public bool detachOnDeath;

    void Awake() {
        GetComponentInParent<Life>().delegados += OnDeath;
    }

    void OnDeath(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        if (detachOnDeath) {
            transform.SetParent(null);
        }

        StartCoroutine(deathRot());
    }

    System.Collections.IEnumerator deathRot() {

        ParticleSystem ps = GetComponent<ParticleSystem>();

        float dur = ps.main.duration;

        if (delay < dur) {
            delay = dur;
        }

        ps.Play();

        yield return new WaitForSeconds(dur);

        Destroy(gameObject);
    }
}