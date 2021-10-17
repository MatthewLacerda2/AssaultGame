using UnityEngine;

[RequireComponent(typeof(Animator))]
public class botInterface : MonoBehaviour {

    public enumBotState state = enumBotState.ShortestSight;

    public float distToPlayer, timeNaVista;

    public bool amVisible, canShoot, isPlayerVisible, playerWithinRange;

    [Space] public int numShooters;
    public bool maxShootersFilled, maxViewFilled;
    
    Animator anime;

    void Awake() {
        anime = GetComponent<Animator>();
    }

    public void Refresh(bool isVisible, bool canShoo, bool PlayerVisible, bool playerInRange, float timeVista) {

        amVisible = isVisible;
        canShoot = canShoo;
        isPlayerVisible = PlayerVisible;
        playerWithinRange = playerInRange;

        anime.SetBool("amVisible", isVisible);
        anime.SetBool("canShoot", canShoo);
        anime.SetBool("isPlayerVisible", PlayerVisible);
        anime.SetBool("playerWithinRange", playerInRange);
        anime.SetFloat("timeNaVista", timeVista);
        
        distToPlayer = Vector3.Distance(lendaLib.playerPos, transform.position);
        numShooters = lendaLib.probeSystem.numShooters;
        maxShootersFilled = lendaLib.probeSystem.numShooters >= lendaLib.probeSystem.maxShooters;
        maxViewFilled = lendaLib.probeSystem.numInView >= lendaLib.probeSystem.maxOnScreen;

        anime.SetFloat("distToPlayer", distToPlayer);
        anime.SetInteger("numShooters", numShooters);
        anime.SetBool("maxShootersFilled", maxShootersFilled);
        anime.SetBool("maxViewFilled", maxViewFilled);
    }
}