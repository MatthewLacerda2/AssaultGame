using UnityEngine;

[RequireComponent(typeof(botInterface))]
public class autoBotInterface : MonoBehaviour {

    public Transform visao;

    public float range = 5.0f;

    botInterface botInterf;

    float timeNaVista;

    void Awake() {
        botInterf = GetComponent<botInterface>();
    }

    // Update is called once per frame
    public void Update() {

        bool playerWithinRange = Vector3.Distance(lendaLib.playerPos, transform.position) <= range;
        bool amVisible = lendaLib.isInViewFrustum(lendaLib.cameraPos, Camera.main.transform.forward, Camera.main.fieldOfView, transform.position);

        bool isPlayerVisible = false;
        if (visao != null) {
            visao.LookAt(lendaLib.cameraPos);
            isPlayerVisible = lendaLib.RaycastVisibility(visao.position, lendaLib.playerTransf, range);
        }

        if (isPlayerVisible) {
            timeNaVista += Time.deltaTime;
        } else {
            timeNaVista -= Time.deltaTime;
        }

        botInterf.Refresh(amVisible, false, isPlayerVisible, playerWithinRange, timeNaVista);
    }
}