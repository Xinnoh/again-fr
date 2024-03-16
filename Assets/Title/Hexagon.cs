using System.Collections;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public bool isPulsing = false;
    public Hexagon leftNeighbor, rightNeighbor;
    private SpriteRenderer sprite;

    // Flags to control pulse direction
    public bool pulseLeft, pulseRight;


    // Cooldown mechanism
    private float lastPulseTime = -2.0f; // Initialize to allow immediate pulsing
    private float pulseCooldown = 2.0f; // 2 seconds cooldown

    public bool SignalLeft, SignalRight;

    [SerializeField] private float minOpacity = .2f;

    [SerializeField] private float midOpacity = .5f;
    [SerializeField] private float maxOpacity = .8f;
    [SerializeField] private float fadeOutRate = .02f;
    [SerializeField] private float fadeInRate = 1f;

    [SerializeField] private float transferDelay = .5f;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, minOpacity);
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
    }

    public void TriggerPulse(bool fromLeft)
    {
        // Check if enough time has passed since the last pulse
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
        yield return new WaitForSeconds(0.2f); 
        StartPulse();
        lastPulseTime = Time.time; // Update the time of the last pulse

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

        yield return new WaitForSeconds(transferDelay); // Duration of the pulse
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
