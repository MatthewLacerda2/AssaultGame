using UnityEngine;
using UnityEngine.UI;
using TMPro;

//add a UI
public class bulletTime : MonoBehaviour {

    public AnimationCurve shortTime;

    public float cooldown = 20.0f;

    public float exitTime = 0.4f;

    [HideInInspector] public bool isBendingTime;

    bool canBend = true;

    public void executar() {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (isBendingTime) {
                sair(false);
            } else if(canBend){
                iniciar(shortTime);
            }
        }
    }

    public bool iniciar(AnimationCurve curva) {
        if (isBendingTime) {
            return false;
        }

        isBendingTime = true;

        StartCoroutine(iniciarRoutine(curva));

        return true;
    }

    System.Collections.IEnumerator iniciarRoutine(AnimationCurve curva) {
        float x = 0;
        float length = curva.keys[curva.keys.Length - 1].time;

        canBend = false;

        while (x < length) {

            Time.fixedDeltaTime = 1f / (60f / Time.timeScale);

            if (Time.timeScale == 0) {
                yield return null;
            }

            x += Time.unscaledDeltaTime;
            Time.timeScale = curva.Evaluate(x);
            yield return null;
        }

        seguroMorreuDeVelho();

        yield return new WaitForSeconds(cooldown);

        canBend = true;
    }

    public void sair(bool abrupt) {
        StopAllCoroutines();

        if (abrupt) {
            seguroMorreuDeVelho();
            return;
        }

        StartCoroutine(sairRoutine());
    }

    System.Collections.IEnumerator sairRoutine() {
        float interpValue = 1 - Time.timeScale;

        float auxTime = exitTime;

        while (auxTime > 0) {
            if (Time.timeScale == 0) {
                yield return null;
            }

            auxTime -= Time.unscaledDeltaTime;
            Time.timeScale += (interpValue * Time.unscaledDeltaTime);            
            yield return null;
        }

        seguroMorreuDeVelho();
    }

    void seguroMorreuDeVelho() {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 1f / 60f;
        isBendingTime = false;

        StartCoroutine(enableBend());
    }

    System.Collections.IEnumerator enableBend() {
        canBend = false;
        yield return new WaitForSeconds(cooldown);
        canBend = true;
    }
}