using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GameController : MonoBehaviour
{
    public static List<BoidBehavior> Boids = new List<BoidBehavior>(100);
    public List<BoidBehavior> InspectBoids = new List<BoidBehavior>(100);
    public static Vector3 COM;
    public static Vector3 AverageDir;
    public Vector3 InspectAverageDir;
    public Vector3 InspectCOM;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // pretty sure this is a big for loop for this so do it sparingly
        Boids = FindObjectsByType<BoidBehavior>(FindObjectsSortMode.None).ToList<BoidBehavior>();
        InspectBoids = Boids;
    }

    // Update is called once per frame
    void Update()
    {
        COM = Vector3.zero;
        AverageDir = Vector3.zero;

        Boids = FindObjectsByType<BoidBehavior>(FindObjectsSortMode.None).ToList<BoidBehavior>();


        foreach (BoidBehavior boid in Boids)
        {
            COM += boid.transform.position;
            AverageDir += boid.transform.TransformDirection(Vector3.up);
        }

        AverageDir.Normalize();
        COM /= Boids.Count;
        InspectAverageDir = AverageDir;
        InspectCOM = COM;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, AverageDir * 10);
        Gizmos.DrawSphere(COM, 1f);
    }


}
