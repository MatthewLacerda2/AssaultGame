using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System.Linq;

public class ProbeSystem : MonoBehaviour {

    [HideInInspector] public int numShooters;
    [HideInInspector] public int numInView;

    public static bool isChapterFirstLoad = true;
    public static bool isSceneFirstLoad = true;
    public static Vector3 up = new Vector3(0, 1.8f, 0);

    [TextArea(1, 7)] public string Anotacao;

    public float targetTime = 25.0f;
    public int startIndex;
    public string levelAdjective = "";

    [Space] public float timeThreshold = 4.2f;
    public int maxOnScreen = 9;
    public int maxShooters = 5;
    public int maxBotsFalando = 3;
    public int nextSceneIndex = -1;

    [Space]
    public GameObject[] destroyOnAwake;
    public ProbeSample[] samples;
    public probeArea[] Area = new probeArea[1];

    [HideInInspector] public List<botInterface> botShooters = new List<botInterface>();
    [HideInInspector] public List<botWait> botWaits = new List<botWait>();
    cronometro kronos;

    float timerThres;
    int num, mortos;

    const float espacamento = 2f;
    public static int budget =      64;
    const float samplingDist = ((int)(36 / espacamento) + 1) * (Mathf.PI / 4) * 1.414f * 2;  //*2 pq D=2R    //Problema do Circulo de Gauss Aproximado
    //Testei e TÁ CERTO, CARALHO

    void Awake() {

        if (lendaLib.probeSystem != null) {
            Debug.LogError("Só pode ter um probeSys, porra");
            this.enabled = false;
            return;
        }

        lendaLib.probeSystem = this;

        foreach (GameObject go in destroyOnAwake) {
            if (go != null) {
                Destroy(go);
            }
        }

        GameObject canvas = GameObject.Find("Canvas");

        lendaLib.mainCanvas = canvas.GetComponent<mainCanvas>();
        kronos = canvas.GetComponentInChildren<cronometro>();

        lendaLib.playerTransf.GetComponentInChildren<weaponsHolder>().startIndex = startIndex;

        AlistarProbes();

        Debug.Log(samples.Length + " samples");

    }

    void AlistarProbes() {
        if (Area.Length < 1) {
            Debug.Log("Faltou Setar os clusters, seu imbecil!");

            Area = new probeArea[1];
            Area[0].box = new Vector2(80, 80);
            return;
        }

        List<ProbeSample> samplesList = new List<ProbeSample>();

        float searchRadius = espacamento / 2;

        for (int c = 0; c < Area.Length; c++) {

            float xLength = Area[c].box.x / 2, zLength = Area[c].box.y / 2;
            xLength -= searchRadius / 2;
            zLength -= searchRadius / 2;

            NavMeshHit reachHit;
            NavMeshPath path = new NavMeshPath();
            NavMesh.SamplePosition(lendaLib.playerPos, out reachHit, espacamento, NavMesh.AllAreas);
            Vector3 reach = reachHit.position;

            bool impar = false;

            for (float i = Area[c].center.x - xLength; i < Area[c].center.x + xLength; i += espacamento) {
                
                if (impar) {
                    zLength -= espacamento / 2;
                } else {
                    zLength += espacamento / 2;
                }
                
                for (float j = Area[c].center.z - zLength; j < Area[c].center.z + zLength; j += espacamento) {
                    NavMeshHit navHit;
                    Vector3 pos = new Vector3(i, Area[c].center.y, j);

                    bool hasHit = NavMesh.SamplePosition(pos, out navHit, searchRadius, NavMesh.AllAreas);
                    if (!hasHit) {
                        continue;
                    }

                    NavMesh.CalculatePath(navHit.position, reach, NavMesh.AllAreas, path);  //Island problem solved!
                    if (path.status == NavMeshPathStatus.PathComplete) {
                        reach = navHit.position;
                    } else {
                        continue;
                    }

                    ProbeSample samp = new ProbeSample();
                    samp.position = navHit.position + up;
                    samp.amInFrustum = lendaLib.isInViewFrustum(lendaLib.cameraPos, Camera.main.transform.forward, 90.0f, samp.position);
                    samp.distToPlayer = Vector3.Distance(lendaLib.playerPos, samp.position);
                    samp.index = samplesList.Count;
                    samp.playerVisible = false;
                    //samp.playerVisible = lendaLib.isInViewFrustum(lendaLib.cameraPos, Camera.main.transform.forward, 90.0f, samp.position);    //~ == =    //~...

                    foreach (probeArea cluster in Area) {
                        if (lendaLib.BoundsContains(cluster.center, new Vector3(cluster.box.x, espacamento * 2, cluster.box.y), samp.position)) {
                            samplesList.Add(samp);
                            break;
                        }
                    }
                }

                impar = !impar;
            }
        }

        samples = samplesList.ToArray();

        botWaits = GetComponentsInChildren<botWait>().ToList();
    }

