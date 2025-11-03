using System.Collections.Generic;
using UnityEngine;
using PersonalHelpers;
using static UnityEngine.Mathf;
public class BoidBehavior : MonoBehaviour
{
   public static BoundsHandler WorldBounds;





    [SerializeField] private float AngleDelta = 0;
    public static float MaxAngleDelta = 5;
    public static float MaxAngleChangeRate = 0.25f;

    [Min(0.01f)] public static float RepulsionFallOffPower = 2;
    public static bool DoLocalBehavior = true;
    [SerializeField] private SpecLog Logger = new();
    public static float Speed = 5;
    private RaycastHit2D[] ViewRays;
    private Ray[] RenderRays;
    public static int ViewRayCount;
    public static float ViewAngle;
    public static float TurnRate= 0.01f;
    public static float ViewDistance;
    [SerializeField] private CircleCollider2D RepulsionCollider;
    [SerializeField] private Vector3 FacingDirection;
    public static float SeparationStrength = 5f;
    public static float CohesionStrength = 5f;
    public static float AlignmentStrength = 5f;
    [SerializeField] private List<Collider2D> CloseBoids = new List<Collider2D>(10);
    [SerializeField] private LayerMask ObjectsToSense;
    [SerializeField] private Vector3 NearestBoidPos;
    private bool started = false;
    private bool CollisionDataUsed = false;
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (CollisionDataUsed)
        {
            CloseBoids.Clear();
            CollisionDataUsed = false;
        }




