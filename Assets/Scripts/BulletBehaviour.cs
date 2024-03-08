using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private float speed = 10f; // Speed of the bullet
    [SerializeField] private ParticleSystem particles; // Reference to the explosion particle system prefab
    [SerializeField] private int damage = 2; // Damage the bullet deals
    public float duration = 3f;

    void FixedUpdate()
    {
        Vector3 movement = transform.up * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    void Start()
    {
        Destroy(gameObject, duration);
        Invoke("Explode", 4f);  // Fallback if it escapes
    }

    public void SetDirection(float angle, float bSpeed)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        speed = bSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("WallTiles") || collision.CompareTag("Enemy"))
        {
            Explode();
            DealDamage(collision);
        }
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    private void Explode()
    {
        // Instantiate the particle system before destroying the bullet
        if (particles != null)
        {
            Instantiate(particles, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void InitializeBullet(Weapon weapon)
    {
        if (weapon != null)
        {
            speed = weapon.hitboxSpeed;
            duration = weapon.duration;
        }
    }

    public void SetDuration(float life)
    {
        duration = life;
    }



    public Vector2 boxOffset = Vector2.zero; 
    public Vector2 boxSize = Vector2.one; 
    public Color boxColor = Color.green; 

    void OnDrawGizmos()
    {
        Gizmos.color = boxColor;
        Vector3 boxOffset3D = new Vector3(boxOffset.x, boxOffset.y, 0);
        Vector3 positionWithOffset = transform.position + boxOffset3D;
        Vector3 sizeWithMinimalDepth = new Vector3(boxSize.x, boxSize.y, 1);
        Gizmos.DrawWireCube(positionWithOffset, sizeWithMinimalDepth);
    }

}
