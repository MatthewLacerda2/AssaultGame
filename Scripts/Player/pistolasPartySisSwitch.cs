using UnityEngine;

public class pistolasPartySisSwitch : MonoBehaviour {

    public ParticleSystem partyDir, partyEsq;

    playerGun pg;

    bool esq = false;

    void Awake() {
        pg = GetComponent<playerGun>();
        pg.delegados += SwitchParties;
    }

    void SwitchParties(float obrigatorio) {
        esq = !esq;

        if (esq) {
            pg.partySis = partyEsq;
        } else {
            pg.partySis = partyDir;
        }
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}