        if (collision.TryGetComponent<BoidBehavior>(out BoidBehavior b))
        {
            if (b != this)
            {



                CloseBoids.Add(collision);
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        started = true; 
        RepulsionCollider = GetComponent<CircleCollider2D>();
    }

    Vector3 ClosestBoidPosition()
    {

        float mindist = float.MaxValue;
        float dist = 0;
        Vector3 output = Vector3.zero;
        foreach (Collider2D c in CloseBoids)
        {

            dist = (c.transform.position - transform.position).magnitude;

            if (dist < mindist)
            {
                mindist = dist;

                output = c.transform.position;
            }
        }

        return output;

    }


    void GlobalBehavior()
    {
        FacingDirection = transform.TransformDirection(Vector3.up);
        transform.position = transform.position + FacingDirection * (Speed * Time.deltaTime);
        ViewRays = new RaycastHit2D[ViewRayCount];
        RenderRays = new Ray[ViewRayCount];

        float turnAngle = 0;
        //seperation

        float turnDir = 0;
        float turnforce = 0;

        //Wall avoidance

        for (int i = 0; i < ViewRayCount; i++)
        {
            float halfViewAngle = ViewAngle / 2f;

            float angle = Lerp(0, ViewAngle, i / (float)ViewRayCount) - halfViewAngle;
            float anglesign = (Sign(angle));
            //   Debug.Log(angle);

            Vector3 dir = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * FacingDirection;

            //RenderRays[i] = new Ray(transform.position + RepulsionCollider.radius * dir, dir);
            RenderRays[i] = new Ray(transform.position + 1f * dir, dir);
            ViewRays[i] = Physics2D.Raycast(RenderRays[i].origin, RenderRays[i].direction, ViewDistance, ObjectsToSense);

            float fraction = ViewRays[i].fraction;
            fraction = (fraction == 0) ? 1 : fraction;

            turnDir -= ((((halfViewAngle) - Abs(angle)) / halfViewAngle)) * anglesign * (1 - fraction);

            turnforce += (1 - fraction) / ViewRayCount;
        }

        turnAngle = Sign(turnDir) * Clamp01(Abs(turnDir)) * Time.deltaTime * TurnRate * turnforce;

        NearestBoidPos = ClosestBoidPosition();

        float angleToClosestBoid = Vector3.SignedAngle(NearestBoidPos - transform.position, FacingDirection, Vector3.forward);
        float distToClosestBoid = (transform.position - NearestBoidPos).magnitude;

        float repulsionForce = 1 - (distToClosestBoid / (RepulsionCollider.radius * 2));

        repulsionForce = Pow(repulsionForce, 2);

        if (CloseBoids.Count > 0)
        {

            Logger.Log(angleToClosestBoid);
            

            turnAngle += (angleToClosestBoid / 180) * SeparationStrength * repulsionForce;

        }

        CloseBoids.Clear();

        //Cohesion

        float angleToCom = Vector3.SignedAngle(GameController.COM - transform.position, FacingDirection, Vector3.forward);

        turnAngle -= (angleToCom / 180) * CohesionStrength;

        //Alignement

        float angleToAverageDir = Vector3.SignedAngle(GameController.AverageDir, FacingDirection, Vector3.forward);

        turnAngle -= (angleToAverageDir / 180) * AlignmentStrength;

        transform.Rotate(new Vector3(0, 0, turnAngle));

    }





    void Localbehavior()
    {

        
        //init values


        CollisionDataUsed = true;
        FacingDirection = transform.TransformDirection(Vector3.up);
        transform.position = transform.position + FacingDirection * (Speed * Time.deltaTime);
        ViewRays = new RaycastHit2D[ViewRayCount];
        RenderRays = new Ray[ViewRayCount];

        float turnAngle = 0;
        float turnDir = 0;
        float turnforce = 0;


        Vector3 localCOM = Vector3.zero;
        Vector3 localAverageDirection = Vector3.zero;
        float cumulativeRepulsionWeight = 0;

        for (int i = 0; i < CloseBoids.Count; i++)
        {
            localCOM += CloseBoids[i].transform.position;
            localAverageDirection += CloseBoids[i].transform.TransformDirection(Vector3.up);


            // normalizes a weight for the steering force to be applied where 1 is as close as possible and 0 is right at the edge of the detection range

            float repulsionWeight = 1 - ((transform.position - CloseBoids[i].transform.position).magnitude)/RepulsionCollider.radius;

            repulsionWeight = Pow(repulsionWeight, RepulsionFallOffPower);


            // applies the sign of the angle to the offending boid to indicate which direction this repulsion should be applied

            repulsionWeight *= -Sign(Vector3.SignedAngle(CloseBoids[i].transform.position - transform.position, FacingDirection, Vector3.forward));

            // adds the repulsion to the total wich will be combined to form an overall direction

            cumulativeRepulsionWeight += repulsionWeight;
        }

        localAverageDirection.Normalize();
        localCOM /= CloseBoids.Count;


      //  cumulativeRepulsionWeight /= CloseBoids.Count;

        for (int i = 0; i < ViewRayCount; i++)
        {
            float halfViewAngle = ViewAngle / 2f;

            float angle = Lerp(0, ViewAngle, i / (float)ViewRayCount) - halfViewAngle;
            float anglesign = (Sign(angle));
            //   Debug.Log(angle);

            Vector3 dir = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * FacingDirection;

            //RenderRays[i] = new Ray(transform.position + RepulsionCollider.radius * dir, dir);
            RenderRays[i] = new Ray(transform.position + 1f * dir, dir);
            ViewRays[i] = Physics2D.Raycast(RenderRays[i].origin, RenderRays[i].direction, ViewDistance, ObjectsToSense);

            float fraction = ViewRays[i].fraction;
            fraction = (fraction == 0) ? 1 : fraction;

            turnDir -= ((((halfViewAngle) - Abs(angle)) / halfViewAngle)) * anglesign * (1 - fraction);

            turnforce += (1 - fraction) / ViewRayCount;
        }

        //Separation


        if (CloseBoids.Count != 0)
        {
            turnAngle -= (cumulativeRepulsionWeight) * SeparationStrength;
        }


        //Wall avoidance
        // += for consistency
        turnAngle += Sign(turnDir) * Clamp01(Abs(turnDir)) * Time.deltaTime * TurnRate * turnforce;

        Logger.Log(turnAngle);

        //Cohesion

        float angleToCom = Vector3.SignedAngle(localCOM - transform.position, FacingDirection, Vector3.forward);


        if (CloseBoids.Count != 0)
        {
            turnAngle -= (angleToCom / 180) * CohesionStrength;
        }

        Logger.Log(turnAngle);

        //Alignement

        float angleToAverageDir = Vector3.SignedAngle(localAverageDirection, FacingDirection, Vector3.forward);

        turnAngle -= (angleToAverageDir / 180) * AlignmentStrength;

        Logger.Log(turnAngle);

        transform.Rotate(new Vector3(0, 0, turnAngle + AngleDelta));

        










    }





    // Update is called once per frame
    void FixedUpdate()
    {
        if (WorldBounds != null && WorldBounds.BoundsRect.Contains(transform.position))
        {

        }
        else
        {
            transform.position = Vector3.zero;
        }





            AngleDelta += Random.Range(-MaxAngleChangeRate, MaxAngleChangeRate);

        //float potentialoverflow = AngleDelta - (MaxAngleDelta * Sign(AngleDelta));

      //  if (Abs(potentialoverflow) > 0)
        {
       //     AngleDelta -= potentialoverflow * Sign(AngleDelta);
            AngleDelta = Clamp(AngleDelta,-MaxAngleDelta, MaxAngleDelta);
        }






        if (DoLocalBehavior)
        {
            Localbehavior();

        }
        else
        {
            GlobalBehavior();
        }
    }

    private void OnDrawGizmos()
    {
        if (started && ViewRays != null)
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < ViewRayCount; i++)
            {

                float distance = ViewRays[i].distance;
                distance = (distance == 0) ? ViewDistance : distance;

                Gizmos.DrawRay(RenderRays[i].origin, RenderRays[i].direction * distance);

                Gizmos.DrawSphere(ViewRays[i].point, 0.1f);

            }
            Gizmos.color = Color.purple;

            Gizmos.DrawLine(transform.position, NearestBoidPos);
        }





    }
}
