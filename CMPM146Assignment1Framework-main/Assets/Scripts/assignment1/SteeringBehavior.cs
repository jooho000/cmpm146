using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SteeringBehavior : MonoBehaviour
{
    public Vector3 target;
    public KinematicBehavior kinematic;
    public List<Vector3> path;
    // you can use this label to show debug information,
    // like the distance to the (next) target
    public TextMeshProUGUI label;
    public float slowRadius = 15f;
    public float nextPathPointRadius = 15f;
    public float stopDistance = 0.5f;

    public float turnRate = 5f;

    public bool hasArrived = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        kinematic = GetComponent<KinematicBehavior>();
        target = transform.position;
        path = null;
        EventBus.OnSetMap += SetMap;
    }

    // Update is called once per frame
    void Update()
    {
        // Assignment 1: If a single target was set, move to that target
        //                If a path was set, follow that path ("tightly")

        // you can use kinematic.SetDesiredSpeed(...) and kinematic.SetDesiredRotationalVelocity(...)
        //    to "request" acceleration/decceleration to a target speed/rotational velocity

        Vector3 nextTarget;
        float nextSlowRadius;
        float nextStopRadius;
        float nextEndSpeed;

        if (path != null && path.Count > 1)
        {
            float turnAngle = Vector3.Angle(transform.forward, path[1] - path[0]);

            nextTarget = path[0];
            nextSlowRadius = slowRadius * turnAngle / 180f;
            nextStopRadius = stopDistance + (nextPathPointRadius - stopDistance) * turnAngle / 180f;
            nextEndSpeed = kinematic.max_speed * (1f - turnAngle / 180f);
        }
        else
        {
            if (path != null) nextTarget = path[0];
            else nextTarget = target;
            nextSlowRadius = slowRadius;
            nextStopRadius = stopDistance;
            nextEndSpeed = 0f;
        }


        Vector3 direction = nextTarget - transform.position;
        float distance = direction.magnitude;

        if (distance < nextStopRadius)
        {
            if (path != null)
            {
                path.RemoveAt(0);
                if (path.Count == 0)
                {
                    path = null;
                    hasArrived = true;
                }
            }
            else
            {
                hasArrived = true;
            }
        }

        if (hasArrived)
        {
            kinematic.SetDesiredSpeed(0f);
            kinematic.SetDesiredRotationalVelocity(0f);
            return;
        }

        float desiredSpeed;
        if (distance < nextSlowRadius)
        {   
            desiredSpeed = distance / nextSlowRadius * (kinematic.max_speed - nextEndSpeed) + nextEndSpeed;
        }
        else
        {
            desiredSpeed = kinematic.max_speed;
        }

        kinematic.SetDesiredSpeed(desiredSpeed);

        float angleBetween = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        float desiredAngularVelocity = angleBetween * turnRate;
        kinematic.SetDesiredRotationalVelocity(desiredAngularVelocity);
    }


    public void SetTarget(Vector3 target)
    {
        this.target = target;
        path = null;
        EventBus.ShowTarget(target);
        hasArrived = false;
    }

    public void SetPath(List<Vector3> path)
    {   
        this.path = path;
        hasArrived = false;
    }

    public void SetMap(List<Wall> outline)
    {
        path = null;
        hasArrived = false;
        target = transform.position;
    }
}
