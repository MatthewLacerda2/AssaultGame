using UnityEngine;

[RequireComponent(typeof(Camera))]
public class photoMode : MonoBehaviour {

    //public Transform target;

    public float moveSpeed = 7.5f;
    public float viewSpeed = 2;

    float x, y;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.position += transform.right * 0.01f * moveSpeed;
        } else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftAlt)) {
            transform.position -= transform.right * 0.01f * moveSpeed;
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            transform.position += transform.forward * 0.01f * moveSpeed;
        } else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            transform.position -= transform.forward * 0.01f * moveSpeed;
        }

        if(Input.GetKey(KeyCode.LeftShift)) {
            transform.position += new Vector3(0, 0.01f * moveSpeed, 0);
        } else if(Input.GetKey(KeyCode.LeftControl)) {
            transform.position -= new Vector3(0, 0.01f * moveSpeed, 0);
        }

        x += Input.GetAxis("Mouse X");
        y -= Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(y * viewSpeed, x * viewSpeed, 0);

        if(Input.GetKeyDown(KeyCode.P)) {
            if(Time.timeScale == 1) {
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {

            if(Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }

            Cursor.visible = !Cursor.visible;
        }
    }
}