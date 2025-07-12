using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunkspawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject nextChunkPrefab;
    public Transform spawnPoint;
    private bool hasSpawnedNext = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        if (other.CompareTag("Player") && !hasSpawnedNext)
        {
            Debug.Log("Spawning");
            hasSpawnedNext = true;

            Instantiate(nextChunkPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ReturnToPoolAfterDelay(20f));

        }
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //ChunkPoolManager.Instance.ReturnToPool(chunkTag, transform.parent.gameObject);
        Destroy(transform.parent.gameObject);
    }
}
