using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class postProcesSettings : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        PostProcessVolume postProcess = GetComponent<PostProcessVolume>();

        int qualityLevel = QualitySettings.GetQualityLevel();

        ScreenSpaceReflections SSRF;
        postProcess.profile.TryGetSettings(out SSRF);
        ScreenSpaceReflectionPresetParameter pr = SSRF.preset;

        switch (qualityLevel) {
            case 0:
                allowHDR(false);
                pr.value = ScreenSpaceReflectionPreset.Lower;
                break;
            case 1:
                allowHDR(false);
                pr.value = ScreenSpaceReflectionPreset.Medium;
                break;
            case 2:
                allowHDR(false);
                pr.value = ScreenSpaceReflectionPreset.High;

                break;
            case 3:
                allowHDR(true);
                pr.value = ScreenSpaceReflectionPreset.Overkill;
                break;
            default:
                Debug.LogWarning("QualityLevel " + qualityLevel + " não foi addressed");
                break;
        }

        SSRF.preset = pr;
    }

    void allowHDR(bool valor) {
        Camera[] cams = lendaLib.playerTransf.GetComponentsInChildren<Camera>();

        foreach(Camera cam in cams) {
            cam.allowHDR = valor;
        }
    }
}