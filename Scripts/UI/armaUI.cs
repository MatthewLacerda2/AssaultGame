using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class armaUI : MonoBehaviour {

    playerGun[] guns;
    TextMeshProUGUI textM;

    void Awake() {
        weaponsHolder weapons = lendaLib.playerTransf.GetComponentInChildren<weaponsHolder>();
        List<playerGun> listOfGuns = new List<playerGun>();
        foreach(Transform child in weapons.transform) {
            playerGun gum = child.GetComponent<playerGun>();
            if (gum) {
                listOfGuns.Add(gum);
            }
        }
        guns = listOfGuns.ToArray();

        textM = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        foreach(playerGun gun in guns) {
            if (gun != null) {
                if (gun.gameObject.activeSelf) {
                    textM.text = gun.numBullets.ToString();
                    break;
                }
            }
        }
    }
}