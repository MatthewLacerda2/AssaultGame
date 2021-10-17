using UnityEngine;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(impulse))]
public class grenade : MonoBehaviour {

    public AnimationCurve damageDropoff;
    public GameObject partySis;

    public float delay = 6.0f;

    public float damage = 70.0f;
    public float radius = 4.0f;

    void Awake() {
        GetComponent<Life>().delegados += die;
    }

    void die(Dano damage) {
        if (damage.killed == true) {
            Explode();
        }
    }

    float timer;
    void Update() { //Foi mais limpo fazer no Update do que numa Routine
        timer += Time.deltaTime;
        if (timer >= delay) {
            Explode();
        }
    }

    void Explode() {
        if (partySis != null) {
            partySis.transform.SetParent(null);
            partySis.SetActive(true);
        }

        lendaLib.explosion(transform.position, radius, damage, damageDropoff);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision) {
        Explode();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}