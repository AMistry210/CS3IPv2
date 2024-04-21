using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionHandler : MonoBehaviour
{
    public string asteroidTag = "Asteroid"; // Tag for your asteroids
    public string planetTag = "Planet"; // Tag for your planet
    public string NoPassLayer = "NoPassLayer"; // Name of the "no pass" layer
    public GameObject gameOverScreen; // Reference to the game over screen UI object
    public GameObject EndScreen; // Reference to the thank you screen UI object

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(asteroidTag))
        {
            // Game over logic
            Debug.Log("Game Over - Spaceship collided with an asteroid!");
            // Display game over screen
            if (gameOverScreen != null)
            {
                gameOverScreen.SetActive(true); // Activate game over screen
            }
        }
        else if (collision.gameObject.CompareTag(planetTag))
        {
            // You win logic
            Debug.Log("Congratulations! You win - Spaceship reached the planet!");
            // Display thank you screen
            if (EndScreen != null)
            {
                EndScreen.SetActive(true); // Activate thank you screen
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(NoPassLayer))
        {
            // Collision with the "no pass" layer logic
            Debug.Log("Collision with no pass layer!");
            // You can add code here to handle the collision with the "no pass" layer
            // For example, you might want to prevent the spaceship from passing through or perform other actions.
        }
    }
}
