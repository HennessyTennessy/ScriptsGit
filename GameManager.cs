using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Количество летучих мышей")]
    [Tooltip("Количество летучих мышей")]
    public int N;
    [Header("Настройка точки спавна")]
    [Tooltip("Минимальная высота спавна")]
    public int H1;
    [Tooltip("Максимальная высота спавна")]
    public int H2;
    [Tooltip("Минимальное расстояние от центра до спавна")]
    public int S1;
    [Tooltip("Максимальное расстояние от центра до спавна")]
    public int S2;
    [Tooltip("Минимальное время на подготовку к атаке")]
    public int T1;
    [Tooltip("Максимальное время на подготовку к атаке")]
    public int T2;
    [Space]
    [Header("Остальные настройки")]
    [Tooltip("Чувствительность мыши")]
    public int sensivity;
    [Tooltip("Скорость игрока")]
    public int speedPlayer;
    [Tooltip("Скорость летучих мышей")]
    public int speedBat;
    [Tooltip("Управление мышью")]
    public bool isMouseController;
    [Tooltip("Нарисовать кривую полета")]
    public bool isPathDrawing;
    [Space]
    [Header("Виды мышей")]
    public GameObject[] Bats;    

    private void Start()
    {
        for (int i = 0; i < N; i++)
        {
            Instantiate(Bats[UnityEngine.Random.Range(0, (Bats.Length))], transform.position, Quaternion.identity);
        }
    }
}
