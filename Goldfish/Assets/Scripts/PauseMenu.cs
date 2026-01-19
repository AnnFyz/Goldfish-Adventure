using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] float fadingSpeed = 2f;
    [SerializeField] GameObject status;
    [SerializeField] float backgroundSoundVolume = 0.15f;
    [SerializeField] RawImage fadeImage;
    RawImage pauseMenuiImage;
    PlayerControlls playerInput;


    private void Awake()
    {
        pauseMenuiImage = GetComponent<RawImage>();
        playerInput = new PlayerControlls();
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.PlayerInput.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        playerInput.PlayerInput.Pause.performed -= OnPause;
        playerInput.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        
        if (pauseMenu.activeSelf)
        {
            Time.timeScale = 0;
            SoundManager.Instance.GetSound("BackgroundMusic").audioSource.volume = backgroundSoundVolume * 0.25f;
            status.SetActive(false);
            StartCoroutine(FadeImage(false));
        }
        else
        {
            Time.timeScale = 1;
            SoundManager.Instance.GetSound("BackgroundMusic").audioSource.volume = backgroundSoundVolume;
            status.SetActive(true);
            StartCoroutine(FadeImage(true));
        }
     
    }

    public IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1f; i >= 0; i -= Time.unscaledDeltaTime * fadingSpeed)
            {
                // set color with i as alpha
                if (i <= 0.85f)
                    pauseMenuiImage.color = new Color(pauseMenuiImage.color.r, pauseMenuiImage.color.g, pauseMenuiImage.color.b, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1f; i += Time.unscaledDeltaTime * fadingSpeed)
            {
                // set color with i as alpha
                if (pauseMenuiImage.color.a <= 0.85f)
                    pauseMenuiImage.color = new Color(pauseMenuiImage.color.r, pauseMenuiImage.color.g, pauseMenuiImage.color.b, i);
                yield return null;
            }         
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator StartLoadingNewScene(int index)
    {
        // loop over 1 second
        for (float i = 0, j = 1f; i <= 1; i += Time.unscaledDeltaTime, j -= Time.unscaledDeltaTime)
        {
            // set color with i as alpha
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.b, fadeImage.color.g, i);
            yield return null;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }

    public void LoadNewScene(int index)
    {
        if (SoundManager.Instance != null) { Destroy(SoundManager.Instance.gameObject); }
        StartCoroutine(StartLoadingNewScene(index));
    }
}
