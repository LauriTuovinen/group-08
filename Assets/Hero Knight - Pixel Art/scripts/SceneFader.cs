using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private float fadeTime;
    private Image fadeoutUIImage;

    public enum FadeDirection
    {
        In,
        Out
    }
    private void Awake()
    {
        fadeoutUIImage = GetComponent<Image>();
    }

    public IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = fadeDirection == FadeDirection.Out ? 1 : 0;
        float fadeEndValue = fadeDirection == FadeDirection.Out ? 0 : 1;

        if(fadeDirection == FadeDirection.Out)
        {
            while(alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);

                yield return null;
            }
            fadeoutUIImage.enabled = false;
        }

        else
        {
            fadeoutUIImage.enabled = true;
            
            while(alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);

                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string levelToLoad)
    {
        fadeoutUIImage.enabled = true;

        yield return Fade(fadeDirection);
    }

    void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        fadeoutUIImage.color = new Color(fadeoutUIImage.color.r, fadeoutUIImage.color.g, fadeoutUIImage.color.b, alpha);
        alpha += Time.deltaTime * (1 / fadeTime) * (fadeDirection == FadeDirection.Out ? -1 : 1);
    }
}
