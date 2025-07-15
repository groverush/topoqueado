using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private BasePowerUp[] powerUpPrefabs;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private HammerPowerUps hammerPowerUps; // Asignar desde inspector
    [SerializeField] private HoleNavigation holeNavigationScript;

    private Coroutine spawnRoutine;
    private BasePowerUp currentPowerUp;

    private void Start ()
    {
        if (hammerPowerUps != null)
        {
            hammerPowerUps.OnDoubleHitEnd += ResumeSpawning;
        }

        StartSpawning();
    }

    private void StartSpawning ()
    {
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    private void StopSpawning ()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
            Debug.Log("Spawner detenido mientras DoubleHit está activo.");
        }
    }

    private IEnumerator SpawnLoop ()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (hammerPowerUps == null || !hammerPowerUps.IsDoubleHitActive())
            {
                if (currentPowerUp == null) // Solo si no hay un power-up activo en el escenario
                {
                    SpawnPowerUp();
                }
                else
                {
                    Debug.Log("Esperando a que se consuma el power-up antes de generar uno nuevo.");
                }
            }
            else
            {
                StopSpawning();
                yield break;
            }
        }
    }

    private void ResumeSpawning ()
    {
        Debug.Log("DoubleHit finalizado. Reanudando spawner.");
        StartSpawning();
    }

    private void SpawnPowerUp ()
    {
        if (powerUpPrefabs.Length == 0 || holeNavigationScript.Holes.Count == 0) return;

        int randomPrefabIndex = Random.Range(0, powerUpPrefabs.Length);

        BasePowerUp newPowerUp = Instantiate(powerUpPrefabs[randomPrefabIndex], holeNavigationScript.GetRandomHole().transform.position, Quaternion.identity);
        currentPowerUp = newPowerUp;

        newPowerUp.OnCollected += HandlePowerUpCollected;

        Debug.Log("PowerUp instanciado.");
    }

    private void HandlePowerUpCollected ()
    {
        Debug.Log("PowerUp recogido, se habilita nuevo spawn.");
        currentPowerUp = null;
    }

    private void OnDestroy ()
    {
        if (hammerPowerUps != null)
        {
            hammerPowerUps.OnDoubleHitEnd -= ResumeSpawning;
        }
    }
}
