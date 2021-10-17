#if UNITY_EDITOR
using UnityEngine;

public class levelBuilder : MonoBehaviour {

    public Transform origem;
    public Transform clone;

    public int valor = 1;

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            valor++;

            if (valor == 5) {
                valor = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(origem.position, origem.forward, out hit, 69f);

            if (hasHit == false) {
                return;
            }

            if (valor == 1) {   //copiar

                if (hit.transform.gameObject.isStatic == false) {
                    clone = hit.transform;
                }
            }
            if (valor == 2) {   //colar
                Instantiate(clone.gameObject, hit.point, clone.rotation);
            }
            if (valor == 3) {   //deletar

                if (hit.transform.gameObject.isStatic == false) {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }
}
#endif