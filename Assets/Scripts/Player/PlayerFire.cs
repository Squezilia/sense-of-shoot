using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFire : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject bullet;
    [SerializeField] private InputActionReference fireActionReference;

    public float recoilMagnitude = 600f;
    public float bulletTolerance = 1f;
    public float bulletSpeed = 40f;
    public float bulletTtl = 5f;
    public double bulletDamage = 5;

    private float lastFire = 0f;
    public float fireSpeed = 0.1f;

    private void OnEnable()
    {
        if (fireActionReference == null) fireActionReference.action.Enable();
    }

    private void OnDisable()
    {
        if (fireActionReference != null) fireActionReference.action.Disable();
    }
    
    void Update()
    {
        if (fireActionReference == null) return;

        if (!fireActionReference.action.IsPressed() || lastFire + fireSpeed > Time.time) return;
        
        GameObject clone = Instantiate(bullet);
        
        clone.GetComponent<Bullet>().Instantiate(rb.linearVelocity, bulletDamage, bulletTolerance, bulletSpeed, bulletTtl, this.gameObject);

        float normalRotation = (transform.rotation.eulerAngles.z + 180) * Mathf.Deg2Rad;
        rb.AddForce(new Vector2(Mathf.Cos(normalRotation), Mathf.Sin(normalRotation)) * recoilMagnitude, ForceMode2D.Force);

        lastFire = Time.time;
    }
}
