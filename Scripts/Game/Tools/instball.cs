using UnityEngine;

public class instball : MonoBehaviour {

    public GameObject bolaBasket;

    public Vector3 boundBox = new Vector3(4, 2, 4);

    public float instRate = 0.6f;

    float x;

    void Start() {
        x = instRate;
    }

    // Update is called once per frame
    void Update() {
        x += Time.deltaTime;

        if (x >= instRate) {
            instanciar();
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, boundBox);
    }

    void instanciar() {
        float rotX = Random.Range(-180f, 180f);
        float rotY = Random.Range(-180f, 180f);
        float rotZ = Random.Range(-180f, 180f);
        float offX = Random.Range(-boundBox.x, boundBox.x);
        float offY = Random.Range(-boundBox.y, boundBox.y);
        float offZ = Random.Range(-boundBox.z, boundBox.z);

        Vector3 offset = new Vector3(offX, offY, offZ);

        GameObject go = Instantiate(bolaBasket, transform.position + offset, Quaternion.Euler(rotX, rotY, rotZ));

        x = 0;
    }
}