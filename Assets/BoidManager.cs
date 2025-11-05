using PersonalHelpers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoidManager : MonoBehaviour 
{

    public UnityEvent< string> myCustomEvent;
    public float PanicIntensity;
    public float SpawnRadius;
    public GameObject BoidPrefab;
    public int InspectorBoidCount;
    public static int BoidCount;
    public delegate void IntAction(int value);
    public static Action SetBoidCount = () => { };


    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            CallBoidCount();
        }
    }

    public void test2()
    {

    }

    public void test(string value = "")
    {

    }

    [ContextMenu("Invoke SetBoidCount")]

    public void CallBoidCount()
    {
        SpawnRadius = Mathf.Min(new float[] {SpawnRadius, WorldBounds.BoundsRect.width / 2f - 5, WorldBounds.BoundsRect.height / 2f - 5});

        BoidCount = InspectorBoidCount;
        BoidBehavior.BoidCount = 0;
        BoidManager.SetBoidCount.Invoke();

        int NewBoidCount = BoidCount - BoidBehavior.BoidCount;

        for (int i = 0; i < NewBoidCount; i++)
        {
            Vector2 pos = UnityEngine.Random.insideUnitCircle * SpawnRadius;
            Instantiate(BoidPrefab, pos + WorldBounds.BoundsRect.center, Quaternion.identity);
        }
    }




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
    [SerializeField] private BoundsHandler WorldBounds;



    private void Update()
    { 
    
        BoidBehavior.RepulsionFallOffPower = RepulsionFallOffPower;

        BoidBehavior.PanicIntensity = PanicIntensity;
        BoidBehavior.WorldBounds = WorldBounds;
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
