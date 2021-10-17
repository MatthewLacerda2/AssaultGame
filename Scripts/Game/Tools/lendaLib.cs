using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class lendaLib {

    public static ProbeSystem probeSystem;
    public static mainCanvas mainCanvas;

    public static Transform playerTransf {
        get {
            return Camera.main.transform.root;
        }
    }

    public static Vector3 playerPos {
        get{
            return playerTransf.position;
        }
    }
    
    public static Vector3 cameraPos {
        get {
            return Camera.main.transform.position;
        }
    }

    public static Mesh cubeMesh {
        get {
            return CubeMesh();
        }
    }

    // ( ͡° ͜ʖ ͡°)
    // ¯\_(ツ)_/¯
    // ▄︻̷̿┻̿═━一
    // ʕ•ᴥ•ʔ
    // ( ▀̿ Ĺ̯ ▀ )
    // ಠ_ಠ

    static Mesh CubeMesh() {
        Vector3[] vertices = {  new Vector3 (0, 0, 0), new Vector3 (1, 0, 0), new Vector3 (1, 1, 0), new Vector3 (0, 1, 0),
                                new Vector3 (0, 1, 1), new Vector3 (1, 1, 1), new Vector3 (1, 0, 1), new Vector3 (0, 0, 1)} ;

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        return createMesh(new Mesh(), "cubeDoLenda", vertices, triangles);
    }

    public static Mesh recalcularMesh(Mesh mesh) {
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        mesh.Optimize();
        return mesh;
    }

    public static Mesh createMesh(Mesh mesh, string name, Vector3[] vertices, int[] triangles) {

        //mesh.Clear();

        mesh.name = name;
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return recalcularMesh(mesh);
    }

    public static Vector3 Medium(Vector3[] vectors) {
        Vector3 point = Vector3.zero;

        foreach(Vector3 vec in vectors) {
            point += vec;
        }

        return point;
    }

    public static Vector3 NearestPointInLine(Vector3 origin, Vector3 direction, Vector3 point) {

        direction.Normalize();

        Vector3 dist = point - origin;
        float dot = Vector3.Dot(dist, direction);

        return origin + direction * dot;
    }

    public static Vector3 Vector3Clamp(Vector3 vetor, float min, float max) {

        float abs = Mathf.Abs(vetor.magnitude);

        if (abs < min) {
            return vetor * (min / abs);
        }else if (abs > max) {
            return vetor * (max / abs);
        } else {
            return vetor;
        }
    }

    public static Vector3 Vector2DTO3D(Vector2 vector) {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector2 Vector3DTO2D(Vector3 vector) {
        return new Vector2(vector.x, vector.y);
    }

    public static void explosion(Vector3 center, float radius, float damage, AnimationCurve curva) {

        Life auxLife;

        foreach(Collider col in Physics.OverlapSphere(center, radius)) {
            auxLife = col.transform.GetComponent<Life>();

            if (auxLife == null) {
                continue;
            }

            Vector3 dir = col.transform.position - center;
            dir.Normalize();

            float dist = Vector3.Distance(col.transform.position, center);
            float auxdamage = damage * curva.Evaluate(dist);

            auxLife.AddDamage(auxdamage, auxdamage / 4.0f, center, dir, col, Dano.danoType.explosao);
        }
    }

    public static float GetLastKeyTime(AnimationCurve curva) {
        if (curva.keys.Length == 0) {
            return 0;
        } else {
            return curva.keys[curva.keys.Length - 1].time;
        }
    }

    public static float Vector3Max(Vector3 v3) {
        float x = Mathf.Abs(v3.x);
        float y = Mathf.Abs(v3.y);
        float z = Mathf.Abs(v3.z);

        if (x >= y && x >= z) {
            return x;
        } else if (y >= x && y >= z) {
            return y;
        } else {
            return z;
        }
    }

    public static float pathLength(Vector3[] path) {
        float dist = 0;
        for (int i = 0; i < path.Length - 1; i++) {
            dist += Vector3.Distance(path[i], path[i + 1]);
        }

        return dist;
    }

    public static float SomaRiemann(AnimationCurve curva, int numSecoes) {

        numSecoes = Mathf.Clamp(numSecoes, 2, 1000);

        float soma = 0;

        for(int i = 0; i < curva.keys.Length - 1; i++) {

            float timeAtual = curva.keys[i].time;

            float secaoLength = curva.keys[i + 1].time - curva.keys[i].time;

            float subSecao = secaoLength / numSecoes;

            for (int j = 1; j < numSecoes + 1; j++) {

                float eval = curva.Evaluate(timeAtual + (j * subSecao));

                soma += (eval * subSecao);

            }
        }

        return soma;
    }

    public static float SomaRiemannConvergida(AnimationCurve curva, float margem, float erro) {

        UnityEngine.Profiling.Profiler.BeginSample("somaRiemannConvergida");

        margem = Mathf.Clamp(margem, 0, 0.2f);
        erro = Mathf.Abs(erro);

        int i = 2;
        float soma = SomaRiemann(curva, i);
        i++;

        for(; i < 10; i++) {

            float auxSoma = SomaRiemann(curva, Mathf.CeilToInt(Mathf.Pow(2,i)));

            float auxMargem = 1 - (auxSoma / soma);
            if (auxSoma > soma) {
                auxMargem = 1 - (soma / auxSoma);
            }

            float auxErro = Mathf.Abs(auxSoma - soma);

            soma = auxSoma;

            if (auxMargem <= margem || (auxErro <= erro)) {
                break;
            }

        }

        Debug.Log("Soma = " + soma + ", Riemann convergiu em " + i + " interacoes pra convergir Riemann");
        UnityEngine.Profiling.Profiler.EndSample();

        return soma;
    
    }

    public static bool isInViewFrustum(Vector3 point, Vector3 direction, float fov, Vector3 target) {
        Vector3 viewDirection = target - point;

        float angle = Vector3.Angle(viewDirection, direction);
        return angle <= fov / 1.5f;
    }

    public static bool isPointInLine(Vector3 origin, Vector3 direction, Vector3 point, float lineLength) {
        return (point == NearestPointInLine(origin, direction, point));
    }

    /*
    //essa função devia servir pra substituir um elemento X num vetor pro um Y, qndo não se sabe o indice de X
    //mas eu ainda nao testei então não sei se funciona :D
    //era pra usar object ou ref?
    public static void subst(object antigo, object novo, params object[] vetor) {
        for(int i = 0; i < vetor.Length; i++) {
            if(vetor[i] == antigo) {
                vetor[i] = novo;
            }
            return;
        }
    }
    */

    public static bool RaycastReflect(Vector3 origin, Vector3 direction, out RaycastHit[] hits, float maxDistance, int maxBounces) {
        maxBounces++;   //Direct ray counts as bounce :p

        List<RaycastHit> hitList = new List<RaycastHit>();

        RaycastHit auxHit;
        bool hasHit = Physics.Raycast(origin, direction, out auxHit, maxDistance);

        if (hasHit == false) {
            hits= hitList.ToArray();
            return false;
        }

        while(hasHit && maxDistance > 0 && maxBounces>0) {
            hitList.Add(auxHit);
            direction = Vector3.Reflect(direction, auxHit.normal);
            maxDistance -= Vector3.Distance(origin, auxHit.point);
            origin = auxHit.point;
            maxBounces--;

            hasHit = Physics.Raycast(origin, direction, out auxHit, maxDistance); //yo what about non-statics?    //nah whadahell
        }

        hits = hitList.ToArray();

        return true;
    }

    public static bool RaycastAll(Vector3 origin, Vector3 direction, out RaycastHit[] hits, float range, Transform exclude) {

        List<RaycastHit> auxHits = Physics.RaycastAll(origin, direction, range).ToList();

        if (auxHits.Count == 0) {
            hits = null;
            return false;
        }

        for(int i = 0; i < auxHits.Count; i++) {
            if (auxHits[i].transform.root == exclude) {
                auxHits.RemoveAt(i);
            }
        }

        hits = auxHits.ToArray();
        hits.OrderBy(ht => ht.distance);
        /*
        string strange = "";
        for(int i = 0; i < hits.Length; i++) {
            strange += hits[i].transform.name + "(" + hits[i].distance + "), ";
        }
        Debug.Log(strange);
        */
        return true;

    }

    public static bool RandomChance(float chance, float total) {
        float x = Random.Range(0, total);

        return x <= chance;
    }

    public static bool RaycastVisibility(Vector3 origem, Transform target, float range) {
        float dist = Vector3.Distance(target.position, origem);
        if (dist > range + 1.0f) {
            return false;
        }

        Vector3 dir = (target.position - origem).normalized;

        if (Physics.Raycast(origem, dir, out RaycastHit hit, range)) {
            return hit.transform.root == target.transform.root;
        } else {
            return true;
        }
    }

    public static bool BoundsContains(Vector3 center, Vector3 box, Vector3 point) {
        box /= 2;

        bool x = Mathf.Abs(point.x - center.x) <= box.x;
        bool y = Mathf.Abs(point.y - center.y) <= box.y;
        bool z = Mathf.Abs(point.z - center.z) <= box.z;

        return x && y && z;
    }

    public static bool ColliderContains(Collider col, Vector3 source) {

        Vector3 closestPoint = col.ClosestPoint(source);    //Vai retornar 'source' se source estiver dentro do collider

        return Vector3.Distance(closestPoint, source) == 0;
    }

    public static GameObject[] GetDirectChildsOf(GameObject go) {
        List<GameObject> gos = new List<GameObject>();

        foreach (Transform child in go.transform) {
            if (child.transform.parent == go.transform) {
                gos.Add(child.gameObject);
            }
        }

        return gos.ToArray();
    }

    public static void Balancear(Rigidbody[] buddies, float mass) {

        float[] volumes = new float[buddies.Length];
        float totalVol = 0;

        int i = 0;
        foreach (Rigidbody buddy in buddies) {

            Collider col = buddy.GetComponent<Collider>();

            if (col==null || col.isTrigger) {
                continue;
            }

            float volume = Volume(col);

            volumes[i] = volume;
            totalVol += volume;

            i++;

        }

        i = 0;
        foreach (Rigidbody buddy in buddies) {
            buddy.mass = (volumes[i] / totalVol) * mass;
            i++;
        }
    }

    public static float Volume(Collider col) {
        
        if (col.GetType() == typeof(BoxCollider)) {
            
            BoxCollider box = col as BoxCollider;

            return box.size.x * box.size.y * box.size.z;

        } else if (col.GetType() == typeof(SphereCollider)) {

            SphereCollider sphere = col as SphereCollider;

            return (4 / 3) * Mathf.PI * (Mathf.Pow(sphere.radius, 3));

        } else if (col.GetType() == typeof(CapsuleCollider)) {

            CapsuleCollider capsule = col as CapsuleCollider;

            float cilinder = Mathf.PI * capsule.radius * capsule.radius * capsule.height;
            float sphere = (4 / 3) * Mathf.PI * (Mathf.Pow(capsule.radius, 3));
            return cilinder + sphere;

        } else {    //Harsh approx

            MeshCollider mesh = col as MeshCollider;

            Vector3 size = mesh.bounds.size;

            return size.x * size.y * size.z * (Mathf.PI / 4);
        }
    }

    public static float pitagoras(float x, float y) {
        x = x * x;
        y = y * y;

        return Mathf.Sqrt(x + y);
    }

    public static float fovSize(float fovAngle, float distance) {
        return 2 * distance * Mathf.Tan(fovAngle / 2);
    }

    public static float AngleBetween(Vector3 forward, Vector3 source, Vector3 target) {
        Debug.LogWarning("take this with a grain of salt");

        Quaternion rotationAngle = Quaternion.LookRotation(target - source);
        return 380 - rotationAngle.eulerAngles.magnitude;
    }

    public static void RotateTowards(Transform transf, Vector3 target) {
        //É uma mistura de mirar() do shooterAI.cs com o do turret.cs
        Debug.LogWarning("funcao nao implementada");
    }
}