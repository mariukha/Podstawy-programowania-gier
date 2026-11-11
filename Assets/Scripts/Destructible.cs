using UnityEngine;

public class Destructible : MonoBehaviour
{
    public int pieces = 3;
    public float force = 5f;
    public float pieceLifeTime = 4f;

    bool broken;

    public void Break(Vector3 hitPoint)
    {
        if (broken) return;
        broken = true;

        Material material = GetComponent<Renderer>().sharedMaterial;
        Vector3 pieceScale = transform.localScale / pieces;

        for (int x = 0; x < pieces; x++)
        for (int y = 0; y < pieces; y++)
        for (int z = 0; z < pieces; z++)
        {
            Vector3 local = new Vector3((x + 0.5f) / pieces - 0.5f, (y + 0.5f) / pieces - 0.5f, (z + 0.5f) / pieces - 0.5f);
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.transform.position = transform.TransformPoint(local);
            piece.transform.rotation = transform.rotation;
            piece.transform.localScale = pieceScale;
            piece.GetComponent<Renderer>().sharedMaterial = material;
            Rigidbody rb = piece.AddComponent<Rigidbody>();
            rb.AddExplosionForce(force, hitPoint, 2f, 0.3f, ForceMode.Impulse);
            Destroy(piece, pieceLifeTime);
        }

        Destroy(gameObject);
    }
}
