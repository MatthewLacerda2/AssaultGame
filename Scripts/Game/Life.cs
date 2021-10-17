using UnityEngine;

[RequireComponent(typeof(Collider))]    //Melhor deixar isso aqui
public class Life : MonoBehaviour {

    public float vida = 50;

    public bool hasBlood = false;

    public damageMaster[] masters;

    public delegate void listener(Dano dano);
    public listener delegados;
    
    //6 parametros, novo recorde!
    public void AddDamage(float damage, float impulso, Vector3 source, Vector3 direcao, Collider col, Dano.danoType danType) {
        AddDamage(new Dano(damage, impulso, source, direcao, col, danType));
    }

    public void AddDamage(Dano dano) {
        dano.direction.Normalize(); //redundancia

        foreach(damageMaster master in masters) {
            if (master.damageType == dano.damageType) {
                dano.amount *= master.ratio;
                break;
            }
        }

        vida -= dano.amount;

        if (vida <= 0 && vida + dano.amount > 0) {
            dano.killed = true;
            //Debug.Log(transform.name + " morreu");
        }

        if (delegados != null) {
            delegados(dano);
        }
    }
}
