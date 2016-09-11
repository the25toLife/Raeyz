using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashFade : MonoBehaviour {

    public Image SplashImage;
    public Text SplashText;
    public string LoadLevel;

    IEnumerator Start()
    {
        SplashImage.canvasRenderer.SetAlpha(0.0f);
        SplashText.canvasRenderer.SetAlpha(0.0f);

        FadeIn();
        yield return new WaitForSeconds(3.0f);
        FadeOut();
        yield return new WaitForSeconds(3.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(LoadLevel);
    }

    void FadeIn()
    {
        SplashImage.CrossFadeAlpha(1.0f, 1.5f, false);
        SplashText.CrossFadeAlpha(1.0f, 1.5f, false);
    }

    void FadeOut()
    {
        SplashImage.CrossFadeAlpha(0.0f, 2.5f, false);
        SplashText.CrossFadeAlpha(0.0f, 2.5f, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UnityEngine.SceneManagement.SceneManager.LoadScene(LoadLevel);
    }
}
