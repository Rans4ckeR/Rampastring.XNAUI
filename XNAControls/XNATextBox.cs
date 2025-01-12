﻿namespace Rampastring.XNAUI.XNAControls;

using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Rampastring.Tools;
using Rampastring.XNAUI.Input;
using TextCopy;

/// <summary>
/// A text input control.
/// </summary>
public class XNATextBox : XNAControl
{
    private const int TEXT_HORIZONTAL_MARGIN = 3;
    private const int TEXT_VERTICAL_MARGIN = 2;
    private const double SCROLL_REPEAT_TIME = 0.03;
    private const double FAST_SCROLL_TRIGGER_TIME = 0.4;
    private const double BAR_ON_TIME = 0.5;
    private const double BAR_OFF_TIME = 0.5;

    /// <summary>
    /// Creates a new text box.
    /// </summary>
    /// <param name="windowManager">The WindowManager that will be associated with this control.</param>
    public XNATextBox(WindowManager windowManager)
        : base(windowManager)
    {
    }

    /// <summary>
    /// Raised when the user presses the Enter key while this text box is the
    /// selected control.
    /// </summary>
    public event EventHandler EnterPressed;

    /// <summary>
    /// Raised when the text box receives input that changes its text.
    /// </summary>
    public event EventHandler InputReceived;

    /// <summary>
    /// Raised whenever the text of the text box is changed, by the user or
    /// programmatically.
    /// </summary>
    public event EventHandler TextChanged;

    private Color? _textColor;

    /// <summary>
    /// The color of the text in the text box.
    /// </summary>
    public virtual Color TextColor
    {
        get => _textColor ?? UISettings.ActiveSettings.AltColor;

        set => _textColor = value;
    }

    private Color? _idleBorderColor;

    /// <summary>
    /// The color of the text box border when the text box is inactive.
    /// </summary>
    public virtual Color IdleBorderColor
    {
        get => _idleBorderColor ?? UISettings.ActiveSettings.PanelBorderColor;

        set => _idleBorderColor = value;
    }

    private Color? _activeBorderColor;

    /// <summary>
    /// The color of the text box border when the text box is selected.
    /// </summary>
    public Color ActiveBorderColor
    {
        get => _activeBorderColor ?? UISettings.ActiveSettings.AltColor;

        set => _activeBorderColor = value;
    }

    private Color? _backColor;

    /// <summary>
    /// The color of the text box background.
    /// </summary>
    public Color BackColor
    {
        get => _backColor ?? UISettings.ActiveSettings.BackgroundColor;

        set => _backColor = value;
    }

    /// <summary>
    /// The index of the spritefont that this textbox uses.
    /// </summary>
    public int FontIndex { get; set; }

    /// <summary>
    /// The maximum length of the text of this text box, in characters.
    /// </summary>
    public int MaximumTextLength { get; set; } = int.MaxValue;

