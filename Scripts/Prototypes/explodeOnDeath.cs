using UnityEngine;

[RequireComponent(typeof(Life))]
public class explodeOnDeath : MonoBehaviour {

    public GameObject explosao;
    public AnimationCurve damageCurve;

    public float radius, damage, deathTime = 0;

    public bool signMeUp = false;

    void Awake() {
        if (signMeUp) {
            GetComponent<Life>().delegados += morreno;
        }
    }

    void morreno(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        kaboom();
    }

    public void kaboom() {
        if (signMeUp) {
            GetComponent<Life>().delegados -= morreno;
        }

        StartCoroutine(morrenoRoutine());
    }

    System.Collections.IEnumerator morrenoRoutine() {
        yield return new WaitForSeconds(deathTime);

        explosao.transform.SetParent(null);
        explosao.SetActive(true);

        lendaLib.explosion(transform.position, radius, damage, damageCurve);

        Destroy(gameObject);
    }
}