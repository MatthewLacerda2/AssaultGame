using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class impulse : MonoBehaviour {

    public float valor = 5.0f;

    public SnapAxis axis;

    public bool weightProportional = true;

    Rigidbody rigidbuddy;

    // Start is called before the first frame update
    void Awake() {
        rigidbuddy = GetComponent<Rigidbody>();
    }

    void Start() {
        Impulsionar();
    }

    public void Impulsionar() {
        rigidbuddy.AddForce(GetMyVector());
    }

    public Vector3 GetMyVector() {
        Vector3 myVector;

        if (axis == SnapAxis.X) {
            myVector = transform.right;
        }else if (axis == SnapAxis.Y) {
            myVector = transform.forward;
        }else if (axis == SnapAxis.Z) {
            myVector = transform.up;
        } else {
            myVector = Vector3.zero;
        }

        float auxValor = valor;
        if (weightProportional) {
            auxValor *= rigidbuddy.mass;
        }

        return myVector * auxValor;
    }

    void OnDrawGizmosSelected() {
        rigidbuddy = GetComponent<Rigidbody>();
        Vector3 pos = transform.position + GetMyVector();

        Gizmos.color = new Color(0.25f, 0.15f, 0, 0.7f);
        //Gizmos.DrawLine(transform.position, pos);

        Gizmos.DrawWireCube((transform.position + pos) / 2, Vector3.one/30);
    }
}