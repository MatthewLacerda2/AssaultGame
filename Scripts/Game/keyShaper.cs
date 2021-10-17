using UnityEngine;

public class keyShaper : MonoBehaviour {

    [Range(0, 100)] public float shapeAmount = 100;

    public float shapeTime = 1;

    public bool interpolate = true;

    SkinnedMeshRenderer skRender;

    bool adicionando;

    void Awake() {
        skRender = GetComponent<SkinnedMeshRenderer>();
    }

    void Start() {
        StartCoroutine(Rotine());
    }

    System.Collections.IEnumerator Rotine() {
        while (interpolate) {
            yield return null;

            float shape = skRender.GetBlendShapeWeight(0);

            if (adicionando) {
                shape += shapeTime * Time.deltaTime * 100;  //Must multiply by 100 to be one second

                if (shape > shapeAmount) {
                    shape = shapeAmount;
                    adicionando = false;
                }
            } else {
                shape -= shapeTime * Time.deltaTime * 100;

                if (shape <= 0) {
                    shape = 0;
                    adicionando = true;
                }
            }

            skRender.SetBlendShapeWeight(0, shape);
        }
    }

    public void interpolateOnce(float start, float finish, float shapeTime) {
        if (adicionando) {
            return;
        }

        Debug.LogWarning("Essa funcao nao tá pronta");  //Da pra economizar umas linhas

        float timer = shapeTime;
        float x = start;

        float value = 1;
        if (start > finish) {
            value = -1;
        }

        skRender.SetBlendShapeWeight(0, start);
        while (timer>0) {
            timer -= Time.deltaTime;
            x += (value * shapeTime * Time.deltaTime * 100);
            skRender.SetBlendShapeWeight(0, x);
        }
        skRender.SetBlendShapeWeight(0, finish);//O  seguro morreu de velho
    }
}