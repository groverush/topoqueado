using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HoleNavigation : MonoBehaviour
{
    // === Holes ===
    private List<GameObject> holes = new();
    private enum HoleType { Hammer, Mole };
    [SerializeField] private HoleType currentHoleType;
    private GameObject currentHole;

    // === Properties ===
    public GameObject CurrentHole => currentHole;

    public List<GameObject> Holes => holes;

    void Awake()
    {
        InitializeHoleList();
    }

    private void InitializeHoleList()
    {
        if (currentHoleType == HoleType.Hammer)
        {
            // Search for hammer holes and add them to the list
            Transform hammerHolesContainer = GameObject.Find("Box")?.transform.Find("HammerHoles");

            if (hammerHolesContainer != null)
            {
                foreach (Transform hammerHole in hammerHolesContainer.transform)
                {
                    holes.Add(hammerHole.gameObject);
                }
            }
        }
        else
        {
            // Search for mole holes and add them to the list
            Transform moleHolesContainer = GameObject.Find("Box")?.transform.Find("MoleHoles");

            if (moleHolesContainer != null)
            {
                foreach (Transform moleHole in moleHolesContainer.transform)
                {
                    holes.Add(moleHole.gameObject);
                }
            }
        }

        // Select the center hole in the list as the current hole and highlight it 
        if (holes.Count > 0)
        {
            currentHole = holes[holes.Count / 2];
            Highlight(currentHole);
        }
    }

    // Move to the nearest hole in the direction of the pressed key
    public void SelectHole(Vector2 movementInput, Vector3 horizontalVector, Vector3 verticalVector)
    {
        if (currentHole == null) return;

        Vector3 currentPos = currentHole.transform.position;

        // Determine dominant axis (horizontal or vertical)
        Vector3 axis;
        float directionSign;

        if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
        {
            axis = horizontalVector;
            directionSign = Mathf.Sign(movementInput.x);
        }
        else
        {
            axis = verticalVector;
            directionSign = Mathf.Sign(movementInput.y);
        }

        GameObject closestHole = null;
        float shortestDistance = float.MaxValue;

        // Search for the nearest hole
        foreach (var hole in holes)
        {
            if (hole == currentHole) continue;

            Vector3 toNextHole = hole.transform.position - currentPos;

            // Projection on the principal axis (X or Z)
            float axisProjection = Vector3.Dot(axis, toNextHole.normalized);

            // Filtering if the object is not sufficiently aligned with the desired direction
            if (axisProjection * directionSign < 0.5f) continue;

            float distance = toNextHole.sqrMagnitude;

            if (distance < shortestDistance)
            {
                closestHole = hole;
                shortestDistance = distance;
            }
        }

        // If a valid object was found, the highlight is updated
        if (closestHole != null)
        {
            RemoveHighlight(currentHole);
            currentHole = closestHole;
            Highlight(currentHole);
        }
    }

    private void Highlight(GameObject obj)
    {
        if (obj != null && obj.TryGetComponent<Outline>(out var outline))
        {
            outline.enabled = true;
        }
    }

    private void RemoveHighlight(GameObject obj)
    {
        if (obj != null && obj.TryGetComponent<Outline>(out var outline))
        {
            outline.enabled = false;
        }
    }
    public GameObject GetRandomHoleExcluding ( GameObject excludedHole )
    {
        if (holes.Count <= 1) return null;

        List<GameObject> availableHoles = new List<GameObject>(holes);
        availableHoles.Remove(excludedHole);

        int randomIndex = Random.Range(0, availableHoles.Count);
        return availableHoles[randomIndex];
    }
    public GameObject GetRandomHole ()
    {
        if (holes.Count <= 1) return null;

        int randomIndex = Random.Range(0, holes.Count);
        return holes[randomIndex];
    }
}