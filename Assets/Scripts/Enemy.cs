using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class StandingHostile : MonoBehaviour
{
    [Header("Globals")]
    [SerializeField] public Rigidbody2D rb;
    private bool lockInTarget = false;
    
    [Header("Fire")]
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject bullet;
    
    public float fireSpeed = 0.2f;
    public float recoilMagnitude = 600f;
    public float bulletTolerance = 2f;
    public float bulletToleranceIncreasePerHit = 1f;
    public float bulletSpeed = 40f;
    public float bulletTtl = 5f;
    public double bulletDamage = 10;
    private float lastFire = 0f;
    private float bulletToleranceIncrease = 0f;
    
    [Header("Entity Sight")]
    [SerializeField] public LayerMask targetLayers;
    public float fov = 90f;
    public float rayStep = 12f;
    public float knownRayStep = 10f;
    public float fovDepth = 15f;
    public float knownFovDepth = 20f;
    public float pollingRate = 0.1f;
    public float knownPollingRate = 0.01f;

    [Header("Entity React")] 
    public float senseOfTolerance = 10f;
    public float depthIncrease = 10f;
    private bool isHit = false;

    private void FixedUpdate()
    {
        float rate = lockInTarget ? knownPollingRate : pollingRate;
        if (Time.fixedTime % rate > Time.fixedDeltaTime) return;
        
        float step = lockInTarget ? knownRayStep : rayStep;
        float depth = lockInTarget ? knownFovDepth : fovDepth;
        depth = isHit ? depth + depthIncrease : depth;
        float rayCount = (fov / step);
    
        for (int i = 0; i < rayCount; i++)
        {
            float rayRotation = (this.transform.rotation.eulerAngles.z - (fov / 2) + (i * rayStep)) *  Mathf.Deg2Rad;
            Vector2 rayVector = new(Mathf.Cos(rayRotation), Mathf.Sin(rayRotation));

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayVector, depth, targetLayers);
            Debug.DrawRay(transform.position, rayVector * depth, Color.green);
            
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                lockInTarget = true;
                return;
            }
        }

        lockInTarget = false;
    }

    void Update()
    {
        if (!lockInTarget) return;
        //
        // rotate
        //
        Vector2 playerVector = player.transform.position;
        Vector2 distanceVector = playerVector - (Vector2)transform.position;
        
        float playerAngle = Mathf.Atan2(distanceVector.y, distanceVector.x);
        
        transform.rotation = Quaternion.Euler(0, 0, playerAngle * Mathf.Rad2Deg);
            
        //
        // fire
        //
        if (lastFire + fireSpeed > Time.time) return;
        
        GameObject clone = Instantiate(bullet);
        
        clone.GetComponent<Bullet>().Instantiate(rb.linearVelocity, bulletDamage, Mathf.Min(bulletTolerance + bulletToleranceIncrease, fov / 9) , bulletSpeed, bulletTtl, this.gameObject);

        float normalRotation = (transform.rotation.eulerAngles.z + 180) * Mathf.Deg2Rad;
        rb.AddForce(new Vector2(Mathf.Cos(normalRotation), Mathf.Sin(normalRotation)) * recoilMagnitude, ForceMode2D.Force);

        lastFire = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet")) return;
        isHit = true;
        bulletToleranceIncrease += Random.Range(0, bulletToleranceIncreasePerHit);
        
        float randomizedTolerance = Random.Range(-senseOfTolerance, senseOfTolerance);
        
        // enemy got hit and will turn to from bullet
        float incomingRotation = other.gameObject.transform.rotation.eulerAngles.z + 90f + 180f + randomizedTolerance;
        rb.rotation = incomingRotation;
    }
}
