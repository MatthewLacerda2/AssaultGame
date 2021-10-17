#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ProbeSystem))]
public class ProbeSysDebugger : MonoBehaviour {

    public GameObject probe;
    public Material On, Off;
    
    public bool enable;

    MeshRenderer[] renderers;

    GameObject tree;
    ProbeSystem probeSys;

    void Start() {
        probeSys = GetComponent<ProbeSystem>();

        if (probe == null || On == null || Off == null) {
            Debug.LogWarning("ProbeSysDebugger nao setado. Terminating...");
            this.enabled = false;
            return;
        }
        /*
        float numSamples = probeSys.samples.Length;
        float frameTime = Mathf.Ceil(numSamples / ProbeSystem.budget);
        Debug.Log(numSamples + " samples. " + frameTime + " frames (" + frameTime / 60 + "s)");
        */
        tree = Instantiate(new GameObject("tree"), Vector3.zero, Quaternion.Euler(0, 0, 0));
        tree.layer = 9; //Clipping layer :D

        alistarProbes();

        tree.SetActive(enable);
    }

    void alistarProbes() {
        if (probeSys.samples.Length == 0) {
            return;
        }

        Quaternion quatZerado = Quaternion.Euler(0,0,0);

        List<MeshRenderer> rendersList = new List<MeshRenderer>();

        for(int i = 0; i < probeSys.samples.Length; i++) {
            Vector3 probeViewPosition = probeSys.samples[i].position - (ProbeSystem.up * 0.9f);
            GameObject go = Instantiate(probe, probeViewPosition, quatZerado, tree.transform);
            go.layer = 9;
            rendersList.Add(go.GetComponent<MeshRenderer>());
        }

        renderers = rendersList.ToArray();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Y)) {
            if (tree) {
                tree.SetActive(!tree.activeSelf);
            }
        }

        for(int i = 0; i < probeSys.samples.Length; i++) {
            if (probeSys.samples[i].playerVisible) {
                renderers[i].material = On;
            } else {
                renderers[i].material = Off;
            }
        }
    }

    void OnEnable() {
        if (tree) {
            tree.SetActive(true);
        }
    }

    void OnDisable() {
        if (tree) {
            tree.SetActive(false);
        }
    }
}
#endif