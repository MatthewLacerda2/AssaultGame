using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CJ : MonoBehaviour {

    Rigidbody rigidbuddy;

    void Awake() {
        rigidbuddy = GetComponent<Rigidbody>();

        if (rigidbuddy == null) {
            Debug.LogWarning("Wtf po");
            this.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (rigidbuddy.IsSleeping()) {
            rigidbuddy.isKinematic = true;
            GetComponent<Collider>().enabled=false;
            this.enabled = false;
        }
    }
}