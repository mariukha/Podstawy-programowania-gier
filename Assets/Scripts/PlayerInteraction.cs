using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam;
    public float interactDistance = 2.5f;
    public float maxAngle = 20f;
    public float messageTime = 2f;
    public float grabDelay = 0.9f;
    public float pickUpTime = 2.3f;
    
    PlayerMovement player;
    Collider playerCollider;
    Transform holdPoint;
    Interactable looked;
    Interactable held;
    Interactable pending;
    Animator animator;
    float grabTimer;
    float lockTimer;
    int carryLayer;
    Transform leftHand;
    Transform rightHand;

    public bool IsBusy => held != null || pending != null;
    string message;
    float messageTimer;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
        carryLayer = animator.GetLayerIndex("Carry");
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        holdPoint = new GameObject("HoldPoint").transform;
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;

        looked = held == null && pending == null && !player.topDownMode ? FindTarget() : null;

        if (looked != null && kb.eKey.wasPressedThisFrame)
        {
            pending = looked;
            grabTimer = grabDelay;
            lockTimer = pickUpTime;
            animator.SetTrigger("PickUp");
            message = pending.message;
            messageTimer = messageTime;
            Debug.Log("Interakcja z obiektem: " + pending.displayName);
        }

        if (pending != null)
        {
            if (!kb.eKey.isPressed || player.topDownMode)
            {
                pending = null;
            }
            else
            {
                grabTimer -= Time.deltaTime;
                if (grabTimer <= 0f)
                {
                    held = pending;
                    pending = null;
                    held.Grab(holdPoint, playerCollider);
                }
            }
        }

        if (held != null && (!kb.eKey.isPressed || player.topDownMode || held.TooFar()))
        {
            held.Release();
            held = null;
        }

        float carry = Mathf.MoveTowards(animator.GetLayerWeight(carryLayer), held != null ? 1f : 0f, 4f * Time.deltaTime);
        animator.SetLayerWeight(carryLayer, carry);

        if (lockTimer > 0f) lockTimer -= Time.deltaTime;
        player.locked = lockTimer > 0f;

        if (messageTimer > 0f) messageTimer -= Time.deltaTime;
    }

    void LateUpdate()
    {
        holdPoint.position = (leftHand.position + rightHand.position) / 2f + transform.forward * 0.05f;
        holdPoint.rotation = transform.rotation;
    }

    Interactable FindTarget()
    {
        Vector3 eye = transform.position + Vector3.up * 1.5f;
        Vector3 camPos = cam.transform.position;

        Interactable best = null;
        float bestAngle = maxAngle;

        foreach (Interactable obj in Interactable.All)
        {
            if (obj.IsHeld) continue;
            Collider col = obj.GetComponent<Collider>();
            Vector3 flat = col.ClosestPoint(transform.position) - transform.position;
            flat.y = 0f;
            if (flat.magnitude > interactDistance) continue;

            float angle = Vector3.Angle(cam.transform.forward, col.bounds.center - camPos);
            if (angle > bestAngle) continue;

            Vector3 dir = col.bounds.center - eye;
            if (Physics.Raycast(eye, dir.normalized, out RaycastHit hit, dir.magnitude + 0.5f) && hit.collider.GetComponentInParent<Interactable>() != obj) continue;

            best = obj;
            bestAngle = angle;
        }
        return best;
    }

    void OnGUI()
    {
        int font = Mathf.RoundToInt(Screen.height * 0.035f);
        GUIStyle style = new GUIStyle(GUI.skin.box) { fontSize = font, alignment = TextAnchor.MiddleCenter };
        style.normal.textColor = Color.white;

        string text = null;
        if (looked != null) text = "[E] Weź: " + looked.displayName;
        else if (held != null) text = "Trzymasz: " + held.displayName + " (puść E, aby upuścić)";

        if (text != null)
        {
            Vector2 size = style.CalcSize(new GUIContent(text)) + new Vector2(font, font * 0.4f);
            GUI.Box(new Rect((Screen.width - size.x) / 2f, Screen.height * 0.9f - size.y, size.x, size.y), text, style);
        }

        if (messageTimer > 0f)
        {
            GUIStyle big = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(font * 1.3f), alignment = TextAnchor.MiddleCenter };
            big.normal.textColor = Color.yellow;
            GUI.Label(new Rect(0, Screen.height * 0.12f, Screen.width, font * 2f), message, big);
        }
    }
}
