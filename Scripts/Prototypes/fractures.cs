using UnityEngine;

[RequireComponent(typeof(Life))]
public class fractures : MonoBehaviour {

    public float breakForce = 5;

    public bool considerWeight = true;

    public bool addRigidbodyOnDeath = true;

    void Awake() {
        Debug.Log(transform.childCount + "childs aqui");

        GetComponent<Life>().delegados += takeDamage;

        if (breakForce < 0) {
            Debug.LogWarning("Breakforce nao pode ser negativa, imbecil, ajeita isso aqui");
            breakForce*= -1;
        }
    }

    void takeDamage(Dano damage) {
        float threshold = breakForce;
        if (considerWeight) {
            threshold *= damage.collider.bounds.size.magnitude;
        }

        bool thresholdWon = damage.amount >= threshold;
        bool isChild = damage.collider.transform.IsChildOf(transform);

        if (thresholdWon && isChild){
            damage.collider.transform.SetParent(null);

            Rigidbody rigidbuddy = damage.collider.transform.GetComponent<Rigidbody>();
            if (rigidbuddy == null) {
                if (addRigidbodyOnDeath) {
                    rigidbuddy = damage.collider.gameObject.AddComponent<Rigidbody>();
                    rigidbuddy.mass = damage.collider.bounds.size.magnitude;
                }
            } else {
                rigidbuddy.isKinematic = false;
                rigidbuddy.AddForce(damage.direction * damage.amount);
            }
        }
    }
}