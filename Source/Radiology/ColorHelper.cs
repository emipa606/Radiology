using UnityEngine;

namespace Radiology;

public static class ColorHelper
{
    public static string Text(this Color color, string text)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
    }
}