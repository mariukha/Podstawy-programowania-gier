using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public string displayName = "Kostka";
    public string message = "Podniesiono kostkę!";
    public float followSpeed = 25f;
    public float maxHoldDistance = 3f;

    public static readonly List<Interactable> All = new List<Interactable>();

    Rigidbody rb;
    Collider[] colliders;
    Transform holdPoint;
    Collider holder;

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
        rb.useGravity = false;
        foreach (Collider c in colliders) Physics.IgnoreCollision(c, player, true);
    }

    public void Release()
    {
        if (holder != null)
            foreach (Collider c in colliders) Physics.IgnoreCollision(c, holder, false);
        holdPoint = null;
        holder = null;
        rb.useGravity = true;
    }

    public bool TooFar()
    {
        return IsHeld && Vector3.Distance(rb.position, holdPoint.position) > maxHoldDistance;
    }

    void FixedUpdate()
    {
        if (!IsHeld) return;
        rb.linearVelocity = (holdPoint.position - rb.position) * followSpeed;
        rb.angularVelocity = Vector3.zero;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, holdPoint.rotation, 10f * Time.fixedDeltaTime));
    }
}
