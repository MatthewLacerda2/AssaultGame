using UnityEngine;
using System.Collections;

public class positionLerp : MonoBehaviour {

    public Vector3 movement;

    public float timer;
    public bool destruirGameobject;

    void Start() {
        StartCoroutine(lerping());
    }

    IEnumerator lerping() {
        while(timer>0){
            timer-=Time.deltaTime;
            transform.position += (movement * Time.deltaTime);
            yield return null;
        }
        
        if(destruirGameobject){
            Destroy(gameObject);
        }else{
            Destroy(this);
        }
    }
}