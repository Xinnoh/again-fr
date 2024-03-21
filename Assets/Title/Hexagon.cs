using System.Collections;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public bool isPulsing = false;
    public Hexagon leftNeighbor, rightNeighbor;
    private SpriteRenderer sprite;

    [HideInInspector] public bool pulseLeft, pulseRight;

    private float lastPulseTime = -2.0f; 
    private float pulseCooldown = 2.0f;

    public bool SignalLeft, SignalRight;

    [SerializeField] private float minOpacity = .2f;

    [SerializeField] private float midOpacity = .5f;
    [SerializeField] private float maxOpacity = .8f;
    [SerializeField] private float fadeOutRate = .02f;
    [SerializeField] private float fadeInRate = 1f;

    [SerializeField] private float pulseLength = .5f;
    [SerializeField] private float transferDelay = .05f;

    private bool canPulse;
    public int sidePulseChance = 70;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, minOpacity);

        StartCoroutine(CreatePulseDelay());
    }

    private void Update()
    {
        if(SignalLeft)
        {
            TriggerPulse(false);
            SignalLeft = false;
        }
        if(SignalRight)
        {
            TriggerPulse(true);
            SignalRight = false;
        }

        if (sprite.color.a > minOpacity)
        {
            float newOpacity = Mathf.Max(sprite.color.a - fadeOutRate * Time.deltaTime, minOpacity);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newOpacity);
        }

        if(leftNeighbor == null || rightNeighbor == null)
        {
            RandomPulseChance();
        }

    }


    private void RandomPulseChance()
    {
        if (!canPulse)
        {
            return;
        }

        if(Random.Range(0, sidePulseChance) < 2)
        {
            if(leftNeighbor == null)
            {
                SignalRight = true;
                SignalLeft= true;
            }
            else
            {
                SignalRight = true;
                SignalLeft = true;
            }

        }

        StartCoroutine(CreatePulseDelay());
        
    }

    IEnumerator CreatePulseDelay()
    {
        canPulse = false;

        yield return new WaitForSeconds(Random.Range(.1f, 6f));

        canPulse = true;
    }


    public void TriggerPulse(bool fromLeft)
    {
        if (Time.time - lastPulseTime >= pulseCooldown)
        {
            if (fromLeft && !pulseRight)
            {
                StartCoroutine(DelayedPulse(true));
            }
            else if (!fromLeft && !pulseLeft)
            {
                StartCoroutine(DelayedPulse(false));
            }
        }
    }

    IEnumerator DelayedPulse(bool pulseDirectionRight)
    {

        StartCoroutine(FadeToMaxOpacity());
        yield return new WaitForSeconds(transferDelay); 
        StartPulse();
        lastPulseTime = Time.time; 

        if (pulseDirectionRight)
        {
            pulseRight = true;
            if (rightNeighbor != null) rightNeighbor.TriggerPulse(true);
        }
        else
        {
            pulseLeft = true;
            if (leftNeighbor != null) leftNeighbor.TriggerPulse(false);
        }

        yield return new WaitForSeconds(pulseLength); 
        StopPulse();
    }

    IEnumerator FadeToMaxOpacity()
    {
        isPulsing = true;
        while (sprite.color.a < maxOpacity)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Min(sprite.color.a + fadeInRate * Time.deltaTime, 1));
            yield return null;
        }
    }

    IEnumerator FadeToMidOpacity()
    {
        isPulsing = true;
        while (sprite.color.a < midOpacity)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Min(sprite.color.a + fadeInRate * Time.deltaTime, 1));
            yield return null;
        }
    }

    private void OnMouseEnter()
    {

        StartCoroutine(FadeToMidOpacity());
    }

    private void StartPulse()
    {
        if (!isPulsing)
        {
            //sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, maxOpacity);
            isPulsing = true;
        }
    }

    private void StopPulse()
    {
        isPulsing = false;
        pulseLeft = false;
        pulseRight = false;
    }
}
