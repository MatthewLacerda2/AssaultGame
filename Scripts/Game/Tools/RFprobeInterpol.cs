using UnityEngine;

public class RFprobeInterpol : MonoBehaviour {

    ReflectionProbe[] rfProbes;

    public float timer;

    int index = 0;

    void Awake() {
        rfProbes = GetComponentsInChildren<ReflectionProbe>();

        foreach (ReflectionProbe rp in rfProbes) {
            rp.intensity = 0;
        }
        rfProbes[index].intensity = 1;
    }

    // Update is called once per frame
    void Update() {
        int nextIndex = index + 1;
        if (nextIndex >= rfProbes.Length) {
            nextIndex = 0;
        }

        rfProbes[index].intensity -= (timer / Time.deltaTime);
        rfProbes[nextIndex].intensity += (timer / Time.deltaTime);

        if (rfProbes[index].intensity <= 0) {
            index++;

            if (index >= rfProbes.Length) {
                index = 0;
            }
        }
    }
}