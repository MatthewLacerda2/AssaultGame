using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class raycastInstance : MonoBehaviour {

    //Sim, dá pra fazer isso aqui com Impulse.cs, Hakai.cs e o inspector
    //Mas essa classe é simples e feita com o objetivo especifico
        //E God knows how untrustworthy the physics system is

    public float range = 50.0f;
    public float speed = 70.0f;
    public float damage = 15.0f;
    public float impulso = 5.0f;

    Vector3 origin;
    void Start() {
        origin = transform.position;
    }

    void LateUpdate() {
        float dist = Vector3.Distance(origin, transform.position);
        if (dist > range) {
            Destroy(gameObject);
            return;
        }

        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        Life lie = other.GetComponent<Life>();

        if (lie != null) {
            lie.AddDamage(damage, impulso, transform.position, transform.forward, other, Dano.danoType.tiro);
        }

        Destroy(gameObject);
    }
}