    // Start is called before the first frame update
    void Start() {

        comunicador.maxFaladores = maxBotsFalando;

        foreach (Life lie in GetComponentsInChildren<Life>()) {
            hologramAI hai = lie.GetComponent<hologramAI>();
            if (hai != null) {
                continue;
            }

            if (lie.tag == "Enemy") { //Sem esse If, ia contar os lifes das armas
                lie.delegados += contagem;
                num++;
            }
        }
    }

    // Update is called once per frame
    void Update() {

        if (Time.timeScale == 0) {
            return;
        }

        Profiler.BeginSample("probeSys");
        
        RefreshProbes();

        contarAtiradores();

        encherSacoJogador();

        Profiler.EndSample();
    }

    int index = 0;
    void RefreshProbes() {

        for (int i = 0; i < budget; i++) {

            Vector3 target = lendaLib.cameraPos;
            Vector3 pos = samples[index].position;
            Vector3 dir = (target - pos).normalized;

            float distToPlayer = Vector3.Distance(target, pos);
            samples[index].distToPlayer = distToPlayer;

            samples[index].amInFrustum = lendaLib.isInViewFrustum(lendaLib.cameraPos, Camera.main.transform.forward, 90.0f, samples[index].position);

            RaycastHit rayHit;
            bool hasHit = Physics.Raycast(pos, dir, out rayHit, distToPlayer);

            if (hasHit) {

                bool atingiuBackface = Vector3.Dot(dir, rayHit.normal) >= 0;
                bool atingiuPlayer = lendaLib.playerTransf == rayHit.transform.root;

                hasHit = !atingiuBackface && atingiuPlayer;
                samples[index].playerVisible = hasHit;
            } else {
                samples[index].playerVisible = true;
            }

            index++;
            if (index == samples.Length) {
                index = 0;
            }
        }
        
        //samples.OrderBy(sp => sp.playerVisible).ThenBy(sp => sp.amInFrustum).ThenBy(sp => sp.distToPlayer);
    }

    void contarAtiradores() {

        if (botShooters.Count == 0) {
            numShooters = 0;
            return;
        }

        int auxShooters = 0;
        int auxView = 0;
        for (int i = 0; i < botShooters.Count; i++) {
            if (botShooters[i].state == enumBotState.MirarTiro) {
                auxShooters++;
            }
            if (botShooters[i].amVisible) {
                auxView++;
            }
        }
        numShooters = auxShooters;
        numInView = auxView;

        botWaits.OrderBy(bw => Vector3.Distance(lendaLib.playerPos, bw.transform.position));
        botShooters.OrderBy(bt => bt.state == enumBotState.MirarTiro).ThenBy(bt => bt.amVisible).ThenBy(bt => bt.distToPlayer);
    }

