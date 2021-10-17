using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class playerGrenadeProjectie : MonoBehaviour {

    public float timer = 0.333f;
    public float dano = 100.0f;
    public float radius = 3.0f;
    public float impulse = 500;

    public AnimationCurve curva;
    public GameObject explosao;

    Rigidbody rigidbuddy;

    // Start is called before the first frame update
    void Start() {
        rigidbuddy = GetComponent<Rigidbody>();

        rigidbuddy.AddForce(transform.forward * impulse);
    }

    void explode() {
        explosao.transform.SetParent(null);
        explosao.transform.position += (transform.forward * 0.1f);
        explosao.SetActive(true);

        lendaLib.explosion(transform.position, radius, dano, curva);

        Destroy(gameObject);
    }

    System.Collections.IEnumerator explosionRoutine() {
        GetComponent<AudioSource>().Play();

        this.enabled = false;

        yield return new WaitForSeconds(timer);

        explode();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.transform.root == lendaLib.playerTransf) {
            return;
        }

        rigidbuddy.Sleep();
        rigidbuddy.useGravity = false;
        rigidbuddy.detectCollisions = false;

        transform.position = collision.GetContact(0).point;
        transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);

        if (collision.gameObject.isStatic) {
            StartCoroutine(explosionRoutine());
        } else {
            explode();
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}