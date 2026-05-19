using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public PlayerAnimatorHandler animatorHandler;   // ← Drag the component here
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



        animatorHandler.SetFacing(moveX);           // for left/right flip
    }

    void Move()
    {
        bool isMoving = moveDirection.magnitude > 0.01f;
        animatorHandler.SetRunning(isMoving);
        float speed = PlayerStats.Instance.moveSpeed;
        rb.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }
}
