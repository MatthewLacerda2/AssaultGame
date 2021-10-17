using UnityEngine;

public class aimLock : MonoBehaviour {

    public GameObject obj;

    void Start() {
        if (obj == null) {
            obj = Camera.main.transform.gameObject;
        }
    }

    void FixedUpdate() {
        //Melhorar: lerpar a rotacao
        //escolher quais axis rotacionar
        if (obj != null) {
            Vector3 auxVec3 = obj.transform.position;
            auxVec3.y = transform.position.y;
            Quaternion rotationAngle = Quaternion.LookRotation(transform.position - auxVec3);
            transform.rotation = rotationAngle;
        }
    }
}