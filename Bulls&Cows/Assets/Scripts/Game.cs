using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Game : MonoBehaviour
{
    private System.Random rand = new System.Random(); // ���������� ��� ��������� ��������� �����
    private int[] inventNumber = new int[4]; // ������ ����������� �����
    private string inventNumberString; // ��������� ���������� ��� �����
    private string dropdownsNumber; //
    private int fullCoincidence; // ������ ���������� �����
    private int partlyCoincidence; // ��������� ���������� �����

    private int attempt; // ���-�� �������
    private int stars; // ���-�� �����
    private int level = 1; // ������� ������
    private float scores; // ���-�� �����

    public Text attemptText; // ����� �������
    public Text pastNumber; // ����� ������� �������� �����
    public Text levelText; // ������� �����
    public Text starsText; // ������ �����
    public Text notStars; // �� ������� �����.
    public Text notStars1;

    public Dropdown[] dropdowns; // ���������� ������
    public Image levelImage;

    public GameObject menuPan; // ���� ������
    public GameObject endAttemptPan; // ������ ��� ��������� �������
    public GameObject AboutGamePan;
    public GameObject recordPan;
    public GameObject howGamePan;
    public Button check; // ������ "���������"

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
            levelText.text = "�������: " + level;
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{
    //    levelText.text = "�������: " + level;
    //}

    public void NewGame() // ������ ����� ����
    {
        dropdowns[0].value = 0;
        dropdowns[1].value = 0;
        dropdowns[2].value = 0;
        dropdowns[3].value = 0;
        attempt = 10;
        attemptCount = 0;
        attemptText.text = "�������� �������: " + attempt.ToString();
        pastNumber.text = "";
        menuPan.SetActive(false);
        NewNumber();
        endAttemptPan.SetActive(false);
    }

    private void NewNumber() // �������� ������ ����� (������)
    {
        bool contains; //
        for (int i = 0; i < 4; i++) // ���� ���������� ������� ������ ����� ������ �������
        {
            do
            {
                contains = false;
                inventNumber[i] = rand.Next(10); // �������� ����� �����
                for (int a = 0; a < i; a++) // ���� ��������� ��������������� ����� � �����������
                {
                    if (inventNumber[a] == inventNumber[i])
                    {
                        contains = true;
                    }
                }
            } while (contains);
        }
        inventNumberString = inventNumber[0].ToString() + inventNumber[1] + inventNumber[2] + inventNumber[3]; // �� ��������� ������ ������
    }

    public void CheckNumber() // �������� �����
    {
        ComparingNumbers();
        Result();
    }

    private void ComparingNumbers() // ��������� ��������� � ����������� �����
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
    private void Result() // ����� ����������
    {
        if (fullCoincidence >= 4) // �������, ���� �������� ���.
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
                string nameBull = " ����� � ";
                string nameCow = " �����";
                if (fullCoincidence == 1) nameBull = " ��� � ";
                if (fullCoincidence >= 2) nameBull = " ���� � ";
                if (partlyCoincidence == 1) nameCow = " ������";
                if (partlyCoincidence >= 2) nameCow = " ������";
                attemptText.text = "�������� �������: " + attempt;
                pastNumber.text += dropdownsNumber + " - " + fullCoincidence + nameBull + partlyCoincidence + nameCow + "\n";
            }
        }
    }

    private void LevelUpdate() // ��������� ������
    {
        if (scores / (5 * level) >= 1)
        {
            levelImage.fillAmount = 0;
            scores = 0;
            level++;
            levelText.text = "�������: " + level;
        }
        else
        {
            levelImage.fillAmount = scores / (5 * level);
        }
        starsText.text = stars.ToString();
        if (recordsAttempt > attemptCount)
        {
            recordsAttempt = attemptCount;
            recordsText = "������: ����� " + dropdownsNumber + " �������� �� " + recordsAttempt + " �������";
        }
    }

    public void ExitMenu() // ����� � ����
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
        allAttemptText.text = "����� �������: " + allAttempt;
        allHintText.text = "����� ��������� ������������: " + allHint;
        allEarnedStarsText.text = "����� ���������� �����: " + allEarnedStars;
        allSpentStarsText.text = "����� ��������� �����: " + allSpentStars;
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
    public void BuyAttempt() // ������� �������
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
        notStars.text = "������������ �����!" + "\n" + "(����������� �����, ����� ������� ������ �����)";
        notStars1.text = "������������ �����!" + "\n" + "(����������� �����, ����� ������� ������ �����)";
        yield return new WaitForSeconds(3f);
        notStars.text = "";
        notStars1.text = "";
        yield break;
    }
    IEnumerator AutoRestart() // ���������� ����� ����
    {
        pastNumber.text = dropdownsNumber + " �� ��������! " + "\n" + "���������� �������: " + attempt;
        yield return new WaitForSeconds(2f);
        pastNumber.text = "����������! �������� ����� �����";
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
            pastNumber.text += "<color=red>���������: � ����� ����� ���� �����: </color>" + inventNumberString[i].ToString() + "\n";
            stars--;
            starsText.text = stars.ToString();
            allHint++;
        }
        else StartCoroutine(NotMoney());
    }
    public void Proverka() // ���� �������� �����
    {
        print(inventNumberString);
    }
    private void OnApplicationPause(bool pause) // ����������.
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
