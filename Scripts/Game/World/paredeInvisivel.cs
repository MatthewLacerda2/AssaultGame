using UnityEngine;

public class paredeInvisivel : MonoBehaviour {

    Collider col;

    void Awake() {
        col = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision) {
        if(!collision.transform.CompareTag("Player") && gameObject.activeSelf) {
            Physics.IgnoreCollision(col, collision.collider);
        }
    }
}