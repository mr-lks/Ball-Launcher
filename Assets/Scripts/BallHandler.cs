using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab; // Prefab for the ball object
    [SerializeField] private Rigidbody2D pivot; // Pivot point for the spring joint
    [SerializeField] private float respawnDelay; // Delay before respawning a new ball
    [SerializeField] private float detachDelay = 0.5f; // Delay before detaching the ball

    private Rigidbody2D currentBallRigidbody; // Reference to the current ball's Rigidbody component
    private SpringJoint2D currentBallSpringJoint; // Reference to the current ball's SpringJoint2D component

    private Camera mainCamera; // Reference to the main camera in the scene
    private bool isDragging; // Flag to track if the ball is being dragged

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera in the scene

        SpawnNewBall(); // Spawn a new ball when the game starts
    }

    void Update()
    {
        if (currentBallRigidbody == null) { return; } // If there's no current ball, exit the update loop

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            // If the primary touch is not pressed and the ball was being dragged, launch the ball
            if (isDragging)
            {
                LaunchBall();
            }
            isDragging = false; // Reset the dragging flag
            return;
        }

        isDragging = true; // Set the dragging flag to true
        currentBallRigidbody.isKinematic = true; // Make the ball kinematic to prevent it from being affected by physics

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue(); // Get the position of the primary touch
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition); // Convert the touch position to world coordinates

        currentBallRigidbody.position = worldPosition; // Move the ball to the new position based on the touch input
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false; // Make the ball non-kinematic to allow it to be affected by physics
        currentBallRigidbody = null; // Clear the reference to the current ball Rigidbody

        Invoke(nameof(DetachBall), detachDelay); // Invoke the DetachBall method after a delay
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false; // Disable the spring joint to detach the ball from the pivot
        currentBallSpringJoint = null; // Clear the reference to the current ball SpringJoint2D

        Invoke(nameof(SpawnNewBall), respawnDelay); // Invoke the SpawnNewBall method after a delay to spawn a new ball
    }

    private void SpawnNewBall()
    {
        // Instantiate a new ball at the pivot position with no rotation
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>(); // Get the Rigidbody component of the new ball
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>(); // Get the SpringJoint2D component of the new ball

        currentBallSpringJoint.connectedBody = pivot; // Set the connected body of the spring joint to the pivot
    }
}
