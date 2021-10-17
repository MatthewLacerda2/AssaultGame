using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(shooterAI))]
public class hologramAI : MonoBehaviour {

    public Texture holoText;

    public Vector2 speeds = new Vector2(0.1f, 0);

    public float deathFadeTime = 2.1f;
    [Range(0f, 1f)] public float origin = 0.9f;
    [Range(0f, 1f)] public float deathAnimeSpeed = 0.2f;

    public float maxDist = 22f;

    Light myLight;
    shooterAI chuta;
    SkinnedMeshRenderer skRender;
    MeshRenderer armaRender;

    float offSetX, offSetY;

    void Awake() {
        transform.SetParent(null);

        chuta = GetComponent<shooterAI>();
        skRender = GetComponentInChildren<SkinnedMeshRenderer>();

        Life lie = GetComponent<Life>();
        lie.vida = 0.001f;
        lie.delegados += Morte;

        armaRender = chuta.arma.GetComponentInChildren<MeshRenderer>();

        chuta.dano = 0;
        chuta.spread2D *= 3;
        chuta.firerate *= 2;
        chuta.woodDecal = new GameObject("holoDecal");
        chuta.stoneDecal = new GameObject("holoDecal");
        chuta.metalDecal = new GameObject("holoDecal");
        chuta.capsulaDeBala = new GameObject("holoCapsule");

        foreach (Material mat in skRender.materials) {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            Color collor = mat.color;
            collor.a = origin;
            mat.color = collor;
        }

        foreach (Material mat in skRender.materials) {
            mat.SetTexture("_DetailAlbedoMap", holoText);
        }
        foreach (Material mat in armaRender.materials) {
            mat.SetTexture("_DetailAlbedoMap", holoText);
        }

        foreach (Collider col in GetComponentsInChildren<Collider>()) {
            col.isTrigger = true;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.speed = agent.speed * 0.8f;
        agent.acceleration = 4.0f;
        agent.radius = agent.radius * 0.66f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.avoidancePriority = 99;
        if (agent.stoppingDistance > 4.0f) {
            agent.stoppingDistance = 4.0f;
        }

        GameObject go = Instantiate(new GameObject("holoLight"), transform.position + ProbeSystem.up / 2, Quaternion.identity, transform);
        myLight = go.AddComponent<Light>();
        myLight.type = LightType.Point;
        myLight.lightShadowCasterMode = LightShadowCasterMode.Default;
        myLight.intensity = 1.25f;
        myLight.range = 3.0f;
        myLight.color = new Color(0.0f, 0.8f, 1, 1);
        myLight.cullingMask = 279;
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(lendaLib.playerPos, transform.position) > maxDist) {
            Morte(new Dano(1000,0,transform.position,Vector3.zero,null,Dano.danoType.logico));
            return;
        }

        offSetX += (speeds.x * Time.deltaTime);
        offSetY += (speeds.y * Time.deltaTime);

        foreach (Material mat in skRender.materials) {
            mat.SetTextureOffset("_DetailAlbedoMap", new Vector2(offSetX, offSetY));
        }
        foreach(Material mat in armaRender.materials) {
            mat.SetTextureOffset("_DetailAlbedoMap", new Vector2(offSetX, offSetY));
        }
    }

    void Morte(Dano damage) {
        if (damage.killed == false) {
            return;
        }

        this.enabled = false;

        StartCoroutine(fadeRoutine());

        GetComponent<Animator>().speed = deathAnimeSpeed;

        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
            rb.useGravity = false;
        }

        GetComponent<ParticleSystem>().Play();

        Destroy(gameObject, deathFadeTime);

        //skRender.enabled = false;
    }

    System.Collections.IEnumerator fadeRoutine() {
        float scale = 0.9f;
        float x = deathFadeTime * scale;

        float init = myLight.intensity;


        while (x > 0) {
            foreach(Material mat in skRender.materials) {
                Color collor = mat.color;
                collor.a = x / (deathFadeTime * scale);
                mat.color = collor;
            }

            myLight.intensity = init * (x/(deathFadeTime*scale));

            x -= Time.deltaTime;

            yield return null;
        }
    }
}