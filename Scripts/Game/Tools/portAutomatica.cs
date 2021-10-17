using UnityEngine;

[RequireComponent(typeof(Animator))]
public class portAutomatica : MonoBehaviour {

    public Vector3 myOriginOffset;

    [Range(1.5f, 25.0f)] public float distAberta = 5.0f;

    public bool includeBots = true;

    public Life[] protegidos;
    int prots = 0;

    Animator anime;

    Vector3 pos {
        get {
            return transform.position + myOriginOffset;
        }
    }

    void Awake() {
        anime = GetComponent<Animator>();

        foreach(Life prot in protegidos) {
            if (prot == null || prot.gameObject.activeSelf == false) {
                continue;
            }

            prot.delegados += exclude;
            prots++;
        }
    }

    // Update is called once per frame
    void Update() {
        if (prots > 0) {
            anime.SetBool("playerInRange", false);
            return;
        }

        float distPlayer = Vector3.Distance(pos, lendaLib.playerPos);
        anime.SetBool("playerInRange", distPlayer <= distAberta);

        if (includeBots) {
            checkForBots();
        }
    }

    void checkForBots() {
        foreach(botInterface bot in lendaLib.probeSystem.botShooters) {
            float dist = Vector3.Distance(bot.transform.position, pos);

            if (dist <= distAberta * 2 / 3) {
                anime.SetBool("botsInRange", true);
                return;
            }
        }

        anime.SetBool("botsInRange", false);
    }

    void exclude(Dano damage) {
        if (damage.killed) {
            prots--;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos, distAberta);
    }
}