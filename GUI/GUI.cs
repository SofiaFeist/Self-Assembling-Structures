﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUI : MonoBehaviour
{
    _3DSelfAssembly SA;

    public Button reconfigurationButton;
    public Slider reconfigurationSpeed;
    public Button resetButton;

    public static bool Paused;


    
    void Start()
    {
        SA = FindObjectOfType(typeof(_3DSelfAssembly)) as _3DSelfAssembly;   // Fancy way to do the same
        //SA = FindObjectOfType<_3DSelfAssembly>();



        // GUI ELEMENTS
        // Start Reconfiguration Button
        reconfigurationButton.GetComponent<Button>();
        reconfigurationButton.onClick.AddListener(StartReconfiguration);


        // Slider
        reconfigurationSpeed.GetComponents<Slider>();
        reconfigurationSpeed.minValue = _3DSelfAssembly.minSpeed;
        reconfigurationSpeed.maxValue = _3DSelfAssembly.maxSpeed;


        // Reset Button
        resetButton.GetComponent<Button>();
        resetButton.onClick.AddListener(reset);
    }

    void Update()
    {
        // Update Slider value
        SA.currentSpeed = reconfigurationSpeed.value;
    }
    

    void StartReconfiguration()
    {
        Paused = false;
        StartCoroutine(SA.SelfAssembly());
        //StartCoroutine(SA.RandomReconfiguration());

        reconfigurationButton.onClick.AddListener(Pause);
        GameObject.Find("ReconfigurationStart").GetComponentInChildren<Text>().text = "PAUSE";

        reconfigurationButton.onClick.RemoveListener(StartReconfiguration);
    }


    void Pause()
    {
        if (Paused == true)
        {
            Paused = false;
            GameObject.Find("ReconfigurationStart").GetComponentInChildren<Text>().text = "PAUSE";
        }
        else
        {
            Paused = true;
            GameObject.Find("ReconfigurationStart").GetComponentInChildren<Text>().text = "RESUME";
        }
    }


    void reset()
    {
        Vector3 saveCameraPosition = Camera.main.transform.position;
        Debug.Log(Camera.main.transform.position);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log(Camera.main.transform.position);
        Camera.main.transform.position = saveCameraPosition;   // KEEP CAMERA POSITION AND ROTATION
        Debug.Log(Camera.main.transform.position);
    }
}
