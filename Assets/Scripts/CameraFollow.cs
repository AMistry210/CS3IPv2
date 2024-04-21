using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the spaceship's Transform
    public Transform cameraPivot; // Reference to the pivot point for camera rotation
    public float distance = 5f; // Distance from the spaceship
    public float height = 2f; // Height above the spaceship
    public float damping = 5f; // How quickly the camera should move
    public LayerMask collisionLayerMask; // Layer mask for collision detection

    // Reference to the face tracking position (assuming you have access to it)
    public Transform faceTracker;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private void Start()
    {
        // Store the initial local position and rotation of the camera
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        // Start the coroutine to update the camera position
        StartCoroutine(UpdateCameraPosition());
    }

    private IEnumerator UpdateCameraPosition()
    {
        while (true)
        {
            yield return null;

            if (target == null || faceTracker == null)
                continue;

            // Calculate the direction from the spaceship to the face tracker
            Vector3 directionToFace = faceTracker.position - target.position;

            // Project the direction onto the plane perpendicular to the spaceship's forward direction
            Vector3 projectedDirection = Vector3.ProjectOnPlane(directionToFace, target.forward);

            // Calculate the desired position for the camera
            Vector3 desiredPosition = target.position + projectedDirection.normalized * distance + Vector3.up * height;

            // Check for collisions between current position and desired position
            RaycastHit hit;
            if (Physics.Linecast(target.position, desiredPosition, out hit, collisionLayerMask))
            {
                // Adjust desired position to avoid collision
                transform.position = hit.point;
            }
            else
            {
                // Smoothly move the camera towards the desired position using Lerp
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
            }

            // Make the camera pivot point look at the spaceship
            cameraPivot.LookAt(target);

            // Reset the camera's rotation relative to the spaceship
            transform.localRotation = initialLocalRotation;
        }
    }
}
