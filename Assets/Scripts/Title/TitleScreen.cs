using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private AudioSource audioSource;
    public SpriteRenderer titleCover;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        titleCover.color = new Color (titleCover.color.r, titleCover.color.g, titleCover.color.b, 0f);
    }

    public void StartPressed()
    {
        StartCoroutine(FadeOutAudioSource(audioSource, audioSource.volume / 2, 2f));
        StartCoroutine(FadeSpriteRenderer(titleCover, 2f)); 
        StartCoroutine(LoadSceneAfterDelay("Dungeon", 2f)); 

    }

    public void OptionsPressed()
    {
        StartCoroutine(FadeOutAudioSource(audioSource, audioSource.volume / 2, 0.5f)); 
    }

    public void ExitPressed()
    {
        StartCoroutine(FadeOutAudioSource(audioSource, 0f, 0.5f)); 
    }

    private IEnumerator FadeOutAudioSource(AudioSource source, float targetVolume, float duration)
    {
        float currentTime = 0;
        float start = source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        source.volume = targetVolume; // Ensure the target volume is set after the loop
    }


    private IEnumerator FadeSpriteRenderer(SpriteRenderer spriteRenderer, float duration)
    {
        float time = 0f;
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Ensure alpha is 1 for full opacity

        while (time < duration)
        {
            time += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, endColor, time / duration);
            yield return null;
        }
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
