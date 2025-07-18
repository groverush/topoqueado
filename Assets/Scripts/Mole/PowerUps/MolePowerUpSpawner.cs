using System.Collections;
using UnityEngine;

public class MolePowerUpSpawner : MonoBehaviour
{
    [SerializeField] private BasePowerUp[] molePowerUpPrefabs;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private MolePowerUpManager molePowerUpManager;
    [SerializeField] private HoleNavigation holeNavigationScript;

    private Coroutine spawnRoutine;
    private BasePowerUp currentPowerUp;

    private void Start ()
    {
        molePowerUpManager.OnMoleVisionEnd += ResumeSpawning;
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

            if (molePowerUpManager.IsAnyPowerUpActive())
                continue;

            if (currentPowerUp == null)
            {
                SpawnPowerUp();
            }
        }
    }

    private void ResumeSpawning ()
    {
        if (!molePowerUpManager.IsAnyPowerUpActive())
        {
            StartSpawning();
        }
    }

    private void SpawnPowerUp ()
    {
        if (molePowerUpPrefabs.Length == 0 || holeNavigationScript.Holes.Count == 0) return;

        int randomIndex = Random.Range(0, molePowerUpPrefabs.Length);
        BasePowerUp newPowerUp = Instantiate(
            molePowerUpPrefabs[randomIndex],
            holeNavigationScript.GetRandomHole().transform.position,
            molePowerUpPrefabs[randomIndex].transform.rotation
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
        molePowerUpManager.OnMoleVisionEnd -= ResumeSpawning;
    }
}
