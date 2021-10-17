using UnityEngine;

[System.Serializable]
public class damageMaster { //it was me, arthur, the author of your pain

    public Dano.danoType damageType = Dano.danoType.tiro;

    [Range(0f,1f)] public float ratio = 1;

    public damageMaster(Dano.danoType type, float rat) {
        rat = Mathf.Clamp(rat, 0f, 1f);

        this.damageType = type;
        this.ratio = rat;
    }
}
