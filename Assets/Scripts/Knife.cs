using UnityEngine;
using UnityEngine.Events;

public class Knife : MonoBehaviour
{ // Start is called before the first frame update

    private Rigidbody2D rb;

    private BoxCollider2D boxCollider2D;
    
    public UnityEvent onKnifeHit;

    public bool isHited;

    private int index = 0;

    private int maxIndex;
    
    private bool isKnifeHitKnife = false;

    private ParticleSystem woodParcitle;

    //[SerializeField] private List<Sprite> knifeSprites;

    private void Awake()
    {
        //boxCollider2DInWheel
        woodParcitle = GetComponent<ParticleSystem>();
        isHited = false;
        //maxIndex = knifeSprites.Count;
        
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        //GetComponentInChildren<SpriteRenderer>().sprite = knifeSprites[0];
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
            //Debug.Log("Knife");
            rb.velocity = new Vector2(rb.velocity.x, -3.0f);
            if (gameObject.transform.parent)
            {
                Debug.Break();
                //gameObject.GetComponentInParent<Rotation>()?.EnableRestartButton();
                gameObject.GetComponentInParent<Rotation>()?.OnLoseGame();
                Vibration.Vibrate();
            }
            else
            {
                boxCollider2D.isTrigger = true;
                //Debug.Break();
            }

            
            //rb.gameObject.SetActive(false);
            //rb.AddForce(transform.up.normalized*-1.0f*15);
        }
        if (col.gameObject.CompareTag("Wheel")&& !isKnifeHitKnife)
        {
            //Debug.Log("Wheel");
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(col.collider.transform);
            boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, -0.69f);
            boxCollider2D.size = new Vector2(boxCollider2D.size.x, 1.15f);
            //Debug.Log(Vector2.Distance(transform.position,col.collider.transform.position));
            //boxCollider2D = col.gameObject.GetComponent<Rotation>().boxCollider2DInWheel;
            //Vibration.Vibrate();
            woodParcitle.Play();
            onKnifeHit?.Invoke();
        }
    }
}
