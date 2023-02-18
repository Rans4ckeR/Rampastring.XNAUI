﻿namespace Rampastring.XNAUI.XNAControls;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class XNAListBoxItem
{
    public XNAListBoxItem()
    {
    }

    public XNAListBoxItem(string text)
    {
        Text = text;
    }

    public XNAListBoxItem(string text, Color textColor)
    {
        Text = text;
        TextColor = textColor;
    }

    public event EventHandler TextChanged;

    private Color? textColor;

    public Color TextColor
    {
        get => textColor ?? (!Selectable ? UISettings.ActiveSettings.DisabledItemColor : UISettings.ActiveSettings.AltColor);

        set => textColor = value;
    }

    private Color? backgroundColor;

    public Color BackgroundColor
    {
        get => backgroundColor ?? UISettings.ActiveSettings.BackgroundColor;

        set => backgroundColor = value;
    }

    public Texture2D Texture { get; set; }

    public bool IsHeader { get; set; }

    private string text;

    /// <summary>
    /// The text of the list box item prior to its parsing by the list box.
    /// If this is modified when the item belongs to a <see cref="XNAListBox"/>, the ListBox
    /// will re-parse it and save the result <see cref="TextLines"/> to support multi-line
    /// items and potentially cut the text if it's too long.
    /// </summary>
    public string Text
    {
        get => text;
        set
        {
            text = value;
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Stores optional custom data associated with the list box item.
    /// </summary>
    public object Tag { get; set; }

    public int TextYPadding { get; set; }

    public int TextXPadding { get; set; }

    public bool Selectable { get; set; } = true;

    private float alpha;

    public float Alpha
    {
        get => alpha;

        set => alpha = value < 0.0f ? 0.0f : value > 1.0f ? 1.0f : value;
    }

    public List<string> TextLines;
}