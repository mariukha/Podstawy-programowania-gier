using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool sticky;
    public GameObject hitEffect;
    public float lifeTime = 5f;
    public float stickTime = 5f;

    bool hit;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (!hit && timer > lifeTime) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hit) return;
        hit = true;

        ContactPoint contact = collision.GetContact(0);
        if (hitEffect != null)
            Destroy(Instantiate(hitEffect, contact.point, Quaternion.LookRotation(contact.normal)), 2f);

        Target target = collision.collider.GetComponent<Target>();
        if (target != null) target.Hit();

        Destructible destructible = collision.collider.GetComponent<Destructible>();
        if (destructible != null) destructible.Break(contact.point);

        if (sticky)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            transform.position = contact.point;
            Destroy(gameObject, stickTime);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
