using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerItem : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f; // Speed at which the item floats up and down
    [SerializeField] private float floatAmount = 0.5f; // Amount the item floats up and down
    [SerializeField] private AudioClip pickupSound; // Audio clip to play when the item is collected
    [SerializeField] private float timeToAdd = 10f; // Amount of time to add when the item is collected

    private float startY;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial Y position for floating
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Make the item float up and down
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(transform.position.x, startY + yOffset, transform.position.z);
    }

    // Trigger when another object enters the collider of this item
    void OnTriggerEnter(Collider other)
    {
        // Check if the other game object is the player (you can adjust this depending on your player object)
        if (other.CompareTag("Player"))
        {
            // Call a method to add time (this could be a method on a game manager or timer)
            GameManager.Instance.AddTime(timeToAdd); // Assuming GameManager has an AddTime method

            // Play the pickup sound
            //AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // Destroy the item
            Destroy(gameObject);
        }
    }
}
