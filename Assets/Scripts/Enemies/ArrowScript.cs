using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    private Rigidbody rb;

    // References to the front and back markers
    [SerializeField] private Transform frontMarker;
    [SerializeField] private Transform backMarker;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction)
    {
        // Normalize the direction vector
        direction.Normalize();

        // Calculate the desired rotation based on the movement direction
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Align the arrow's forward direction with the movement direction
        transform.rotation = targetRotation;

        // Calculate the velocity to move in the given direction
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem playerHealth = collision.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
