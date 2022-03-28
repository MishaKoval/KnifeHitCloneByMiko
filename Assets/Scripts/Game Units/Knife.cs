using UnityEngine;
using UnityEngine.Events;

public class Knife : MonoBehaviour
{
    
    public UnityEvent onKnifeHit;

    public bool isHited;

    public bool isAfterLogDestroy;
    
    #region PrivateFields

    private Rigidbody2D rb;

    private BoxCollider2D boxCollider2D;

    private bool isKnifeHitKnife;

    private ParticleSystem woodParcitle;

    #endregion
   

    #region MonoBehaviour

    private void Awake()
    {
        woodParcitle = GetComponent<ParticleSystem>();
        isHited = false;
        isAfterLogDestroy = false;
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Finish"))
        {
            onKnifeHit.RemoveAllListeners();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Knife"))
        {
            isKnifeHitKnife = true;
            rb.velocity = new Vector2(rb.velocity.x, -7.0f);
            rb.AddTorque(4, ForceMode2D.Impulse);
            if (gameObject.transform.parent)
            {
                GameManager.Instance.LoseGame();
                Vibration.Vibrate();
            }
            else
            {
                boxCollider2D.isTrigger = true;
            }
        }

        if (col.gameObject.CompareTag("Wheel") && !isKnifeHitKnife)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(col.collider.transform);
            boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, -0.69f);
            boxCollider2D.size = new Vector2(boxCollider2D.size.x, 1.15f);
            woodParcitle.Play();
            onKnifeHit?.Invoke();
        }
    }

    #endregion
}