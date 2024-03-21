using Edgar.Unity.Examples.Gungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private AudioSource audioSource;
    public SpriteRenderer titleCover;

    [SerializeField] private float fadeDuration = 1f;

    [SerializeField] private Image cover;
    private bool tutorialMode = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cover.color = new Color (cover.color.r, cover.color.g, cover.color.b, 0f); 
    }

    public void StartPressed()
    {
        tutorialMode = true;
        StartCoroutines();
    }

    public void TutorialSkipped()
    {
        tutorialMode = false;
        StartCoroutines();
    }

    private void StartCoroutines()
    {
        StartCoroutine(LoadAsyncScene());
        StartCoroutine(FadeOutAudioSource(audioSource, audioSource.volume / 2, fadeDuration));
        StartCoroutine(FadeSpriteRenderer(titleCover, fadeDuration));
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
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f); 

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


    IEnumerator LoadAsyncScene()
    {
        yield return StartCoroutine(FadeToBlack());


        if (tutorialMode)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Tutorial");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Dungeon");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

        }
    }

    private IEnumerator FadeToBlack()
    {
        float currentTime = 0f;
        Color color = cover.color;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, currentTime / fadeDuration);
            cover.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }
}
