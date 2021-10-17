using UnityEngine;

public class meshFader : MonoBehaviour {

    public float fadeDelay = 5.0f;
    public float fadeTime = 3.0f;

    public bool startedFading = false;

    MeshRenderer meshRender;

    void Awake(){
        meshRender=GetComponent<MeshRenderer>();
    }

    void OnEnable() {
        Debug.Log("This.Enabled é impreciso. Use iniciar() instead");
    }

    public void iniciar() {
        iniciar(fadeDelay, fadeTime);
    }

    public void iniciar(float delay, float time){
        fadeDelay = delay;
        fadeTime = time;
        this.enabled=true;
        StartCoroutine(fadeRoutine());
    }

    void OnDisable() {
        Debug.Log("OnDisable é impreciso. Use CancelFade() instead");
        CancelFade();
    }

    bool CancelFade(){
        if(startedFading==true){
            return false;
        }

        StopAllCoroutines();

        this.enabled=false;

        return true;
    }

    System.Collections.IEnumerator fadeRoutine(){
        if(fadeDelay>0){
            yield return new WaitForSeconds(fadeDelay);
        }
        startedFading=true;

        float auxFadeTime = fadeTime;
        while(auxFadeTime>0){
            auxFadeTime-=Time.deltaTime;
            foreach(Material mat in meshRender.materials){
                //float auxFloat = mat.GetFloat("_Cutoff");
                float auxValue = 255/fadeTime*Time.deltaTime;   // wtf?
                mat.SetFloat("_Cutoff", auxValue);
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision) {
        if(!collision.transform.root.gameObject.isStatic) {
            Physics.IgnoreCollision(collision.contacts[0].thisCollider, collision.contacts[0].otherCollider);
        }
    }
}