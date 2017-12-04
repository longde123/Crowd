using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SteeringObject : MonoBehaviour
{
    public MovementVars movement;

    Vector3 _velocity;
    Vector3 _steerForce;

    Vector3 _wander;
    float _nextWander;

    public Action LookAt;

    public Vector3 position { get { return transform.position; } }
    public Vector3 velocity { get { return _velocity; } set { _velocity = value; } }
    public float mass { get { return 1f; } }
    public float maxVelocity { get { return movement.velocityLimit; } }


    virtual protected void Start()
    {
        LookAt = LookAtVelocity;
    }

    public Vector3 Seek(Vector3 targetPosition)
    {
        var deltaPos = targetPosition - transform.position;
        var desiredVel = deltaPos.normalized * maxVelocity;
        return desiredVel - _velocity;
    }

    public Vector3 Flee(Vector3 targetPosition)
    {
        var deltaPos = targetPosition - transform.position;
        var desiredVel = -deltaPos.normalized * maxVelocity;        //La velocidad deseada es la OPUESTA a seek
        return desiredVel - _velocity;
    }

    public Vector3 Arrival(Vector3 targetPosition, float arrivalRadius, float minRadius = 3)
    {
        var deltaPos = targetPosition - transform.position;
        //var desiredVel = deltaPos.normalized * maxSpeed;		//¡No normalizamos!
        var distance = deltaPos.magnitude;
        Vector3 desiredVel;
        if (distance < minRadius)
        {
            if (_velocity.sqrMagnitude > .8f) _velocity *= 0.80f;
            return Vector3.zero;
        }
        else if (distance < arrivalRadius)
        {
            desiredVel = deltaPos * maxVelocity / arrivalRadius;
        }
        else
        {
            desiredVel = deltaPos / distance * maxVelocity;
        }
        return desiredVel - _velocity;
    }

    public Vector3 ArrivalOptimized(Vector3 targetPosition, float arrivalRadius)
    {
        var deltaPos = targetPosition - transform.position;
        var desiredVel = Utility.Truncate(deltaPos * maxVelocity / arrivalRadius, maxVelocity);
        return desiredVel - _velocity;
    }

    public Vector3 WanderRandomPos()
    {
        movement.wanderDistanceAhead = 0f;   //HACK: Seteamos a 0 para que los gizmos de wander no muestren cualquier cosa

        if (Time.time > _nextWander)
        {
            _wander = Vector3.zero;
            _nextWander = Time.time + movement.wanderPeriod;
            _wander = Utility.RandomDirection() * movement.wanderRandomStrength;
        }
        return Seek(_wander);
    }

    public Vector3 NewWanderRandom(Vector3 pivote)
    {
        if (Time.time > _nextWander)
        {
            //Vector3 steer = pivote + Utility.RandomDirection() * movement.wanderRandomRadius;
            _nextWander = Time.time + movement.wanderPeriod;
        }
        return Seek(_wander);
    }

    public Vector3 WanderTwitchy()
    {
        var desiredVel = Utility.RandomDirection() * maxVelocity;
        return desiredVel - _velocity;
    }

    public Vector3 WanderWithState(float distanceAhead, float randomRadius, float randomStrength)
    {
        _wander = Utility.Truncate(_wander + Utility.RandomDirection() * randomStrength, randomRadius);
        var aheadPosition = transform.position + _velocity.normalized * distanceAhead + _wander;
        return Seek(aheadPosition);
    }

    public Vector3 WanderWithStateTimed(float distanceAhead, float randomRadius, float randomStrength)
    {
        if (Time.time > _nextWander)
        {
            _nextWander = Time.time + movement.wanderPeriod;
            _wander = Utility.Truncate(_wander + Utility.RandomDirection() * randomStrength, randomRadius);
        }
        var aheadPosition = transform.position + _velocity.normalized * distanceAhead + _wander;
        return Seek(aheadPosition);
    }

    //Pursuit: Seek a proyección futura
    public Vector3 Pursuit(ISteerable who, float periodAhead)
    {
        var deltaPos = who.position - transform.position;
        var targetPosition = who.position + who.velocity * deltaPos.magnitude / who.maxVelocity;
        return Seek(targetPosition);
    }

    //Evade: Flee a proyección futura
    public Vector3 Evade(ISteerable who, float periodAhead)
    {
        var deltaPos = who.position - transform.position;
        var targetPosition = who.position + who.velocity * deltaPos.magnitude / who.maxVelocity;
        return Flee(targetPosition);
    }

    public void ResetForces()
    {
        _steerForce = Vector3.zero;
    }

    public void AddForce(Vector3 force)
    {
        _steerForce += force;
    }

    public void ApplyForces()
    {
        var dt = Time.fixedDeltaTime;
        _steerForce.y = 0f;
        _steerForce = Utility.Truncate(_steerForce, movement.forceLimit);
        _velocity = Utility.Truncate(_velocity + _steerForce * dt, maxVelocity);
        transform.position += _velocity * dt;

        // @TODO: Movement

        LookAt();
    }

    public void LookAtVelocity()
    {
        transform.forward = Vector3.Slerp(transform.forward, _velocity, 0.1f);
    }

    /*
    Vector3 _dstPoint;
    public void LookAtTargetPos()
    {
        Vector3 deltaPos =_dstPoint - transform.position;
        deltaPos.y = transform.forward.y;
        transform.forward = Vector3.Slerp(transform.forward, deltaPos, 0.1f);
    }
   */

    virtual protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _velocity);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + _velocity, transform.position + _velocity + _steerForce);
    }

}

[System.Serializable]
public class MovementVars
{
    public float velocityLimit = 5f;
    public float forceLimit = 10f;

    public float arrivalRadius = 5f;
    public float pursuitPeriod = 2f;

    public float wanderDistanceAhead = 5f;
    public float wanderRandomRadius = 5f;
    public float wanderPeriod;
    public float wanderRandomStrength = 5f;
}

public interface ISteerable
{
    Vector3 position { get; }
    Vector3 velocity { get; }
    float mass { get; }
    float maxVelocity { get; }
}
