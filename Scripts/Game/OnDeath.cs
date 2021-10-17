using UnityEngine;

[RequireComponent(typeof(Life))]
public class OnDeath : MonoBehaviour {

    public Material[] mats;

    Life myLife;

    void Awake(){
        myLife = GetComponent<Life>();
        myLife.delegados += tomarDano;
    }

    void tomarDano(Dano dano){
        if (myLife.vida <= 0) {
            MeshRenderer render = GetComponent<MeshRenderer>();
            if (render != null) {
                render.materials = mats;
            }

            Transform child=transform.GetChild(0);
            while (child!=null) {
                child.gameObject.SetActive(true);
                child.SetParent(null);
                child = transform.GetChild(0);
            }

            Destroy(gameObject);
        }
    }
}