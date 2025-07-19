using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float bounceAmplitude = 0.1f; // qué tanto sube y baja
    [SerializeField] private float bounceFrequency = 3f;    // velocidad del salto

    private Vector3 startPosition;

    private void Start ()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update ()
    {
        // Rotación del objeto
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Movimiento vertical de tipo "bouncing"
        float newY = startPosition.y + Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
