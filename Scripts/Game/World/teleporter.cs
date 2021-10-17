using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class teleporter : MonoBehaviour {

    public Bounds boundBox;

    [Space] public Vector3 destination;

    public float orientation;

    AudioSource source;

    void Awake() {
        source = GetComponent<AudioSource>();
    }

    void Update() {
        Vector3 center = transform.position + boundBox.center;
        bool playerWithinBounds = lendaLib.BoundsContains(center, boundBox.extents, lendaLib.playerPos);

        if (playerWithinBounds) {
            lendaLib.playerTransf.position = destination;
            lendaLib.playerTransf.rotation = Quaternion.Euler(0, orientation, 0);   //unity physics system aint havin it, yo
            source.Play();
        }
    }

    void OnDrawGizmosSelected() {
        if (this.enabled == false) {
            return;
        }

        Gizmos.color = new Color(0, 1, 0, 0.8f);
        Gizmos.DrawSphere(destination, 0.666f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + boundBox.center, boundBox.extents);
    }
}