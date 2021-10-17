using UnityEngine;
using System.Collections;

public class powerup : MonoBehaviour {

    [System.Serializable] public enum powerType {
        infiniteAmmo, doubleDamage, invencibility
    }

    public powerType myType;
    public float duracao;

    void OnTriggerEnter(Collider other) {
        if (other.transform.root != lendaLib.playerTransf) {
            return;
        }

        switch (myType) {
            case powerType.infiniteAmmo:
                infiniteAmmo();
                break;
            case powerType.doubleDamage:
                doubleDamage();
                break;
            case powerType.invencibility:
                invencibility();
                break;
        }

        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null) {
            ps.Stop();
            ps.Clear();
        }

        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<AudioSource>().Play();
        this.enabled = false;
    }

    void infiniteAmmo() {
        StartCoroutine(infiniteAmmoRoutine());
    }

    void doubleDamage() {
        StartCoroutine(doubleDamageRoutine());
    }

    void invencibility() {
        StartCoroutine(invencibilityRoutine());
    }

    IEnumerator infiniteAmmoRoutine() {
        weaponsHolder wph = lendaLib.playerTransf.GetComponentInChildren<weaponsHolder>();

        int[] auxMax = new int[wph.guns.Length];
        int[] auxNum = new int[wph.guns.Length];

        for(int i = 0; i < wph.guns.Length; i++) {
            auxMax[i] = wph.guns[i].maxBullets;
            auxNum[i] = wph.guns[i].numBullets;

            wph.guns[i].maxBullets = 999;
        }

        float aux = duracao;
        while (aux > 0) {

            foreach(playerGun pg in wph.guns) {
                pg.numBullets = 999;
            }

            aux -= Time.deltaTime;

            yield return null;
        }

        for (int i = 0; i < wph.guns.Length; i++) {
            wph.guns[i].maxBullets = auxMax[i];
            wph.guns[i].numBullets = auxNum[i];
        }
    }

    IEnumerator doubleDamageRoutine() {
        weaponsHolder wph = lendaLib.playerTransf.GetComponentInChildren<weaponsHolder>();

        foreach (playerGun pg in wph.guns) {
            if (pg != null) {
                pg.damage *= 2f;
            }
        }

        yield return new WaitForSeconds(duracao);

        foreach (playerGun pg in wph.guns) {
            if (pg != null) {
                pg.damage /= 2f;
            }
        }
    }

    IEnumerator invencibilityRoutine() {

        Life lf = lendaLib.playerTransf.GetComponent<Life>();

        int length = 5; //quant de danoTypes
        damageMaster[] origin = lf.masters;
        damageMaster[] aux = new damageMaster[length];

        for (int i = 0; i < length; i++) {
            aux[i].damageType = (Dano.danoType)i;
            aux[i].ratio = 0;
        }

        lf.masters = aux;

        yield return new WaitForSeconds(duracao);

        lf.masters = origin;

    }
}