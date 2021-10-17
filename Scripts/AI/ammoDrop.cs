using UnityEngine;

[RequireComponent(typeof(CollisionSounds))]
public class ammoDrop : MonoBehaviour {

    public AudioClip clip;
    public Transform pos;
    public gunType[] guns;

    AudioSource sounds;

    public float range = 2.0f;

    void Awake() {
        sounds = GetComponent<AudioSource>();

        if (pos == null) {
            pos = transform;
        }
    }

    private void OnEnable() {
        transform.SetParent(null);

        foreach(Collider col in GetComponentsInChildren<Collider>()) {
            col.isTrigger = false;
        }

        Rigidbody rigidbuddy = GetComponent<Rigidbody>();
        if (rigidbuddy != null) {
            rigidbuddy.Sleep(); // :D
        }
    }

    // Update is called once per frame
    void Update() {
        float distPlayer = Vector3.Distance(lendaLib.playerPos, pos.position);
        if (distPlayer <= range && guns != null) {
            GivePlayerAmmo();
        }

        if (distPlayer <= range * 2f) {
            Vector3 dir = (lendaLib.playerPos - pos.position).normalized;
            transform.position += (dir * 8 * Time.deltaTime);
        }
    }

    void GivePlayerAmmo() {
        //this.enabled = false;

        weaponsHolder wph = lendaLib.playerTransf.GetComponentInChildren<weaponsHolder>();
        
        foreach(playerGun gun in wph.guns) {
            if (gun != null) {
                foreach (gunType type in guns) {
                    if (gun.transform.name == type.nome) {
                        gun.numBullets += type.amount;
                        break;
                    }
                }
            }
        }

        guns = null;

        CollisionSounds cols = GetComponent<CollisionSounds>();
        cols.enabled = false;

        sounds.clip = clip;
        sounds.Play();

        Destroy(gameObject, clip.length);
    }

    void OnDrawGizmosSelected() {
        if (this.enabled == false) {
            return;
        }

        if (pos == null) {
            pos = transform;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos.position, range);
    }
}