    /// <summary>
    /// The text on the text box.
    /// </summary>
    public override string Text
    {
        get => _text;

        set
        {
            _text = value ?? throw new InvalidOperationException("XNATextBox text cannot be set to null.");
            InputPosition = 0;
            TextStartPosition = 0;

            if (_text.Length > MaximumTextLength)
                _text = _text.SafeSubstring(0, MaximumTextLength);

            TextEndPosition = _text.Length;

            while (!TextFitsBox())
            {
                TextEndPosition--;

                if (TextEndPosition < TextStartPosition)
                {
                    TextEndPosition = TextStartPosition;
                    break;
                }
            }

            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private string _text = string.Empty;

    /// <summary>
    /// The input character index inside the textbox text.
    /// </summary>
    public int InputPosition { get; set; }

    /// <summary>
    /// The start character index of the visible part of the text string.
    /// </summary>
    public int TextStartPosition { get; set; }

    /// <summary>
    /// The end character index of the visible part of the text string.
    /// </summary>
    public int TextEndPosition { get; set; }

    // TODO PreviousControl and NextControl should be implemented at XNAControl level,
    // but we currently lack a generic way to handle input in a selected control
    // (without having all controls subscribe to Keyboard.OnKeyPressed, which could
    // have bad effects on performance)

    /// <summary>
    /// The control to switch selection state to when
    /// the user presses Tab while holding Shift.
    /// See also <see cref="NextControl"/>.
    /// </summary>
    public XNAControl PreviousControl { get; set; }

    /// <summary>
    /// The control to switch selection state to when
    /// the user presses Tab without holding Shift.
    /// See also <see cref="PreviousControl"/>.
    /// </summary>
    public XNAControl NextControl { get; set; }

    protected TimeSpan barTimer = TimeSpan.Zero;

    private TimeSpan scrollKeyTime = TimeSpan.Zero;
    private TimeSpan timeSinceLastScroll = TimeSpan.Zero;
    private bool isScrollingQuickly;

    protected override void ParseControlINIAttribute(IniFile iniFile, string key, string value)
    {
        if (key == nameof(MaximumTextLength))
        {
            MaximumTextLength = Conversions.IntFromString(value, MaximumTextLength);
            return;
        }

        base.ParseControlINIAttribute(iniFile, key, value);
    }

    /// <summary>
    /// Initializes the text box.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

#if !XNA
        Game.Window.TextInput += Window_TextInput;
#else
        KeyboardEventInput.CharEntered += KeyboardEventInput_CharEntered;
#endif
        Keyboard.OnKeyPressed += Keyboard_OnKeyPressed;
    }

    public override void Kill()
    {
#if !XNA
        Game.Window.TextInput -= Window_TextInput;
#else
        KeyboardEventInput.CharEntered -= KeyboardEventInput_CharEntered;
#endif
        Keyboard.OnKeyPressed -= Keyboard_OnKeyPressed;

        base.Kill();
    }

#if XNA
    private void KeyboardEventInput_CharEntered(object sender, KeyboardEventArgs e)
#else
    private void Window_TextInput(object sender, TextInputEventArgs e)
#endif
        => HandleCharInput(e.Character);

    private void HandleCharInput(char character)
    {
        if (WindowManager.SelectedControl != this || !Enabled || !Parent.Enabled || !WindowManager.HasFocus)
            return;

        switch (character)
        {
            /*/ There are a bunch of keys that are detected as text input on
             * Windows builds of MonoGame, but not on WindowsGL or Linux builds of MonoGame.
             * We already handle these keys (enter, tab, backspace, escape) by other means,
             * so we don't want to handle them also as text input on Windows to avoid
             * potentially harmful extra triggering of the InputReceived event.
             * So, we detect that input here and return on these keys.
            /*/
            case (char)Keys.Enter:
            case (char)Keys.Tab:
            case (char)Keys.Back:
            case (char)Keys.Escape:
                return;
            default:
                if (_text.Length == MaximumTextLength)
                    break;

                // Don't allow typing characters that don't exist in the spritefont
                if (Renderer.GetSafeString(character.ToString(), FontIndex) != character.ToString())
                    break;

                if (!AllowCharacterInput(character))
                    break;

                _text = _text.Insert(InputPosition, character.ToString());
                InputPosition++;

                if (TextEndPosition >= _text.Length - 1 ||
                    InputPosition > TextEndPosition)
                {
                    TextEndPosition++;

                    while (!TextFitsBox())
                    {
                        TextStartPosition++;
                    }
                }

                TextChanged?.Invoke(this, EventArgs.Empty);
                break;
        }

        barTimer = TimeSpan.Zero;

        InputReceived?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Determines if the user is allowed to type a specific character into the textbox.
    /// </summary>
    /// <param name="character">The character.</param>
    protected virtual bool AllowCharacterInput(char character) => true; // Allow all characters by default

    private void Keyboard_OnKeyPressed(object sender, KeyPressEventArgs e)
    {
        if (WindowManager.SelectedControl != this || !Enabled || !Parent.Enabled || !WindowManager.HasFocus)
            return;

        e.Handled = HandleKeyPress(e.PressedKey);
    }

    /// <summary>
    /// Handles a key press while the text box is the selected control.
    /// Can be overridden in derived classes to handle additional key presses.
    /// Returns true if the key was handled.
    /// </summary>
    /// <param name="key">The key that was pressed.</param>
    protected virtual bool HandleKeyPress(Keys key)
    {
        switch (key)
        {
            case Keys.Home:
                if (_text.Length != 0)
                {
                    TextStartPosition = 0;
                    TextEndPosition = 0;
                    InputPosition = 0;

                    while (true)
                    {
                        if (TextEndPosition < _text.Length)
                        {
                            TextEndPosition++;

                            if (!TextFitsBox())
                            {
                                TextEndPosition--;
                                break;
                            }

                            continue;
                        }

                        break;
                    }
                }

                return true;
            case Keys.End:
                TextEndPosition = _text.Length;
                InputPosition = _text.Length;
                TextStartPosition = 0;

                while (true)
                {
                    if (!TextFitsBox())
                    {
                        TextStartPosition++;
                        continue;
                    }

                    break;
                }

                return true;
            case Keys.X:
                if (!Keyboard.IsCtrlHeldDown())
                    break;

                if (!string.IsNullOrEmpty(_text))
                {
                    ClipboardService.SetText(_text);
                    Text = string.Empty;
                    InputReceived?.Invoke(this, EventArgs.Empty);
                }

                return true;
            case Keys.V:
                if (!Keyboard.IsCtrlHeldDown())
                    break;

                string clipboardText = ClipboardService.GetText();
                if (clipboardText == null)
                    return true;

                // Replace newlines with spaces
                // https://stackoverflow.com/questions/238002/replace-line-breaks-in-a-string-c-sharp
                string textToAdd = Regex.Replace(clipboardText, @"\r\n?|\n", " ");
                Text += Renderer.GetSafeString(textToAdd, FontIndex);
                InputReceived?.Invoke(this, EventArgs.Empty);

                goto case Keys.End;
            case Keys.C:
                if (!Keyboard.IsCtrlHeldDown())
                    break;

                if (!string.IsNullOrEmpty(_text))
                    ClipboardService.SetText(_text);

                return true;
            case Keys.Enter:
                EnterPressed?.Invoke(this, EventArgs.Empty);
                return true;
            case Keys.Escape:
                InputPosition = 0;
                Text = string.Empty;
                InputReceived?.Invoke(this, EventArgs.Empty);
                return true;
            case Keys.Tab:
                if (Keyboard.IsShiftHeldDown())
                {
                    if (PreviousControl != null)
                        WindowManager.SelectedControl = PreviousControl;
                }
                else if (NextControl != null)
                {
                    WindowManager.SelectedControl = NextControl;
                }

                return true;
        }

        return false;
    }

    private bool TextFitsBox()
    {
        return string.IsNullOrEmpty(_text) || Renderer.GetTextDimensions(
            _text.SafeSubstring(TextStartPosition, TextEndPosition - TextStartPosition),
            FontIndex).X < Width - (TEXT_HORIZONTAL_MARGIN * 2);
    }

    public override void OnLeftClick()
    {
        int x = GetCursorPoint().X;
        int inputPosition = TextEndPosition;

        var stringBuilder = new StringBuilder();

        for (int i = TextStartPosition; i < TextEndPosition - TextStartPosition; i++)
        {
            stringBuilder.Append(Text[i]);
            if (Renderer.GetTextDimensions(stringBuilder.ToString(), FontIndex).X +
                TEXT_HORIZONTAL_MARGIN > x)
            {
                inputPosition = i;
                break;
            }
        }

        InputPosition = Math.Max(0, inputPosition);

        barTimer = TimeSpan.Zero;

        base.OnLeftClick();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        barTimer += gameTime.ElapsedGameTime;

        if (barTimer > TimeSpan.FromSeconds(BAR_ON_TIME + BAR_OFF_TIME))
            barTimer -= TimeSpan.FromSeconds(BAR_ON_TIME + BAR_OFF_TIME);

        if (WindowManager.SelectedControl == this)
        {
            if (Keyboard.IsKeyHeldDown(Keys.Left))
            {
                HandleScrollKeyDown(gameTime, ScrollLeft);
            }
            else if (Keyboard.IsKeyHeldDown(Keys.Right))
            {
                HandleScrollKeyDown(gameTime, ScrollRight);
            }
            else if (Keyboard.IsKeyHeldDown(Keys.Delete))
            {
                HandleScrollKeyDown(gameTime, DeleteCharacter);
            }
            else if (Keyboard.IsKeyHeldDown(Keys.Back))
            {
                HandleScrollKeyDown(gameTime, Backspace);
            }
            else
            {
                isScrollingQuickly = false;
                timeSinceLastScroll = TimeSpan.Zero;
                scrollKeyTime = TimeSpan.Zero;
            }
        }
    }

    private void ScrollLeft()
    {
        if (InputPosition == 0)
            return;

        InputPosition--;
        if (InputPosition < TextStartPosition)
        {
            TextStartPosition--;

            while (!TextFitsBox())
                TextEndPosition--;
        }
    }

    private void ScrollRight()
    {
        if (InputPosition >= _text.Length)
            return;

        InputPosition++;

        if (InputPosition > TextEndPosition)
        {
            TextEndPosition++;

            while (!TextFitsBox())
            {
                TextStartPosition++;
            }
        }
    }

    private void DeleteCharacter()
    {
        if (_text.Length > InputPosition)
        {
            _text = _text.Remove(InputPosition, 1);

            if (TextStartPosition > 0)
                TextStartPosition--;

            if (TextEndPosition > _text.Length || !TextFitsBox())
                TextEndPosition--;

            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        InputReceived?.Invoke(this, EventArgs.Empty);
    }

    private void Backspace()
    {
        if (_text.Length > 0 && InputPosition > 0)
        {
            _text = _text.Remove(InputPosition - 1, 1);
            InputPosition--;

            if (TextStartPosition > 0)
                TextStartPosition--;

            if (TextEndPosition > _text.Length || !TextFitsBox())
                TextEndPosition--;

            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        InputReceived?.Invoke(this, EventArgs.Empty);
    }

    private void HandleScrollKeyDown(GameTime gameTime, Action action)
    {
        if (scrollKeyTime.Equals(TimeSpan.Zero))
            action();

        scrollKeyTime += gameTime.ElapsedGameTime;

        if (isScrollingQuickly)
        {
            timeSinceLastScroll += gameTime.ElapsedGameTime;

            if (timeSinceLastScroll > TimeSpan.FromSeconds(SCROLL_REPEAT_TIME))
            {
                timeSinceLastScroll = TimeSpan.Zero;
                action();
            }
        }

        if (scrollKeyTime > TimeSpan.FromSeconds(FAST_SCROLL_TRIGGER_TIME) && !isScrollingQuickly)
        {
            isScrollingQuickly = true;
            timeSinceLastScroll = TimeSpan.Zero;
        }

        barTimer = TimeSpan.Zero;
    }

    public override void Draw(GameTime gameTime)
    {
        FillControlArea(BackColor);

        if (WindowManager.SelectedControl == this && Enabled && WindowManager.HasFocus)
            DrawRectangle(new(0, 0, Width, Height), ActiveBorderColor);
        else
            DrawRectangle(new(0, 0, Width, Height), IdleBorderColor);

        DrawStringWithShadow(
            Text.SafeSubstring(TextStartPosition, TextEndPosition - TextStartPosition),
            FontIndex,
            new(TEXT_HORIZONTAL_MARGIN, TEXT_VERTICAL_MARGIN),
            TextColor);

        if (WindowManager.SelectedControl == this && Enabled && WindowManager.HasFocus && barTimer.TotalSeconds < BAR_ON_TIME)
        {
            int barLocationX = TEXT_HORIZONTAL_MARGIN;

            string inputText = Text.SafeSubstring(TextStartPosition, InputPosition - TextStartPosition);
            barLocationX += (int)Renderer.GetTextDimensions(inputText, FontIndex).X;

            FillRectangle(new(barLocationX, 2, 1, Height - 4), Color.White);
        }

        base.Draw(gameTime);
    }
}