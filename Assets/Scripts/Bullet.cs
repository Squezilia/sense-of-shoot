using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    public double damage;
    public float speed;
    [SerializeField] public Rigidbody2D rb;
    public GameObject issuer;
    
    public void Instantiate(Vector2 parentVelocity, double damage, float tolerance, float speed, float ttl, GameObject sender)
    {
        issuer = sender;
        this.damage = damage;
        this.speed = speed;
        
        float randomizedTolerance = Random.Range(-tolerance, tolerance);
        float lookRotation = (sender.transform.rotation.eulerAngles.z + randomizedTolerance) * Mathf.Deg2Rad;
        
        Transform parentTransform = sender.transform;
        float xOffset = parentTransform.localScale.x * Mathf.Cos(lookRotation);
        float yOffset = parentTransform.localScale.y * Mathf.Sin(lookRotation);
        Vector2 spawnPosition = new(
            parentTransform.position.x + xOffset, 
            parentTransform.position.y + yOffset
        );
        
        Quaternion bulletRotation = parentTransform.rotation;
        Quaternion correctionRotation = Quaternion.Euler(0, 0, -90f + randomizedTolerance);

        transform.position = spawnPosition;
        transform.rotation = bulletRotation * correctionRotation;
        
        Vector2 bulletVelocity = new Vector2(Mathf.Cos(lookRotation), Mathf.Sin(lookRotation)) * speed;
        rb.AddForce(parentVelocity + bulletVelocity, ForceMode2D.Impulse);
        
        Destroy(gameObject, ttl);
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
