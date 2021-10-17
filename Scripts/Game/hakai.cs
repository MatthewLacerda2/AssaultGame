using UnityEngine;

public class hakai : MonoBehaviour {

    public float timer = 0;

    public hakai(float time) {
        timer = time;
    }

    void Start() {
        if (timer <= 0) {
            //Debug.Log("Timer <= 0 em "+transform.name);
            Destroy(gameObject);
        } else {
            Destroy(gameObject, timer);
        }
    }
}