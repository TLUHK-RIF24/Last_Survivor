using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movespeed;
    
    public Rigidbody2D rb;

    private Vector2 moveDirection;

    void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.dKey.isPressed) moveX = 1f;
        else if (Keyboard.current.aKey.isPressed) moveX = -1f;

        if (Keyboard.current.wKey.isPressed) moveY = 1f;
        else if (Keyboard.current.sKey.isPressed) moveY = -1f;

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * movespeed, moveDirection.y * movespeed);
    }
}