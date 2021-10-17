using UnityEngine;

public class minasGroup : MonoBehaviour {

    public Vector3 box;
    public float range;

    minas[] minhasMina;

    void Awake() {
        minhasMina = GetComponentsInChildren<minas>();

        foreach(minas mina in minhasMina) {
            mina.enabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        float dist = Vector3.Distance(lendaLib.playerPos,transform.position);
        bool hasRange = dist <= range;

        bool isInBox = lendaLib.BoundsContains(transform.position, box, lendaLib.playerPos);

        if (hasRange || isInBox) {
            BlowThemAllUp();
        }
    }

    public void BlowThemAllUp() {
        foreach(minas mina in minhasMina) {
            mina.StartRoutine();
        }

        this.enabled = false;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, box);
    }
}