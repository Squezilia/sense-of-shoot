using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    public double health = 100;
    private double maxHealth;
    
    [SerializeField] public Canvas canvas;
    [SerializeField] public GameObject healthBarPrefab;
    private GameObject healthBar;
    private RectTransform healthTransform;
    private float healthBarWidth;
    private float healthBarHeight;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet")) return;
        
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet.issuer.Equals(this.gameObject)) return;
        
        float bulletRotation = (other.gameObject.transform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
        Vector2 bulletForce = new(Mathf.Cos(bulletRotation), Mathf.Sin(bulletRotation));
        
        health -= bullet.damage;
        
        rb.AddForce(bulletForce * bullet.speed, ForceMode2D.Impulse);
        
        Destroy(other.gameObject);
    }

    private void Start()
    {
        maxHealth = health;
        
        rb = GetComponent<Rigidbody2D>();
        
        healthBar = Instantiate(healthBarPrefab, canvas.transform);
        
        healthTransform = healthBar.GetComponent<RectTransform>();
        healthBarWidth = healthTransform.sizeDelta.x;
        healthBarHeight = healthTransform.sizeDelta.y;
    }

    private void Update()
    {
        if (health <= 0) Destroy(gameObject);
        
        healthTransform.sizeDelta = new Vector2((float)(health / maxHealth) * healthBarWidth, healthBarHeight);
        
        healthTransform.position = new Vector3(
            transform.position.x,
            transform.position.y + (transform.localScale.y / 2),
            transform.position.z);
    }
}
