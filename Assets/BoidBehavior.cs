using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
public class BoidBehavior : MonoBehaviour
{
    public bool LOG = false;
    public float Speed = 5;
    private RaycastHit2D[] ViewRays;
    private Ray[] RenderRays;

    public int ViewRayCount;
    public float ViewAngle;
    public float TurnRate= 0.01f;
    public float ViewDistance;
    public CircleCollider2D RepulsionCollider;
    public Vector3 FacingDirection;
    public float SeparationStrength = 5f;

    public float CohesionStrength = 5f;
    public float AlignmentStrength = 5f;
    public List<Collider2D> CloseBoids = new List<Collider2D>(10);
    public LayerMask ObjectsToSense;
    public Vector3 NearestBoidPos;
    private bool started = false;
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.TryGetComponent<BoidBehavior>(out BoidBehavior b))
        {
            if (b != this)
            {
                if (LOG)
                {
                    Debug.Log("hit");

                }
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



  
// Update is called once per frame
void FixedUpdate()
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

            turnDir -= ((((halfViewAngle) - Abs(angle)) / halfViewAngle)) * anglesign * (1- fraction);

            turnforce += (1 - fraction)/ ViewRayCount;
        }


        turnAngle = Sign(turnDir) * Clamp01(Abs(turnDir)) * Time.deltaTime * TurnRate * turnforce;




        NearestBoidPos = ClosestBoidPosition();

        float angleToClosestBoid = Vector3.SignedAngle( NearestBoidPos - transform.position , FacingDirection, Vector3.forward);
        float distToClosestBoid = (transform.position - NearestBoidPos).magnitude;

        float repulsionForce = 1 - (distToClosestBoid / (RepulsionCollider.radius * 2));

        repulsionForce = Pow(repulsionForce, 2);

       

        if (CloseBoids.Count > 0)
        {

            if (LOG)
            {
              //  Debug.Log(transform.position);
             //   Debug.Log(NearestBoidPos);


             //   Debug.Log(distToClosestBoid);
              //  Debug.Log(RepulsionCollider.radius);
             //   Debug.Log(repulsionForce);
                Debug.Log(angleToClosestBoid);
            }

            turnAngle += (angleToClosestBoid / 180) * SeparationStrength * repulsionForce;

        }

        CloseBoids.Clear();

        //Cohesion

        float angleToCom = Vector3.SignedAngle(GameController.COM-transform.position, FacingDirection, Vector3.forward);

        if (LOG)
        {
         //   Debug.Log(angleToCom);

        }


        turnAngle -= (angleToCom / 180) * CohesionStrength;


        //Alignement

        float angleToAverageDir = Vector3.SignedAngle(GameController.AverageDir, FacingDirection, Vector3.forward);

        turnAngle -= (angleToAverageDir / 180) * AlignmentStrength;





        transform.Rotate(new Vector3(0, 0, turnAngle));

    }

    private void OnDrawGizmos()
    {
        if (started)
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
