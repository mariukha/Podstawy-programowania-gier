using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public string displayName = "Kostka";
    public string message = "Podniesiono kostkę!";
    [Min(0.1f)] public float maxFollowSpeed = 5f;
    public float maxHoldDistance = 3f;

    public static readonly List<Interactable> All = new List<Interactable>();

    Rigidbody rb;
    Collider[] colliders;
    Transform holdPoint;
    Collider holder;
    Vector3 pickupOffset;
    Quaternion pickupRotation;
    float minimumReach;

    public bool IsHeld => holdPoint != null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        colliders = GetComponentsInChildren<Collider>();
    }

    void OnEnable()
    {
        All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    public void Grab(Transform point, Collider player)
    {
        holdPoint = point;
        holder = player;
        pickupRotation = Quaternion.Inverse(point.rotation) * rb.rotation;
        minimumReach = player.bounds.extents.x + colliders[0].bounds.extents.z;
        pickupOffset = Quaternion.Inverse(point.rotation) * (rb.position - GripPosition());
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        foreach (Collider c in colliders) Physics.IgnoreCollision(c, player, true);
    }

    public void Release()
    {
        if (holder != null)
            foreach (Collider c in colliders) Physics.IgnoreCollision(c, holder, false);
        holdPoint = null;
        holder = null;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public bool TooFar()
    {
        return IsHeld && Vector3.Distance(rb.position, holdPoint.position) > maxHoldDistance;
    }

    Vector3 GripPosition()
    {
        Vector3 forward = holder.transform.forward;
        float reach = Vector3.Dot(holdPoint.position - holder.bounds.center, forward);
        return holdPoint.position + forward * Mathf.Max(0f, minimumReach - reach);
    }

    public void FollowHands()
    {

        pickupOffset = Vector3.MoveTowards(pickupOffset, Vector3.zero, maxFollowSpeed * Time.deltaTime);
        pickupRotation = Quaternion.RotateTowards(pickupRotation, Quaternion.identity, 360f * Time.deltaTime);
        Vector3 target = GripPosition() + holdPoint.rotation * pickupOffset;
        Vector3 step = target - rb.position;
        float distance = step.magnitude;
        if (distance > 0f && rb.SweepTest(step / distance, out RaycastHit hit, distance, QueryTriggerInteraction.Ignore)
            && hit.collider != holder)
        {
            target = rb.position + step.normalized * Mathf.Max(0f, hit.distance - Physics.defaultContactOffset);
        }
        rb.position = target;
        rb.rotation = holdPoint.rotation * pickupRotation;
    }
}
