using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;

public class SteeringBehavior : MonoBehaviour
{
    public Vector3 target;
    public KinematicBehavior kinematic;
    public List<Vector3> path;
    // you can use this label to show debug information,
    // like the distance to the (next) target
    public TextMeshProUGUI label;
    public float slowRadius = 15f;
    public float stopDistance = 0.5f;

    public float turnRate = 5f;

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

        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        float desiredSpeed;
        if (distance < slowRadius)
        {
            desiredSpeed = distance / kinematic.max_speed;
        } else {
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
        EventBus.ShowTarget(target);
    }

    public void SetPath(List<Vector3> path)
    {   
        this.path = path;
    }

    public void SetMap(List<Wall> outline)
    {
        this.path = null;
        this.target = transform.position;
    }
}
