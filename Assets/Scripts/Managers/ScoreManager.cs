using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // === Managers ===
    [SerializeField] private CollisionManager collisionManager;
    [SerializeField] private UIManager uiManager;

    // === Score ===
    private int hammerScore = 0;
    private int moleScore = 0;

    // === Events ===
    public event Action<int, Vector3> OnHammerScoreChanged;
    public event Action<int, Vector3> OnMoleScoreChanged;

    void Awake()
    {
        if (collisionManager != null)
        {
            collisionManager.OnHitSuccess += AddHammerScore;
            collisionManager.OnHitMiss += AddMoleScore;
        }
    }

    private void AddHammerScore(HammerController hammer)
    {
        Vector3 hammerHolePosition = hammer.holeNavigationScript.CurrentHole.transform.position;
        hammerScore++;
        OnHammerScoreChanged?.Invoke(hammerScore, hammerHolePosition);
    }

    private void AddMoleScore(MoleController mole)
    {
        Vector3 moleHolePosition = mole.holeNavigationScript.CurrentHole.transform.position;
        moleScore++;
        OnMoleScoreChanged?.Invoke(moleScore, moleHolePosition);
    }

    private void ResetScores()
    {
        hammerScore = 0;
        moleScore = 0;

        OnHammerScoreChanged?.Invoke(hammerScore, Vector3.zero);
        OnMoleScoreChanged?.Invoke(moleScore, Vector3.zero);
    }
}