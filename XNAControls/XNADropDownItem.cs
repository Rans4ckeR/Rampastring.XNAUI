﻿namespace Rampastring.XNAUI.XNAControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A drop-down / context menu item.
/// </summary>
public class XNADropDownItem
{
    public Color? TextColor { get; set; }

    public Texture2D Texture { get; set; }

    public string Text { get; set; }

    /// <summary>
    /// An object for containing custom info in the drop down item.
    /// </summary>
    public object Tag { get; set; }

    public bool Selectable { get; set; } = true;

    private float alpha = 1.0f;

    public float Alpha
    {
        get => alpha;

        set
        {
            if (value < 0.0f)
                alpha = 0.0f;
            else if (value > 1.0f)
                alpha = 1.0f;
            else
                alpha = value;
        }
    }
}