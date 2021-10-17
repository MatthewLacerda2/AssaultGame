using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(playerGun))]
public class adSniper : MonoBehaviour {

    public float zoom = 3;

    Camera mainCam, clipCam;
    Image mira, miraSniper;
    playerGun pg;
    SkinnedMeshRenderer render;

    Vector2 origin2D;
    bool adsing;
    float fov;

    void Awake() {
        pg = GetComponent<playerGun>();
        render = GetComponentInChildren<SkinnedMeshRenderer>();

        origin2D = pg.spread2D;
        mainCam = Camera.main;
        fov = mainCam.fieldOfView;

        foreach (Transform child in mainCam.transform) {
            clipCam = child.GetComponent<Camera>();
            if (clipCam != null) {
                break;
            }
        }

        mira = lendaLib.mainCanvas.mira.GetComponent<Image>();
        miraSniper = lendaLib.mainCanvas.sniperMira.GetComponent<Image>();

        miraSniper.gameObject.SetActive(true);
        miraSniper.enabled = false;

        GetComponent<playerGun>().delegados += tiro;
        lendaLib.playerTransf.GetComponent<Life>().delegados += onDeath;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            adsOFF();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (adsing) {
                adsOFF();
            } else {
                adsON();
            }
        }
    }

    void adsON() {
        adsing = true;

        pg.spread2D = origin2D;
        pg.originFOV = fov / zoom;

        mainCam.fieldOfView = fov / zoom;
        clipCam.fieldOfView = fov / zoom;

        render.enabled = false;

        mira.enabled = false;
        miraSniper.enabled = true;
    }

    void adsOFF() {
        adsing = false;

        pg.spread2D = origin2D * 30.0f;
        pg.originFOV = fov;

        mainCam.fieldOfView = fov;
        clipCam.fieldOfView = fov;

        render.enabled=true;

        if (mira != null) {
            mira.enabled = true;
        }
        if (miraSniper != null) {
            miraSniper.enabled = false;
        }
    }

    void tiro(float firerate) {
        //StartCoroutine(esperar(firerate));
    }

    void onDeath(Dano damage) {
        if (damage.killed) {
            adsOFF();
        }
    }

    void OnEnable() {
        adsOFF();
    }

    void OnDisable() {
        adsOFF();
    }

    IEnumerator esperar(float firerate) {
        adsOFF();
        yield return new WaitForSeconds(firerate);
        adsON();
    }
}