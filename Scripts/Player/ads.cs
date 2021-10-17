using UnityEngine;

public class ads : MonoBehaviour {

    public Vector3 adsPos;

    public float adsSpeed=0.15f;

    public float multiplicador = 1;

    Vector3 originalPos;

	// Use this for initialization
	void Start () {
        originalPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 interp = Vector3.zero;

        if (Input.GetKey(KeyCode.Mouse1)) {
            interp = adsPos - transform.localPosition;
        } else {
            interp = originalPos - transform.localPosition;
        }

        float interpTime = adsSpeed * Time.deltaTime*multiplicador;

        interp *= interpTime;

        transform.localPosition += interp;
	}
}