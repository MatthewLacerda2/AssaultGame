using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(shooterAI))]
public class priest : MonoBehaviour {

    public float navSpeed = 1.4f, animSpeed = 1.25f, damageBump = 1.5f, firerateBump = 1.2f;

    public GameObject visualBump;

    List<shooterAI> chutas;

    void Start() {
        chutas = new List<shooterAI>();

        foreach(botWait bw in lendaLib.probeSystem.botWaits) {
            addMotherfucker(bw.GetComponent<botInterface>());
        }
        foreach(botInterface bt in lendaLib.probeSystem.botShooters) {
            addMotherfucker(bt.GetComponent<botInterface>());
        }

        if (visualBump != null) {
            foreach(shooterAI chuta in chutas) {
                Instantiate(visualBump, chuta.transform.position, Quaternion.identity, chuta.transform);
            }

            Debug.LogWarning("Faltou deletar o visualBump quando os bot morrerem");
        }

        GetComponent<Life>().delegados += morreno;
    }

    void addMotherfucker(botInterface bt) {
        if (bt.transform == transform) {
            return;
        }

        Life lie = bt.GetComponent<Life>();
        shooterAI chuta = bt.GetComponent<shooterAI>();

        if (lie.hasBlood == false || chuta == null) {   //hasBlood pra so pegar seres vivos já que é demoniaco :D
            return;
        }

        NavMeshAgent agent = bt.GetComponent<NavMeshAgent>();
        Animator anime = bt.GetComponent<Animator>();

        agent.speed *= navSpeed;
        anime.speed *= animSpeed;
        chuta.dano *= damageBump;
        chuta.firerate *= firerateBump;

        chutas.Add(chuta);
    }

    void morreno(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        for(int i = 0; i < chutas.Count; i++) {
            removeBuff(chutas[i]);
        }

        GetComponent<Life>().delegados -= morreno;
    }

    public void removeBuff(shooterAI shooter) {
        if (chutas.Contains(shooter) == false) {
            return;
        }

        NavMeshAgent agent = shooter.GetComponent<NavMeshAgent>();
        Animator anime = shooter.GetComponent<Animator>();

        agent.speed /= navSpeed;
        anime.speed /= animSpeed;
        shooter.dano /= damageBump;
        shooter.firerate /= firerateBump;

        chutas.Remove(shooter);
    }
}