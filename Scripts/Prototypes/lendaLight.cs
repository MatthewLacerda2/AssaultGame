using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class lendaLight : MonoBehaviour {

    public AnimationCurve curve;

    public float range = 20f;

    Light myLight;

    void Awake() {
        myLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update() {
        myLight.intensity = curve.Evaluate(Vector3.Distance(lendaLib.cameraPos, transform.position) / range);
    }
}