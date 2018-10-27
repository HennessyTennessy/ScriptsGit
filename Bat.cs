using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bat : MonoBehaviour {

    [Header("Уникальная скорость летучей мыши")]
    [Tooltip("Уникальная скорость летучей мыши")]
    public int uniqspeed = 0;

    float S, HS, rndAngle, _x, _y, _z;
    GameManager GM;
    
    public GameObject target, curvWP, WP;
    GameObject _player, _target, _curvWP;
    
    private int currentWP = 0;
    private Vector3 currentTarget;
    private Vector3 homePos;
    private Vector3[] waypoints;
    public int numWP=30;
    
    public bool isAttack = false;
    public bool isPrepare = true;

    Player player;
    GameObject canvas;
    Image imagecanvas;

    Animator anim;
    AudioSource audiosource;
    public AudioClip attackSound;

    private void Start()
    {
        GameObject go = GameObject.Find("GameManager");
        GM = go.GetComponent<GameManager>();

        _player = GameObject.Find("Player");
        player = _player.GetComponent<Player>();
        
        transform.position = Spawn(player.transform.position);

        waypoints = new Vector3[numWP];
        StartCoroutine(Prepare());
        
        canvas = GameObject.Find("Canvas");
        imagecanvas = canvas.GetComponent<Image>();
        audiosource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        imagecanvas.color = Color.Lerp(imagecanvas.color, Color.clear, Time.deltaTime);
        if (!isPrepare)
        {
            if (isAttack)
            {
                Attack();
            }
            else
            {
                if (transform.position == waypoints[currentWP] && currentWP < numWP - 1)
                {
                    currentWP++;
                    currentTarget = waypoints[currentWP];
                }
                else
                {
                    Fly();
                }
            }
        }
    }

    /// <summary>
    /// Случайная точка
    /// </summary>
    /// <param name="player">Позиция игрока</param>
    /// <returns></returns>
    Vector3 Spawn(Vector3 player)
    {
        if (GM.S2 < GM.H2) Debug.LogWarning("Максимум высоты (H2) не может быть больше максимума расстояния (S2) до игрока!!!");
        _y = UnityEngine.Random.Range(GM.H1, GM.H2);  //получаем высоту
        S = UnityEngine.Random.Range(GM.S1, GM.S2);   //получаем расстояние
        if (_y > S)
        {
            Debug.LogWarning("высота больше расстояния");
            S = _y;
        }
        HS = Mathf.Sqrt(S * S - _y * _y);
        rndAngle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        _x = HS * Mathf.Cos(rndAngle);  //ищем х
        _z = HS * Mathf.Sin(rndAngle);  //ищем y

        return player + new Vector3(_x, _y, _z);
    }

    /// <summary>
    /// Режим атаки
    /// </summary>
    void Attack()
    {
        currentTarget = _player.transform.position;
        transform.LookAt(currentTarget);
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, Time.deltaTime * GM.speedBat*uniqspeed);
    }
    /// <summary>
    /// Режим полета
    /// </summary>
    void Fly()
    {
        transform.LookAt(currentTarget);
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, Time.deltaTime * GM.speedBat * uniqspeed);
    }
    /// <summary>
    /// Режим ожидания
    /// </summary>
    private IEnumerator Prepare()
    {
        isPrepare = true;
        currentTarget = _player.transform.position;
        transform.LookAt(currentTarget);
        yield return new WaitForSeconds(UnityEngine.Random.Range(GM.T1,GM.T2));
        isPrepare = false;
        isAttack = true;
    }

    /// <summary>
    /// Контроль режимов
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //если достиг область вокруг игрока
        if (other.tag == "PlayerArea")
        {
            homePos = GM.S2 * (transform.position - player.transform.position); //запоминает местоположение мыши перед ударом
            Debug.Log("Вектор отчета " + homePos);
            isAttack = false;
            CreatePath();
            currentWP = 0;
            currentTarget = waypoints[currentWP];
        }
        //если достиг игрока
        if (other.tag == "Player")
        {
            audiosource.PlayOneShot(attackSound);
            anim.Play("attack1");
            player.damage++;
            Debug.Log("Урон игроку  " + player.damage);
            imagecanvas.color = Color.red;
        }
        //если достиг конца пути
        if (other.gameObject == _target)
        {
            StartCoroutine(Prepare());
            Destroy(_target);
            Destroy(_curvWP);
            currentWP = 0;
            currentTarget = _player.transform.position;
        }
    }

    /// <summary>
    /// Создает путь для летучей мыши
    /// </summary>
    void CreatePath()
    {
        _curvWP = Instantiate(curvWP, player.transform.position + new Vector3 ((-1) * homePos.x, -2, (-1) * homePos.z), Quaternion.identity);
        _target = Instantiate(target, Spawn(player.transform.position), Quaternion.identity);
        waypoints = new Vector3[numWP];
        DrawPath(transform.position, _curvWP.transform.position, _target.transform.position);
    }
    /// <summary>
    /// Рисует путь для летучей мыши
    /// </summary>
    void DrawPath(Vector3 start, Vector3 curv, Vector3 target)
    {
        for (int i = 1; i <= numWP; i++)
        {
            float t = i / (float)numWP;
            waypoints[i - 1] = CalculateQuadBezierPoint(t, start, curv, target);
            if (GM.isPathDrawing) Instantiate(WP, waypoints[i - 1], Quaternion.identity);
        }
    }
    /// <summary>
    /// Вычисляет кривую для пути летучей мыши
    /// </summary>
    Vector3 CalculateQuadBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
