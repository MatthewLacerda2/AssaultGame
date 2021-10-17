using UnityEngine;

public class materialSlide : MonoBehaviour {

    [System.Serializable] public class slider {

        public Vector2 scale = new Vector2(1, 1);
        public Vector2 offset = new Vector2(0, 0);

    }

    slider[] slides;

    //--------------

    public Vector2 scale = new Vector2(1, 1);
    public Vector2 offset = new Vector2(0, 0);

    public bool continuousOffset = true;

    const string mainText = "_MainTex";

    Vector2[] originOffset;
    Renderer rend;

    void Awake() {
        rend = GetComponentInChildren<Renderer>();

        if (offset != Vector2.zero) {
            originOffset = new Vector2[rend.materials.Length];
            int i = 0;
            foreach (Material mat in rend.materials) {
                originOffset[i] = mat.GetTextureOffset(mainText);
                i++;
            }
        }

        if(continuousOffset == false) {
            this.enabled = false;

            int j = 0;
            foreach (Material mat in rend.materials) {
                Vector2 current = mat.GetTextureOffset(mainText);
                mat.SetTextureOffset(mainText, current + offset);
                j++;
            }
        }

        if (scale == Vector2.one) {
            return;
        }

        foreach (Material mat in rend.materials) {
            Vector2 myScale = mat.GetTextureScale(mainText);
            mat.SetTextureScale(mainText, new Vector2(myScale.x * scale.x, myScale.y * scale.y));
        }
    }

    void Update() {
        if (offset == Vector2.zero) {
            return;
        }

        int i = 0;
        foreach(Material mat in rend.materials) {

            Vector2 current = mat.GetTextureOffset(mainText);
            Vector2 displace = offset * Time.deltaTime;

            mat.SetTextureOffset(mainText, current + displace);
            i++;
        }
    }
}