using UnityEngine;

public class emissionSlide : MonoBehaviour {

    public AnimationCurve curva;
    public Material mat;

    MeshRenderer mRender;

    public float x, timer;

    void Awake() {
        mRender = GetComponent<MeshRenderer>();

        timer = curva.keys[curva.keys.Length - 1].time;
    }

    // Update is called once per frame
    void Update() {
        //Debug.LogWarning("Este script nao funciona");

        x += Time.deltaTime;
        if (x > timer) {
            x = 0;
        }

        Color corAtual = mat.GetColor("_EmissionColor");
        Color corNova = corAtual * Mathf.LinearToGammaSpace(x);

        mat.SetColor("_EmissionColor", corNova);

        Material[] mats = new Material[] { mat };
        mRender.materials = mats;
    }
}