    void encherSacoJogador() {
        if (botWaits.Count == 0) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) || numShooters > 0) {
            timerThres = 0;
        } else {
            timerThres += Time.deltaTime;
        }

        if (timerThres >= timeThreshold) {
            botWaits[0].acordar("foi invocado pelo ProSystem");

            if (botWaits.Count > 0) {
                botWaits[0].acordar("foi invocado pelo ProSystem");
            }

            timerThres = 0;
        }
    }

    public void addWaiter(botWait waiter) {
        if (botWaits.Contains(waiter)) {
            return;
        }

        botWaits.Add(waiter);
    }

    public void addBot(botInterface bot) {
        if (botShooters.Contains(bot)) {
            return;
        }

        botShooters.Add(bot);
        contarAtiradores();
    }

    public void removeWaiter(botWait waiter) {
        botWaits.Remove(waiter);
    }

    public void removeBot(botInterface bot) {
        botShooters.Remove(bot);    //se bot for holograma, nao vai remover nada
    }

    public int GetBotIndex(botInterface botInterf) {

        for (int i = 0; i < botShooters.Count; i++) {
            if (botInterf == botShooters[i]) {
                return i;
            }
        }

        return botShooters.Count + 1;
    }

    //IF BRUTE FORCE DOESN'T WORK, YOU'RE NOT FORCING ENOUGH
    public ProbeSample NearestProbe(Vector3 position, bool visibility) {

        Profiler.BeginSample("nearProbesys");

        float pitagoras = lendaLib.pitagoras(espacamento / 2, up.magnitude);

        ProbeSample simp = samples[0];
        float dist = Vector3.Distance(position, simp.position);
        foreach (ProbeSample samp in samples) {
            if (samp.playerVisible == visibility) {

                simp = samp;
                dist = Vector3.Distance(position, simp.position);

                break;
            }
        }

        foreach (ProbeSample samp in samples) {
            if (samp.playerVisible != visibility) {
                continue;
            }

            float auxDist = Vector3.Distance(samp.position, position);
            if (auxDist <= pitagoras) {
                return samp;
            }

            if (dist > auxDist) {
                dist = auxDist;
                simp = samp;
            }
        }

        Profiler.EndSample();

        return simp;
    }

    public Vector3 NearestProbePosition(Vector3 position, bool visibility) {
        return NearestProbe(position, visibility).position;
    }

    public ProbeSample[] ProbesWithin(Vector3 position, float range) {
        List<ProbeSample> samplesList = new List<ProbeSample>();

        foreach(ProbeSample sample in samples) {
            float dist = Vector3.Distance(sample.position, position);

            if (dist <= range) {
                samplesList.Add(sample);
            }
        }

        samplesList.OrderBy(sp => sp.playerVisible).ThenBy(sp => sp.distToPlayer);
        return samplesList.ToArray();
    }

    public ProbeSample[] Triangulate(Vector3 position) {
        Debug.LogWarning("Nao testei essa funcao, ctz que nao ta toda certa");

        ProbeSample[] samples = ProbesWithin(position, espacamento);

        samples.OrderBy(sp => sp.distToPlayer);

        if (samples.Length < 3) {
            Debug.LogWarning("Triangulate nao deu 3 mesmo :P");
        } else {
            samples = new ProbeSample[] { samples[0], samples[1], samples[2] };
        }

        return samples;
    }

    public float OverallVisibility(ProbeSample[] samples) {

        float visibles = 0;
        foreach(ProbeSample sp in samples) {
            if (sp.playerVisible) {
                visibles++;
            }
        }

        return visibles / samples.Length;
    }

    public float OverallVisibility(ProbeSample[] samples, Vector3 position) {

        float visibility = 0;
        int nums = 0;
        foreach(ProbeSample sp in samples) {

            float dist = Vector3.Distance(position, sp.position);

            if (dist >= espacamento / 2) {
                continue;
            }

            num++;

            if (sp.playerVisible == false) {
                continue;
            }

        }

        if (nums == 0) {
            nums = 1;
        }

        Debug.LogWarning(visibility + " / " + nums + " (Essa funcao nem ta pronta!");
        return visibility / nums;
    }

    void contagem(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        mortos++;
        if (mortos == num) {
            //Debug.Log("STOP THE BLOODY CLOCK !");
            kronos.enabled = false;

            lendaLib.playerTransf.GetComponent<playerLife>().levelWin();

            this.enabled = false;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 0, 0.1f, 0.3f);

        foreach(probeArea cluster in Area) {
            Vector3 box = new Vector3(cluster.box.x, 0, cluster.box.y);
            Gizmos.DrawCube(cluster.center, box);
        }
    }
}