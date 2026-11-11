using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject gun;
    public Transform muzzle;
    public Camera cam;
    public Bullet normalBullet;
    public Bullet stickyBullet;
    public Texture2D crosshair;
    public float bulletSpeed = 40f;

    PlayerMovement player;
    Animator animator;
    Collider playerCollider;
    bool aiming;
    bool sticky;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
        gun.SetActive(false);
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        aiming = !player.topDownMode && mouse.rightButton.isPressed;
        gun.SetActive(aiming);
        animator.SetLayerWeight(1, Mathf.MoveTowards(animator.GetLayerWeight(1), aiming ? 1f : 0f, 8f * Time.deltaTime));

        if (Keyboard.current.qKey.wasPressedThisFrame) sticky = !sticky;

        if (aiming && mouse.leftButton.wasPressedThisFrame) Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Vector3 point = Physics.Raycast(ray, out RaycastHit hit, 200f) ? hit.point : ray.GetPoint(200f);
        Vector3 dir = (point - muzzle.position).normalized;

        Bullet bullet = Instantiate(sticky ? stickyBullet : normalBullet, muzzle.position, Quaternion.LookRotation(dir));
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), playerCollider);
        bullet.GetComponent<Rigidbody>().linearVelocity = dir * bulletSpeed;
    }

    void OnGUI()
    {
        if (player.topDownMode) return;
        GUI.Label(new Rect(10, 10, 300, 25), "Pocisk: " + (sticky ? "lepki (Q)" : "zwykły (Q)"));
        if (aiming) GUI.DrawTexture(new Rect(Screen.width / 2f - 16, Screen.height / 2f - 16, 32, 32), crosshair);
    }
}
