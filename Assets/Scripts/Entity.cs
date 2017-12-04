using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour {

    protected SteeringObject steerObject;

    protected Vector3 targetPosition;
	
    void Awake()
    {
        steerObject = GetComponent<SteeringObject>();

        GameManager.Instance.OnPlayerMark += SetTargetPosition;
    }

    void SetTargetPosition(Vector3 target)
    {
        targetPosition = target.RemoveY();
    }

    void Update()
    {
        if(targetPosition != null) FleeTarget(this);
    }

    public static void SeekTarget(Entity e)
    {
        e.steerObject.ResetForces();
        e.steerObject.AddForce(e.steerObject.Seek(e.targetPosition));
        e.steerObject.ApplyForces();
    }

    public static void FleeTarget(Entity e)
    {
        e.steerObject.ResetForces();
        e.steerObject.AddForce(e.steerObject.Flee(e.targetPosition));
        e.steerObject.ApplyForces();
    }
}
