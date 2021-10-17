using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class escudao : MonoBehaviour {

    Animator anime;
    Life myShooterLife;

    void Awake() {
        GetComponent<Life>().delegados += tomarDano;
    }

    public void fuck(Life shooterLife) {
        myShooterLife = shooterLife;
        shooterLife.delegados += tomarDano;

        anime = shooterLife.GetComponent<Animator>();
    }

    void tomarDano(Dano damage) {
        if (myShooterLife.vida > 0) {
            
            if (damage.impulso > 5.0f) {
                anime.Play("Stagger");
            }

            return;
        }

        foreach(MeshCollider mexico in GetComponentsInChildren<MeshCollider>()) {
            mexico.convex = true;
        }

        transform.parent = null;

        Rigidbody rigidbuddy = GetComponent<Rigidbody>();
        rigidbuddy.isKinematic = false;
        Rigidbody aux = myShooterLife.GetComponent<Rigidbody>();
        rigidbuddy.AddForce(aux.velocity, ForceMode.Impulse);

        GetComponent<Life>().delegados -= tomarDano;
        myShooterLife.delegados -= tomarDano;
        Destroy(this);
    }
}