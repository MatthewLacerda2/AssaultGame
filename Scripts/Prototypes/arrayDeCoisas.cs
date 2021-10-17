#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class arrayDeCoisas : MonoBehaviour {

    public GameObject exemplo;

    public GameObject[] coisas;

    int num;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (num < coisas.Length) {
            RemoverCoisas();
        }

        if (num > coisas.Length) {
            AddCoisas();
        }

        num = coisas.Length;
    }

    void AddCoisas() {

    }

    void RemoverCoisas() {

    }
}
#endif