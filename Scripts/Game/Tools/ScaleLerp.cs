using UnityEngine;
using System.Collections;

public class ScaleLerp : MonoBehaviour {

    public AnimationCurve curva;

    public bool destroyLater = false;
    public bool timeScaled = true;

    float timer;

    void Start() {
        timer = lendaLib.GetLastKeyTime(curva);

        StartCoroutine(lerping());

        if (destroyLater) {
            Destroy(gameObject, timer);
        }
    }

    IEnumerator lerping() {
        float timePassed = 0;

        while (timePassed <= timer) {
            if (timeScaled) {
                timePassed += Time.deltaTime;
            } else {
                timePassed += Time.unscaledDeltaTime;
            }

            float x = curva.Evaluate(timePassed);
            transform.localScale = new Vector3(x, x, x);

            yield return null;
        }
    }
}