using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
   
    [SerializeField] private Camera mainCamera;
    
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference runActionReference;
    
    public float maxSpeed = 10f;
    public float runMaxSpeed = 15f;
    public float acceleration = 80f;
    public float decceleration = 160f;

    private Vector2 currentVelocity;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }
    
    private void OnEnable()
    {
        if (moveActionReference == null || runActionReference == null)
        {
            moveActionReference.action.Enable();
            runActionReference.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveActionReference != null || runActionReference != null)
        {
            moveActionReference.action.Enable();
            runActionReference.action.Enable();
        }
    }

    void Update()
    {
        if (moveActionReference == null || runActionReference == null) return;

        //
        // Look
        //
        Vector2 mouseVector = Mouse.current.position.ReadValue();
        Vector2 lookVector = mainCamera.ScreenToWorldPoint(mouseVector);
        
        Vector2 playerVector = rb.position;
        Vector2 distanceVector = lookVector - playerVector;

        var lookDegree = Mathf.Atan2(distanceVector.y, distanceVector.x);
        
        rb.rotation = lookDegree * Mathf.Rad2Deg;
        
        //
        // Movement
        //
        Vector2 moveVector = moveActionReference.action.ReadValue<Vector2>();

        Vector2 targetVelocity = runActionReference.action.IsPressed() ? moveVector * runMaxSpeed : moveVector * maxSpeed;
        float currentAcceleration = (moveVector.sqrMagnitude > 0) ? acceleration : decceleration;
        
        currentVelocity =
            Vector2.MoveTowards(currentVelocity, targetVelocity, currentAcceleration * Time.deltaTime);
        rb.linearVelocity = currentVelocity;
    }
}
