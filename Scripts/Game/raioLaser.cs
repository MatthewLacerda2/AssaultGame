using UnityEngine;

public class raioLaser : MonoBehaviour {

    public float damagePerSecond = 70;

    public Life[] protegidos;

    AudioSource source;
    GameObject flare;

    void Awake() {
        source = GetComponentInChildren<AudioSource>();

        gameObject.layer = 2;   //raycastTransparent

        if (protegidos.Length == 0) {
            GetComponent<BoxCollider>().enabled = false;
        }

        foreach(Life prote in protegidos) {
            if (prote != null) {
                if (prote.gameObject.activeSelf) {
                    prote.delegados += morreu;
                }
            }
        }
    }

    int mortes = 0;
    void morreu(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        mortes++;

        if (mortes == protegidos.Length) {
            gameObject.SetActive(false);
        }
    }

    void audioNaReta() {
        Vector3 point = lendaLib.NearestPointInLine(transform.position, transform.forward, lendaLib.cameraPos);
        Vector3 reta = point - transform.position;

        reta = lendaLib.Vector3Clamp(reta, 0, transform.lossyScale.magnitude);
        point = transform.position + reta;

        source.transform.position = point;
    }

    void tentarDarDano(Collider other) {
        Life lie = other.GetComponent<Life>();

        if (lie != null) {
            lie.AddDamage(damagePerSecond * Time.deltaTime, 0, transform.position, Vector3.zero, other, Dano.danoType.logico);
        }
    }

    void OnCollisionEnter(Collision collision) {
        tentarDarDano(collision.collider);
    }

    void OnCollisionStay(Collision collision) {
        tentarDarDano(collision.collider);
    }

    void OnTriggerEnter(Collider other) {
        tentarDarDano(other);
    }

    void OnTriggerStay(Collider other) {
        tentarDarDano(other);
    }
}