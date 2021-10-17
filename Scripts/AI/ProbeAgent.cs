using UnityEngine;
using UnityEngine.AI;

//Este codigo é a prova factual de que eu sou foda
[RequireComponent(typeof(botInterface))]
[RequireComponent(typeof(NavMeshAgent))]
public class ProbeAgent : MonoBehaviour {

    public AnimationCurve breakSpeed;

    NavMeshAgent agent;
    ProbeSystem probeSys;

    float stopDist {
        get {
            if (agent) {
                return agent.stoppingDistance;
            } else {
                return 0.666f;  //ksksksk
            }
        }
    }

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start() {
        probeSys = lendaLib.probeSystem;
    }

    public void toggleAndar(bool andar) {
        if (this.enabled == false) {
            return;
        }

        if (agent.isStopped == andar) {
            agent.isStopped = !andar;
        }
    }

    public void SetDestination(Vector3 pos) {
        if (this.enabled == false) {
            return;
        }

        agent.SetDestination(pos);
    }

    void PathWithVisibility(Vector3 position, bool visib) {

        Vector3 target = probeSys.NearestProbePosition(position, visib);
        target -= ProbeSystem.up;

        //float distPlayer = Vector3.Distance(lendaLib.playerPos, transform.position);
        float distTarget = Vector3.Distance(target, transform.position);

        if (distTarget > stopDist * 1.5f) {
            agent.SetDestination(target);
        } else {
            agent.SetDestination(lendaLib.playerPos);
        }
    }

    public void ShortestSight() {
        PathWithVisibility(transform.position, true);
    }

    public void Flankear() {
        PathWithVisibility(lendaLib.playerPos, false);
    }

    void OnEnable() {
        agent.enabled = true;
    }

    void OnDisable() {
        agent.enabled = false;
    }

    public void OnDrawGizmosSelected() {
        if (agent) {
            Gizmos.color = new Color(1, 0, 0.75f, 0.666f);

            foreach (Vector3 corner in agent.path.corners) {
                Gizmos.DrawSphere(corner, 0.333f);
            }

            for (int i = 0; i < agent.path.corners.Length - 1; i++) {
                Vector3 from = agent.path.corners[i];
                Vector3 to = agent.path.corners[i + 1];
                Gizmos.DrawLine(from, to);
            }
        }
    }
}