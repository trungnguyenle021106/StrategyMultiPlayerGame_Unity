using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MyColor32", menuName = "MyColor32")]
public class MyColor32 : ScriptableObject
{
    [Serializable]
    public class RGBA
    {
        public string Name;
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }

    public List<RGBA> RGBAs;
    public Color32 GetColor32(string colorName)
    {
        foreach (RGBA rgba in this.RGBAs)
        {
            if (rgba.Name.Equals(colorName))
            {
                return new Color32(rgba.r, rgba.g, rgba.b, rgba.a);
            }
        }
        return new Color32(0, 0, 0, 0);
    }
}
