using UnityEngine;
using System.Collections.Generic;

public class fire : MonoBehaviour{

    List<Life> lifelist = new List<Life>();

    public int damagePerSecond = 60;

    public bool destroyAnyChild;

    void Update(){
		for(int i=0;i<lifelist.Count;i++){
			if(lifelist[i]==null){
				lifelist.RemoveAt(i);
			}
		}
    	foreach(Life lie in lifelist){
		    lie.AddDamage(damagePerSecond * Time.deltaTime, 0, transform.position, new Vector3(0,0,0), lie.GetComponent<Collider>(), Dano.danoType.fisico);
	    }

        if (destroyAnyChild) {
            for (int i = 0; i < transform.childCount; i++) {
                Destroy(transform.GetChild(i));
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        Life auxLife = other.transform.GetComponent<Life>();
        if (auxLife != null) {
            lifelist.Add(auxLife);
        }
    }

    void OnTriggerExit(Collider other) {
        Life auxLife = other.transform.GetComponent<Life>();
        if (auxLife != null) {
            lifelist.Remove(auxLife);
        }
    }
}