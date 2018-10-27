using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Получает список видеофайлов через плагин.
/// Переписывает в titleList и pathList.
/// Делает проверку на доступ к внешней памяти
/// </summary>
public class MonitorDraw : MonoBehaviour {
    //Объекты для рисования:
    public GameObject Monitor;          //канвас, относительно которых создаются панели

    public GameObject Panel;            //префаб панели, относительно которой создаются texts
    public GameObject[] Panels;         //массив из панелей
    int panelsCount;                    //количество панелей

    public Text text;                   //префаб текста
    public Text[] texts;                //массив из текстов
    public TextObject[] textObjects;    //массив из характеристик текстов

    //Элементы управления:
    public Button nextButton;           //кнопка далее    
    int nextBtn = 0;

    //Массивы, которые содеражат название фильма и путь к нему
    string[] titleList;
    string[] pathList;
    string[] durationList;
    int[] durationListInt;

    //Флаги
    /// <summary>
    /// Есть ли доступ к файлам
    /// </summary>
    public bool allowed;
    /// <summary>
    /// Нарисован ли список на мониторе
    /// </summary>
    public bool isListDrawn;
    /// <summary>
    /// Готов ли список к рисованию
    /// </summary>
    public bool isListPrepared;

    //Количество строк на мониторе
    public int ScreenCount=7;

    //Объект для работы с плагином
    AndroidJavaObject GetVideoList;


    private void Start()
    {
        StartCoroutine(ReiceveList());
    }

    /// <summary>
    /// Постоянно проверяет доступ к файлам
    /// </summary>
    private void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GetVideoList.Call("UpdateCheckPerm");
        allowed = GetVideoList.Get<bool>("allowed");
#endif
#if UNITY_EDITOR
        allowed = true;
#endif
        if (allowed) Draw();
    }

    /// <summary>
    /// Получает список файлов. Доступно только в Андроид
    /// </summary>
#if UNITY_ANDROID && !UNITY_EDITOR
    private void Awake()
    {
        allowed = false;
        GetVideoList = new AndroidJavaObject("com.hennessy.GetFileListPlugin.GetVideoList");
    }
    IEnumerator ReiceveList()
    {
        while (true)
        {
            if (!isListPrepared && allowed)
            {
                GetVideoList.Call("GetVideoFiles");
                titleList = new string[GetVideoList.Get<int>("titleListSize")];
                pathList = new string[GetVideoList.Get<int>("pathListSize")];
                durationListInt = new int[GetVideoList.Get<int>("durationListSize")];
                titleList = GetVideoList.Get<string[]>("titleList");
                pathList = GetVideoList.Get<string[]>("pathList");
                durationListInt = GetVideoList.Get<int[]>("durationList");
                isListPrepared = true;
            }
        yield return null;
        }
    }
#endif

    /// <summary>
    /// Получает список файлов. Доступно только в редакторе
    /// </summary>
#if UNITY_EDITOR
    IEnumerator ReiceveList()
    {
        while (true)
        {
            if (!isListPrepared && allowed)
            {
                titleList = new string[19];
                pathList = new string[19];
                durationListInt = new int[19];
                for (int i = 0; i < titleList.Length; i++)
                {
                    titleList[i] = (i + 1) + ". Название of film";
                    pathList[i] = "Путь к фильму " + (i + 1);
                    durationListInt[i] = 4000 * i;
                    yield return null;
                    Debug.Log(i);
                }
                isListPrepared = true;
            }
            yield return null;
        }
    }
#endif

    /// <summary>
    /// Принимает массив и рисует его на мониторе
    /// </summary>
    public void Draw()
    {
        if (!isListDrawn && isListPrepared)
        {
            durationList = new string[durationListInt.Length];
            for (int i = 0; i <= durationListInt.Length-1; i++)
            {
                if (durationListInt[i] != 0)
                {
                    System.TimeSpan time = System.TimeSpan.FromMilliseconds(durationListInt[i]);
                    time = new System.TimeSpan(time.Hours, time.Minutes, time.Seconds);
                    durationList[i] = time.ToString();
                }
                else durationList[i] = null;
            }

            //Создает массив из панелей:
            panelsCount = (int)Mathf.Floor(titleList.Length / ScreenCount) + 1;
            Panels = new GameObject[panelsCount];
            if (panelsCount > 1) nextButton.enabled = true; //если панелей больше 1, то появляется кнопка
            //Создает массив из текстов и характеристик текстов:
            texts = new Text[titleList.Length];
            textObjects = new TextObject[titleList.Length];
            //Создает панели
            for (int i = 0; i < panelsCount; i++)
            {
                Panels[i] = Instantiate(Panel, Monitor.transform);
                Panels[i].SetActive(false);
            }
            //Рисует текста в панели
            for (int i = 0; i < titleList.Length; i++)
            {
                //texts[i] = Instantiate(text, Panels[(int)Mathf.Floor(i / ScreenCount)].transform);
                //texts[i].transform.position = new Vector3(Panels[0].transform.position.x+1000f, (Panels[0].transform.position.y) - (i / 11f), Panels[0].transform.position.z);
                texts[i].text = titleList[i]+"   "+durationList[i];
                textObjects[i] = texts[i].gameObject.GetComponent<TextObject>();
                textObjects[i].title = titleList[i];
                textObjects[i].path = pathList[i];
            }
            //Корректирует расположение панелей
            for (int i = 0; i < panelsCount; i++)
            {
                Panels[i].transform.position = new Vector3(Monitor.transform.position.x, (Monitor.transform.position.y + (0.930f * i)), Monitor.transform.position.z);
            }
            Panels[0].SetActive(true);
            isListDrawn = true;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        else
        {
            GetVideoList.Call("CheckPerm");
        }
#endif
    }

    /// <summary>
    /// Перелистывает список
    /// </summary>
    public void Next()
    {
        if (panelsCount != 1)
        {
            if (nextBtn == (panelsCount - 1))
            {
                nextBtn = 0;
                Panels[0].SetActive(true);
                Panels[panelsCount - 1].SetActive(false);
            }
            else
            {
                Panels[nextBtn + 1].SetActive(true);
                Panels[nextBtn].SetActive(false);
                nextBtn++;
            }
        }
    }
}
