using UnityEngine;

[UnityEngine.ExecuteAlways]
public class lensFlareLerp : MonoBehaviour {

    public AnimationCurve gradiente = new AnimationCurve();

    LensFlare flare;
    
    float originalBrightness;

    void Awake() {
        flare = GetComponentInChildren<LensFlare>(true);
        flare.enabled = true;
        originalBrightness = flare.brightness;
    }

    void OnWillRenderObject() {
        float prop = ((transform.position - Camera.main.transform.position).magnitude);

        if(prop >= QualitySettings.shadowDistance) {
            flare.brightness = 0;
        } else {
            flare.brightness = originalBrightness * gradiente.Evaluate(1 - (prop / QualitySettings.shadowDistance));
        }
    }
}