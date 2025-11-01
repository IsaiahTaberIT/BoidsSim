using PersonalHelpers;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour 
{
    [Min(0.01f)][SerializeField] private float RepulsionFallOffPower = 2;
    [SerializeField] private bool DoLocalBehavior = true;
    [SerializeField] private float Speed = 5;
    [SerializeField] private int ViewRayCount;
    [SerializeField] private float ViewAngle;
    [SerializeField] private float TurnRate = 0.01f;
    [SerializeField] private float ViewDistance;
    [SerializeField] private float SeparationStrength = 5f;
    [SerializeField] private float CohesionStrength = 5f;
    [SerializeField] private float AlignmentStrength = 5f;


    [SerializeField] private float MaxAngleDelta = 5;
    [SerializeField] private float MaxAngleChangeRate = 0.25f;



    private void Update()
    {
        BoidBehavior.MaxAngleDelta = MaxAngleDelta;
        BoidBehavior.MaxAngleChangeRate = MaxAngleChangeRate;
        BoidBehavior.Speed = Speed;
        BoidBehavior.ViewRayCount = ViewRayCount;
        BoidBehavior.ViewAngle = ViewAngle;
        BoidBehavior.TurnRate = TurnRate;
        BoidBehavior.ViewDistance = ViewDistance;
        BoidBehavior.DoLocalBehavior = DoLocalBehavior;
        BoidBehavior.SeparationStrength = SeparationStrength;
        BoidBehavior.CohesionStrength = CohesionStrength;
        BoidBehavior.AlignmentStrength = AlignmentStrength;
    }
}
