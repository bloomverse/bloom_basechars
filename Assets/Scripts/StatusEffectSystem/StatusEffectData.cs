using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Status Effect")]

public class StatusEffectData : ScriptableObject {
    public string Name;
    public int DOTAmount;
    public float MovementPenalty;
    public float Lifetime;
    public float TickSpeed;
    public float moveSpeed;
    public RaycastHit hitpoint;

    public GameObject EffectParticles;



}
