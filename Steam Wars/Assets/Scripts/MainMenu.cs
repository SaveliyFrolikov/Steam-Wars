using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform[] waypoints;
    public GameObject orbiter;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Dropdown difficulty;

    [Space]

    public GameObject aboutMenu;
    public GameObject settingsMenu;
    public GameObject instructionMenu;

    [Space]

    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;

    [Space]

    public AudioClip sfx;
    public AudioSource musicSource;

    AudioSource sfxSource;

    public static MainMenu Instance;

    bool settings = false;
    bool about = false;
    bool instructions = false;
    int targetIndex;
    Vector3 currentWaypoint;

    void Start()
    {
        Instance = this;
        sfxSource = GetComponent<AudioSource>();
        currentWaypoint = waypoints[0].position;
    }

    
    void Update()
    {
        orbiter.transform.LookAt(transform.position);
        musicText.text = Mathf.Round(musicSlider.value) + "%";
        sfxText.text = Mathf.Round(sfxSlider.value) + "%";
        musicSource.volume = Mathf.Round(musicSlider.value) / 100;
        sfxSource.volume = Mathf.Round(sfxSlider.value) / 100;
        PlayerPrefs.SetFloat("sfxVolume", Mathf.Round(musicSlider.value));
        PlayerPrefs.SetFloat("musicVolume", Mathf.Round(sfxSlider.value));
        OrbitSpider();
    }

    public void Play()
    {
        sfxSource.Play();
        PlayerPrefs.SetInt("difficulty", difficulty.value);

        SceneManager.LoadScene(1);
    }

    public void About()
    {
        sfxSource.Play();
        about = !about;
        aboutMenu.GetComponent<Animator>().SetBool("about", about);
    }

    void OrbitSpider()
    {
        if (orbiter.transform.position == currentWaypoint)
        {
            targetIndex++;
            if (targetIndex >= waypoints.Length)
            {
                //orbiter.transform.position = waypoints[0].position;
                targetIndex = 0;
            }
            currentWaypoint = waypoints[targetIndex].position;
        }
        else
        {
            orbiter.transform.position = Vector3.MoveTowards(orbiter.transform.position, currentWaypoint, 10 * Time.deltaTime);
        }
    }

    public void Settings()
    {
        sfxSource.Play();
        settings = !settings;
        settingsMenu.GetComponent<Animator>().SetBool("settings", settings);
    }

    public void Instructions()
    {
        sfxSource.Play();
        instructions = !instructions;
        instructionMenu.GetComponent<Animator>().SetBool("instruction", instructions);
    }
}
