using UnityEngine;

public class ShrinkingWall : MonoBehaviour
{
    // Enum to define the direction of movement
    public enum ShrinkDirection
    {
        Left,  // Moves Right
        Right, // Moves Left
        Front, // Moves Backward
        Back   // Moves Forward
    }

    public ShrinkDirection shrinkDirection;  // Direction assigned in the inspector
    public float shrinkSpeed = 1.0f;         // Speed at which the wall moves
    public float waitTime;            // Time to wait before starting movement

    private Vector3 initialPosition;         // Initial position of the wall
    private float startTime;                 // Time when the movement starts

    void Start()
    {
        // Store the initial position of the wall
        initialPosition = transform.position;

        // Record the time when the script starts
        startTime = Time.time;
    }

    void Update()
    {
        // Check if the wait time has passed
        if (Time.time >= startTime + waitTime)
        {
            MoveWall();
        }
    }

    void MoveWall()
    {
        // Calculate the movement based on the direction
        switch (shrinkDirection)
        {
            case ShrinkDirection.Left:
                transform.position += Vector3.right * shrinkSpeed * Time.deltaTime;
                break;

            case ShrinkDirection.Right:
                transform.position += Vector3.left * shrinkSpeed * Time.deltaTime;
                break;

            case ShrinkDirection.Front:
                transform.position += Vector3.back * shrinkSpeed * Time.deltaTime;
                break;

            case ShrinkDirection.Back:
                transform.position += Vector3.forward * shrinkSpeed * Time.deltaTime;
                break;
        }
    }

    // Optionally reset the position if needed
    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
