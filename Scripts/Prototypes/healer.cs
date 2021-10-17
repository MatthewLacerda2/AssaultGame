using UnityEngine;

[RequireComponent(typeof(shooterAI))]
public class healer : MonoBehaviour {

    shooterAI chuta;

    void Awake() {
        chuta = GetComponent<shooterAI>();
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        chuta.dueProcess();

    }

    void checkForBots() {

        return;// true;
    }
}