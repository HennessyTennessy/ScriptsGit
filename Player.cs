using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    
    private float mouseX, mouseY;
    public Transform camera;
    public int damage;
    public Text text;
    
    GameManager GM;

    private void Start()
    {
        GameObject go = GameObject.Find("GameManager");
        GM = go.GetComponent<GameManager>();
        StartCoroutine(StartInfo());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator StartInfo()
    {
        try
        {
            text.text = "Двигайтесь к зеленому кругу! Быстрее!";
            
        }
        catch (Exception e)
        {
            Debug.Log("Добавьте Text на Canvas");
        }
        yield return new WaitForSeconds(3);
        try
        {
            text.text = null;

        }
        catch (Exception e)
        {
            Debug.Log("Добавьте Text на Canvas");
        }
    }

    void Update()
    {
        //Управление WSAD
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * GM.speedPlayer;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * GM.speedPlayer;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime * GM.speedPlayer;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * GM.speedPlayer;
        }

        //Режимы управления камерой мышь/стрелки
        if (GM.isMouseController)
        {
            mouseX += GM.sensivity * Input.GetAxis("Mouse X");
            mouseY -= GM.sensivity * Input.GetAxis("Mouse Y");

            camera.transform.eulerAngles = new Vector3(mouseY, camera.transform.eulerAngles.y, 0.0f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, mouseX, 0.0f);
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camera.transform.Rotate(-100f * Time.deltaTime * GM.sensivity, 0, 0);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                camera.transform.Rotate(100f * Time.deltaTime * GM.sensivity, 0, 0);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(0, -100f * Time.deltaTime * GM.sensivity, 0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(0, 100f * Time.deltaTime * GM.sensivity, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            Time.timeScale = 0;
            Debug.Log("Уровень пройден!");
            try
            {
                text.text = "Уровень пройден!";
            }
            catch (Exception e)
            {
                Debug.Log("Добавьте Text на Canvas");
            }
        }
    }
}
