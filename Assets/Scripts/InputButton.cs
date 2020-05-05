using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType { Left = 0, Right = 1, Shoot =2 }

public class InputButton : MonoBehaviour
{
    public ButtonType Type;
    public Image VisualIndication;

    public Color ActiveColor;
    public Color InactiveColor;

    public void ActivateVisual (bool active) {
        if (active) {
            VisualIndication.color = ActiveColor;
        } else {
            VisualIndication.color = InactiveColor;
        }
    }
}
