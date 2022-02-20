using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float spawnDelay = 3f;
    [SerializeField] private GameObject buddyGuyPrefab;
    [SerializeField] AudioClip spawnInSound;

    private void Start()
    {
        StartCoroutine(SpawnBuddyGuy());
    }

    private IEnumerator SpawnBuddyGuy()
    {
        Instantiate(buddyGuyPrefab, transform.position, transform.rotation);
        AudioManager.instance.PlaySFX(spawnInSound);
        yield return new WaitForSeconds(spawnDelay);
        StartCoroutine(SpawnBuddyGuy());
    }
}
