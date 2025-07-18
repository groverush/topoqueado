using System.Collections;
using UnityEngine;

public class HammerPowerUpSpawner : MonoBehaviour
{
    [SerializeField] private BasePowerUp[] powerUpPrefabs;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private HammerPowerUpManager hammerPowerUpsManager;
    [SerializeField] private HoleNavigation holeNavigationScript;

    private Coroutine spawnRoutine;
    private BasePowerUp currentPowerUp;

    private void Start ()
    {
        hammerPowerUpsManager.OnDoubleHitEnd += ResumeSpawning;
        hammerPowerUpsManager.OnHammerVisionEnd += ResumeSpawning;
        StartSpawning();
    }

    private void StartSpawning ()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawning ()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop ()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (hammerPowerUpsManager.IsDoubleHitActive || hammerPowerUpsManager.IsHammerVisionActive)
            {
                StopSpawning();
                yield break;
            }

            if (currentPowerUp == null)
            {
                SpawnPowerUp();
            }
        }
    }

    private void ResumeSpawning ()
    {
        if (!hammerPowerUpsManager.IsDoubleHitActive && !hammerPowerUpsManager.IsHammerVisionActive)
        {
            StartSpawning();
        }
    }

    private void SpawnPowerUp ()
    {
        if (powerUpPrefabs.Length == 0 || holeNavigationScript.Holes.Count == 0) return;

        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        BasePowerUp newPowerUp = Instantiate(
            powerUpPrefabs[randomIndex],
            holeNavigationScript.GetRandomHole().transform.position,
            powerUpPrefabs[randomIndex].transform.rotation
        );

        newPowerUp.OnCollected += HandlePowerUpCollected;
        currentPowerUp = newPowerUp;
    }

    private void HandlePowerUpCollected ()
    {
        currentPowerUp = null;
    }

    private void OnDestroy ()
    {
        hammerPowerUpsManager.OnDoubleHitEnd -= ResumeSpawning;
        hammerPowerUpsManager.OnHammerVisionEnd -= ResumeSpawning;
    }
}
