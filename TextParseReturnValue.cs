﻿namespace Rampastring.XNAUI;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

public class TextParseReturnValue
{
    public int LineCount;
    public readonly string Text;

    public TextParseReturnValue(string text, int lineCount)
    {
        Text = text;
        LineCount = lineCount;
    }

    public static TextParseReturnValue FixText(SpriteFont spriteFont, int width, string text)
    {
        string line = string.Empty;
        int lineCount = 0;
        string processedText = string.Empty;
        string[] wordArray = text.Split(' ');

        foreach (string word in wordArray)
        {
            if (spriteFont.MeasureString(line + word).Length() > width)
            {
                processedText = processedText + line + Environment.NewLine;
                lineCount++;
                line = string.Empty;
            }

            line = line + word + " ";
        }

        processedText += line;
        return new(processedText, lineCount);
    }

    public static List<string> GetFixedTextLines(SpriteFont spriteFont, int width, string text, bool splitWords = true, bool keepBlankLines = false)
    {
        if (string.IsNullOrEmpty(text))
            return new(0);

        var returnValue = new List<string>();

        // Remove '\r' characters so Windows newlines aren't counted twice
        string[] lineArray = text.Replace("\r", string.Empty).Split(new[] { '\n' }, StringSplitOptions.None);

        foreach (string originalTextLine in lineArray)
        {
            if (keepBlankLines && string.IsNullOrEmpty(originalTextLine))
                returnValue.Add(string.Empty);

            string line = string.Empty;

            string[] wordArray = originalTextLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in wordArray)
            {
                if (spriteFont.MeasureString(line + word).X > width)
                {
                    if (line.Length > 0)
                    {
                        returnValue.Add(line.Remove(line.Length - 1));
                    }

                    // Split individual words that are longer than the allowed width
                    if (splitWords && spriteFont.MeasureString(word).X > width)
                    {
                        var sb = new StringBuilder();

                        foreach (char character in word)
                        {
                            if (spriteFont.MeasureString(sb.ToString() + character).X > width)
                            {
                                returnValue.Add(sb.ToString());
                                sb.Clear();
                            }

                            sb.Append(character);
                        }

                        if (sb.Length > 0)
                            line = sb + " ";

                        continue;
                    }

                    line = word + " ";
                    continue;
                }

                line = line + word + " ";
            }

            if (!string.IsNullOrEmpty(line) && line.Length > 1)
                returnValue.Add(line.TrimEnd());
        }

        return returnValue;
    }
}