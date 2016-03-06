using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class PrompterWindowScript : MonoBehaviour
{

    public Button leftButton;
    public Button rightButton;
    public Text questionText;

    public delegate void LeftButtonDelegate ();
    public delegate void RightButtonDelegate ();

    private static LeftButtonDelegate leftDel;
    private static RightButtonDelegate rightDel;

    private CanvasGroup promptCanvasGroup;

    public static PrompterWindowScript instance { get; set; }

    void Start ()
    {
        instance = this;
        promptCanvasGroup = GetComponent<CanvasGroup>();

        MakeInvisable();
    }

    public void leftButtonClick() {
        leftDel();
        MakeInvisable();
    }

    public void rightButtonClick() {

        print("PrompterWindowScript rightButtonClick()");

        rightDel();
        MakeInvisable();
    }

    public void MakeInvisable() {
        promptCanvasGroup.alpha = 0f;
    }

    public static void Show(string promptText, string leftText, LeftButtonDelegate leftDeligate, 
        string rightText, RightButtonDelegate rightDeligate ) {
        
        leftDel = leftDeligate;
        rightDel = rightDeligate;

        instance.questionText.text = promptText;
        instance.leftButton.GetComponentInChildren<Text>().text = leftText;
        instance.rightButton.GetComponentInChildren<Text>().text = rightText;

        instance.promptCanvasGroup.alpha = 1f;
    }
}

