using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIUtil
{
    public static void EnableButton(Button button) {

        //ColorBlock cb = ColorBlock.defaultColorBlock;
        //cb.colorMultiplier = 4f;
        //button.colors = cb;

        ColorBlock colors = button.colors;
        colors.colorMultiplier = 4f;
        Color color = colors.normalColor;
        color.a = 1f;

        button.enabled = true;
    }

    public static void DisableButton(Button button) {

        //ColorBlock disabledCB = ColorBlock.defaultColorBlock;
        //disabledCB.colorMultiplier = 0f;

        ColorBlock colors = button.colors;
        Color color = colors.disabledColor;
        color.a = 0.1f;

        button.colors = colors;
        button.enabled = false;
    }

}

