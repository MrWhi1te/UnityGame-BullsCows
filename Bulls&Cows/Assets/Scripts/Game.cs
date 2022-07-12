using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Game : MonoBehaviour
{
    private System.Random rand = new System.Random(); // Переменная для генерации случайных чисел
    private int[] inventNumber = new int[4]; // Массив загаданного числа
    private string inventNumberString; // Строковая переменная для чисел
    private string dropdownsNumber; //
    private int fullCoincidence; // Полное совпадение чисел
    private int partlyCoincidence; // Частичное совпадение чисел

    private int attempt; // Кол-во попыток
    private int stars; // Кол-во звезд
    private int level = 1; // Уровень игрока
    private float scores; // Кол-во очков

    public Text attemptText; // Текст попыток
    public Text pastNumber; // Текст прошлых введеных чисел
    public Text levelText; // Уровень текст
    public Text starsText; // Звезды текст
    public Text notStars; // Не хватает звезд.
    public Text notStars1;

    public Dropdown[] dropdowns; // Выпадающие списки
    public Image levelImage;

    public GameObject menuPan; // Меню панель
    public GameObject endAttemptPan; // Панель при окончании попыток
    public GameObject AboutGamePan;
    public GameObject recordPan;
    public GameObject howGamePan;
    public Button check; // Кнопка "проверить"

    [Header("Records")]
    public Text allAttemptText; //
    public Text allHintText; //
    public Text allEarnedStarsText; //
    public Text allSpentStarsText; //
    public Text bestRecords; //
    public int allAttempt; //
    public int allHint; //
    public int allEarnedStars; //
    public int allSpentStars; //
    public int attemptCount; //
    public string recordsText; //
    public int recordsAttempt = 100; //

    private Save sv = new Save();

    private void Awake()
    {
        if (PlayerPrefs.HasKey("sv"))
        {
            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("sv"));
            stars = sv.stars;
            level = sv.level;
            scores = sv.scores;
            allAttempt = sv.allAttempt;
            allHint = sv.allHint;
            allEarnedStars = sv.allEarnedStars;
            allSpentStars = sv.allSpentStars;
            recordsAttempt = sv.recordsAttempt;
            recordsText = sv.recordsText;
            levelImage.fillAmount = scores / (5 * level);
            starsText.text = stars.ToString();
            levelText.text = "Уровень: " + level;
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{
    //    levelText.text = "Уровень: " + level;
    //}

    public void NewGame() // Запуск новой игры
    {
        dropdowns[0].value = 0;
        dropdowns[1].value = 0;
        dropdowns[2].value = 0;
        dropdowns[3].value = 0;
        attempt = 10;
        attemptCount = 0;
        attemptText.text = "Осталось попыток: " + attempt.ToString();
        pastNumber.text = "";
        menuPan.SetActive(false);
        NewNumber();
        endAttemptPan.SetActive(false);
    }

    private void NewNumber() // Создание нового числа (Рандом)
    {
        bool contains; //
        for (int i = 0; i < 4; i++) // цикл заполнения массива нового числа новыми цифрами
        {
            do
            {
                contains = false;
                inventNumber[i] = rand.Next(10); // создание новой цифры
                for (int a = 0; a < i; a++) // цикл сравнения сгенерированной цифры с предыдущими
                {
                    if (inventNumber[a] == inventNumber[i])
                    {
                        contains = true;
                    }
                }
            } while (contains);
        }
        inventNumberString = inventNumber[0].ToString() + inventNumber[1] + inventNumber[2] + inventNumber[3]; // из элементов делаем строку
    }

    public void CheckNumber() // Проверка числа
    {
        ComparingNumbers();
        Result();
    }

    private void ComparingNumbers() // Сравнение введенных и загаданного числа
    {
        fullCoincidence = 0;
        partlyCoincidence = 0;
        dropdownsNumber = dropdowns[0].value.ToString() + dropdowns[1].value.ToString() + dropdowns[2].value.ToString() + dropdowns[3].value.ToString();
        char[] ch = dropdownsNumber.ToCharArray();
        for (int i = 0; i < 4; i++)
        {
            if (inventNumberString.Contains(ch[i]))
            {
                if (inventNumberString[i] == ch[i]) fullCoincidence++;
                else partlyCoincidence++;
            }
        }
    }
    private void Result() // Вывод результата
    {
        if (fullCoincidence >= 4) // Выигрыш, если отгаданы все.
        {
            check.interactable = false;
            attemptText.text = "";
            scores++;
            allAttempt++;
            stars++;
            allEarnedStars++;
            LevelUpdate();
            StartCoroutine(AutoRestart());
        }
        else //  
        {
            attempt--;
            allAttempt++;
            attemptCount++;
            if (attempt <= 0)
            {
                endAttemptPan.SetActive(true);
            }
            else
            {
                string nameBull = " Быков и ";
                string nameCow = " Коров";
                if (fullCoincidence == 1) nameBull = " Бык и ";
                if (fullCoincidence >= 2) nameBull = " Быка и ";
                if (partlyCoincidence == 1) nameCow = " Корова";
                if (partlyCoincidence >= 2) nameCow = " Коровы";
                attemptText.text = "Осталось попыток: " + attempt;
                pastNumber.text += dropdownsNumber + " - " + fullCoincidence + nameBull + partlyCoincidence + nameCow + "\n";
            }
        }
    }

    private void LevelUpdate() // Повышение уровня
    {
        if (scores / (5 * level) >= 1)
        {
            levelImage.fillAmount = 0;
            scores = 0;
            level++;
            levelText.text = "Уровень: " + level;
        }
        else
        {
            levelImage.fillAmount = scores / (5 * level);
        }
        starsText.text = stars.ToString();
        if (recordsAttempt > attemptCount)
        {
            recordsAttempt = attemptCount;
            recordsText = "Рекорд: число " + dropdownsNumber + " отгадано за " + recordsAttempt + " попыток";
        }
    }

    public void ExitMenu() // Выход в меню
    {
        menuPan.SetActive(true);
        endAttemptPan.SetActive(false);
    }
    public void OpenAbout() //
    {
        AboutGamePan.SetActive(true);
    }
    public void ClosedAbout() //
    {
        AboutGamePan.SetActive(false);
    }
    public void OpenRecord() //
    {
        recordPan.SetActive(true);
        allAttemptText.text = "Всего попыток: " + allAttempt;
        allHintText.text = "Всего подсказок использовано: " + allHint;
        allEarnedStarsText.text = "Всего заработано звезд: " + allEarnedStars;
        allSpentStarsText.text = "Всего потрачено звезд: " + allSpentStars;
        bestRecords.text = recordsText;
    }
    public void ClosedRecord() //
    {
        recordPan.SetActive(false);
    }
    public void OpenHowGame() //
    {
        howGamePan.SetActive(true);
    }
    public void ClosedHowGame() //
    {
        howGamePan.SetActive(false);
    }
    public void BuyAttempt() // Покупка попыток
    {
        if (stars >= 1)
        {
            stars--;
            starsText.text = stars.ToString();
            attempt++;
            endAttemptPan.SetActive(false);
        }
        else StartCoroutine(NotMoney());
    }
    IEnumerator NotMoney() // 
    {
        notStars.text = "Недостаточно звезд!" + "\n" + "(отгадывайте числа, чтобы собрать больше звезд)";
        notStars1.text = "Недостаточно звезд!" + "\n" + "(отгадывайте числа, чтобы собрать больше звезд)";
        yield return new WaitForSeconds(3f);
        notStars.text = "";
        notStars1.text = "";
        yield break;
    }
    IEnumerator AutoRestart() // Обновление числа авто
    {
        pastNumber.text = dropdownsNumber + " Вы отгадали! " + "\n" + "Оставалось попыток: " + attempt;
        yield return new WaitForSeconds(2f);
        pastNumber.text = "Продолжаем! Загадано новое число";
        yield return new WaitForSeconds(2f);
        check.interactable = true;
        NewGame();
        yield break;
    }
    public void Helped()
    {
        if (stars >= 1)
        {
            int i = Random.Range(0, 4);
            pastNumber.text += "<color=red>Подсказка: в числе точно есть цифра: </color>" + inventNumberString[i].ToString() + "\n";
            stars--;
            starsText.text = stars.ToString();
            allHint++;
        }
        else StartCoroutine(NotMoney());
    }
    public void Proverka() // тест проверка числа
    {
        print(inventNumberString);
    }
    private void OnApplicationPause(bool pause) // Сохранения.
    {
        sv.stars = stars;
        sv.level = level;
        sv.scores = scores;
        sv.allAttempt = allAttempt;
        sv.allHint = allHint;
        sv.allEarnedStars = allEarnedStars;
        sv.allSpentStars = allSpentStars;
        sv.recordsAttempt = recordsAttempt;
        sv.recordsText = recordsText;
        PlayerPrefs.SetString("sv", JsonUtility.ToJson(sv));
    }
}

[SerializeField]
public class Save
{
    public int stars;
    public int level;
    public float scores;
    public int allAttempt;
    public int allHint; 
    public int allEarnedStars; 
    public int allSpentStars;
    public int recordsAttempt;
    public string recordsText;
}
