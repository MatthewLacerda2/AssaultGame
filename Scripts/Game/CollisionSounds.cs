using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CollisionSounds : MonoBehaviour {

    public bool collideWithStaticOnly = true;

    public bool balancearOnAwake = false;

    [Space] public AudioClip[] array;

    AudioSource source;

    void Awake() {
        source = GetComponent<AudioSource>();

        if (balancearOnAwake) {
            lendaLib.Balancear(GetComponentsInChildren<Rigidbody>(), GetComponent<Rigidbody>().mass);
        }
    }

    void Update() {
        source.pitch = Time.timeScale;
    }

    void OnCollisionEnter(Collision collision) {
        if (collideWithStaticOnly && collision.gameObject.isStatic == false) {
            return;
        }

        if (source.enabled == false) {
            return;
        }

        if (source.isPlaying && (source.time / source.clip.length > 0.5f)) {
            return;
        }

        source.clip = AudioArray.GetRandomClipFrom(array);
        source.Play();
    }
}