using UnityEngine;

public class Dano {

    public enum danoType {
        tiro = 0, explosao, fisico, logico, magico
    }

    public Collider collider;
    public Vector3 source, direction;

    public float amount, impulso;

    public bool killed;

    public danoType damageType = danoType.tiro;
    /*
    public Dano() {
        amount = 0;
        impulso = 0;
        source = Vector3.zero;
        direction = Vector3.zero;
        collider = null;
        killed = false;
        damageType = danoType.tiro;
    }
    */
    public Dano(float damage, float impulse, Vector3 sauce, Vector3 dir, Collider col, danoType damType){
        amount = damage;
        impulso = impulse;
        source = sauce;
        direction = dir.normalized;
        collider = col;
        damageType = damType;
    }
}