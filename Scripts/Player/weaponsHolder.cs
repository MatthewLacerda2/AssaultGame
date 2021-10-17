using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class weaponsHolder : MonoBehaviour {

    [HideInInspector] public playerGun[] guns;

    [HideInInspector] public int startIndex;

    int index, lastIndex;

    void AwakeAux() {
        List<playerGun> gunsList = new List<playerGun>();

        foreach(Transform child in transform) {
            playerGun pg = child.GetComponent<playerGun>();

            if (pg) {
                int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                if (sceneIndex >= pg.scenes.x && sceneIndex <= pg.scenes.y) {
                    gunsList.Add(pg);
                }
            }
        }

        guns = gunsList.ToArray();
    }

    // Start is called before the first frame update
    void Start() {
        AwakeAux();

        index = startIndex;
        toggleWeapon(index);

        lastIndex = index + 1;
        if (lastIndex == guns.Length) {
            lastIndex = index - 1;
        }
        if (lastIndex == -1) {
            lastIndex = 0;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        int auxIndex = index;

        if (Input.GetKeyDown(KeyCode.Q)) {
            auxIndex = lastIndex;
        }

        for(int i = 0; i < transform.childCount + 1; i++) {
            if (Input.GetKeyDown(i.ToString())) {
                auxIndex = i - 1;
                break;
            }
        }
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            auxIndex = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            auxIndex = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            auxIndex = 2;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            auxIndex = 3;
        }
        */
        if (auxIndex != index && guns.Length - 1 >= auxIndex) {
            bool toggled = toggleWeapon(auxIndex);

            if (toggled) {
                lastIndex = index;
                index = auxIndex;
            }
        }
    }

    bool toggleWeapon(int indice) {
        //Falta evitar o disable da arma quando não dá pra trocar

        int i = 0;
        foreach (Transform child in transform) {
            playerGun pg = child.GetComponent<playerGun>();
            if (pg) {
                pg.gameObject.SetActive(i == indice);
            }

            i++;
        }

        return true;
    }
}