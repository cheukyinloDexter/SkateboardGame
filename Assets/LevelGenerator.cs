using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] spawnPrefabs; // Array of prefabs to spawn
    public int objectCount = 10;      // Number of objects to spawn
    public float spacing = 5f;        // Distance between spawned objects
    public Vector3 startPosition = Vector3.zero; // Starting position for spawning
    public Vector3 spawnDirection = Vector3.forward; // Direction to spawn objects in (e.g., forward)

    [Header("Randomization Settings")]
    public bool randomRotation = true;    // Randomize rotation of spawned objects
    public Vector3 rotationRange = new Vector3(0, 360, 0); // Range of random rotations (X, Y, Z)

    public bool randomOffset = true;      // Add random offset to spawn positions
    public Vector3 offsetRange = new Vector3(2f, 0, 0); // Range for random offsets (X, Y, Z)

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // TODO if the distace to the last obsticale is > than ?, add a higher point/item to incentivze doing the flip
        Vector3 currentPosition = startPosition;

        for (int i = 0; i < objectCount; i++)
        {
            // Select a random prefab
            GameObject prefabToSpawn = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

            // Apply random offset to position
            Vector3 offset = randomOffset
                ? new Vector3(
                    Random.Range(-offsetRange.x, offsetRange.x),
                    Random.Range(-offsetRange.y, offsetRange.y),
                    Random.Range(-offsetRange.z, offsetRange.z))
                : Vector3.zero;

            // Spawn the object
            GameObject spawnedObject = Instantiate(prefabToSpawn, currentPosition + offset, Quaternion.identity);

            // Apply random rotation if enabled
            if (randomRotation)
            {
                Vector3 randomEulerAngles = new Vector3(
                    Random.Range(0, rotationRange.x),
                    Random.Range(0, rotationRange.y),
                    Random.Range(0, rotationRange.z));
                spawnedObject.transform.rotation = Quaternion.Euler(randomEulerAngles);
            }

            // Move to the next position in the line
            currentPosition += spawnDirection.normalized * spacing;
        }
    }
}
