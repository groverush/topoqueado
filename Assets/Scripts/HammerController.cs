using UnityEngine;

public class HammerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {
        transform.position += transform.forward * Time.deltaTime; // Move the hammer forward
    }

    public void Hit()
    {
        transform.Rotate(Vector3.forward * 90); // Rotate the hammer by 90 degrees around the Z-axis
        // Logic for hitting with the hammer
        Debug.Log("Hammer hit!");
    }
}
