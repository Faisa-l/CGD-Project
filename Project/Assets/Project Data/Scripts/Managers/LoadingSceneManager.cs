using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    LoadingVariables loadingVars;
    AsyncOperation asyncOp;

    [SerializeField] Slider slider;
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject startPrompt;

    [SerializeField] float minTime = 1f;
    [SerializeField] float maxTime = 3f;
    float totalTime = 0f;
    float time = 0f;
    void Start()
    {
        totalTime = Random.Range(minTime, maxTime);
        time = 0f;

        startPrompt.SetActive(false);
        loadingVars = FindFirstObjectByType<LoadingVariables>();
        loadingVars.allowVariableDestruction();

        //Real scene loading
        StartCoroutine(loadNextSceneAsync());

        //Fake loading
        StartCoroutine(updateUI());
    }

    private void Update()
    {
        //if the slider shows 100%
        if(slider.value == slider.maxValue)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                asyncOp.allowSceneActivation = true;
            }
        }
    }

    private IEnumerator updateUI()
    {
        //while the slider hasn't reached 100% and the level loading is't finished, update the loading screen
        while (slider.value / slider.maxValue < 1f && asyncOp.progress < 0.9)
        {
            float delta = Random.Range(0f, 1f);

            if(asyncOp.progress < 0.9)
            {
                totalTime += delta/10f;
            }

            time += delta;
            slider.value = time/totalTime;
            text.text = $"{(int)((slider.value / slider.maxValue) * 100f)}%";

            yield return new WaitForSecondsRealtime(delta);
        }

        //when all of the loading is done, turn off the text and turn on the start prompt
        text.gameObject.SetActive(false);
        startPrompt.SetActive(true);

        yield return null;
    }

    private IEnumerator loadNextSceneAsync()
    { 
        //actually loads the next scene
        asyncOp = SceneManager.LoadSceneAsync(loadingVars.sceneName);
        asyncOp.allowSceneActivation = false;
        asyncOp.priority = 19;

        yield return asyncOp;
    }
}
