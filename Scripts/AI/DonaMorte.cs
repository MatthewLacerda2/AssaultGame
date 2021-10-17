using UnityEngine;

public class DonaMorte : MonoBehaviour {

    public RuntimeAnimatorController anime;

    [Range(0, 100.0f)] public float minImpulso = 15.0f;
    [Range(0, 50.0f)] public float minDamage = 15.0f;
    [Range(0, 1.0f)] public float minNormal = 0.5f; //Podia ser usada como atributo pras mortes

    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        float magni = animator.GetFloat("magni");

        if (magni > 0) {
            animator.SetFloat("magni", magni - (Time.deltaTime * 20));
        } else if (magni <= 0) {
            magni = 0;
        }
    }

    public bool killThisMotherfucker(Dano damage) {
        if (anime != null) {
            animator.runtimeAnimatorController = anime;
        } else {
            return false;
        }

        Vector3 auxVec3 = lendaLib.playerPos;
        auxVec3.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(auxVec3 - transform.position);

        bool animeMorte = Twitch(damage);

        return animeMorte;
    }

    public bool criteriaMet(Dano damage) {
        if (damage.impulso < minImpulso) {
            //Debug.Log("Impulso" + minImpulso + "/" + damage.impulso);
            return false;
        }

        if (damage.amount < minDamage) {
            //Debug.Log("Damage " + damage.amount + "/" + minDamage);
            return false;
        }

        float angle = Vector3.Angle(transform.forward, damage.direction);
        angle /= 1.5f;
        if (angle / 180.0f < minNormal) {
            //Debug.Log("False Twitch");
            return false;
        }

        return true;
    }

    public bool Twitch(Dano damage) {
        if (criteriaMet(damage) == false) {
            return false;
        }

        string sourceName = damage.collider.name;

        if (sourceName.Contains("Arm") || sourceName.Contains("Hand")) {
            animator.SetTrigger("Arm");

        } else if (sourceName.Contains("Leg") || sourceName.Contains("Foot")) {
            animator.SetTrigger("Leg");

        } else if (sourceName.Contains("Head") || sourceName.Contains("Neck")) {
            animator.SetTrigger("Head");

        } else {
            animator.SetTrigger("Body");
        }

        if (sourceName.Contains("Left")) {
            animator.SetTrigger("Left");
        }

        if (sourceName.Contains("Right")) {
            animator.SetTrigger("Right");
        }

        float magni = animator.GetFloat("magni");
        if (magni < damage.impulso) {
            magni = damage.impulso;
        }
        animator.SetFloat("magni", magni);

        return true;
    }
}