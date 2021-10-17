using UnityEngine;

public class probeCluster {

    public ProbeSample[] areaSamples;

    public float OverallVisibility() {

        float i = 0;
        foreach (ProbeSample sp in areaSamples) {
            i++;
        }

        return i / areaSamples.Length;
    }
}