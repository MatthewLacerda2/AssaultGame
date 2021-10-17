using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class vidaSlider : MonoBehaviour {

    Life life;
    Slider slider;

    void Awake() {
        slider = GetComponent<Slider>();

        life = lendaLib.playerTransf.GetComponent<Life>();
    }

    // Update is called once per frame
    void Update() {
        float aux = life.vida;
        if (aux <= 0) {
            gameObject.SetActive(false);
            return;
        }

        if (aux > 100) {
            //Debug.Log("life maior do que 100?");
            aux = 100;
        }

        slider.value = aux;
    }
}