#if UNITY_EDITOR
using UnityEngine;

public class test : MonoBehaviour {

    [Range(0, 0.2f)] public float margem = 0.1f;

    public float erro = 0.0015f;

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
            lendaLib.SomaRiemann(GetComponent<AutoMoveAndRotate>().posOffset.curve, 512);
            //lendaLib.SomaRiemannConvergida(GetComponent<AutoMoveAndRotate>().posOffset.curve, margem, erro);
        }
    }
}
#endif