using UnityEngine;

public class InstantiationTrigger : MonoBehaviour {

    public GameObject instance;

    public Transform offset;

    public Vector3 pos;
    Vector3 offsetPos {
        get {
            if (offset) {
                return offset.position;
            } else {
                return Vector3.zero;
            }
        }
    }

    public bool onStay;

    void Awake() {
        Life lie = GetComponent<Life>();
        if (lie != null) {
            lie.delegados += instanciar;
        }

        playerGun playGun = GetComponent<playerGun>();
        if (playGun != null) {
            playGun.delegados += instanciar;
        }
    }

    void instanciar(Dano damage) {
        Instantiate(instance, damage.source + damage.direction * Vector3.Distance(damage.source, transform.position) * 0.9f, offset.rotation);
    }

    void instanciar(float valor) {
        Instantiate(instance, pos + offsetPos, offset.rotation);
    }

    void OnTriggerEnter(Collider other) {
        Instantiate(instance, pos + offsetPos, offset.rotation);
    }

    void OnCollisionEnter(Collision collision) {
        Instantiate(instance, collision.contacts[0].point, offset.rotation);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0, 0.4f, 1, 0.75f);
        Gizmos.DrawSphere(pos + offsetPos, 0.15f);
    }
}