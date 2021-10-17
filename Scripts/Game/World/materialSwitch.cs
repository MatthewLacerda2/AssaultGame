using UnityEngine;

public class materialSwitch : MonoBehaviour {

    public Material[] mats;

    Material[] auxMats;
    MeshRenderer meshRender;

    [Min(1/60)] public float timer = 1f;

    bool switched;

    void Awake() {
        meshRender = GetComponent<MeshRenderer>();
        auxMats = meshRender.materials;
    }

    void Start() {
        StartCoroutine(Rotina());
    }

    System.Collections.IEnumerator Rotina() {
        while (true) {

            if (this.enabled == false) {
                yield return null;
            }

            yield return new WaitForSeconds(timer);

            if (switched) {
                meshRender.materials = auxMats;
            } else {
                meshRender.materials = mats;
            }

            switched = !switched;
        }
    }
}