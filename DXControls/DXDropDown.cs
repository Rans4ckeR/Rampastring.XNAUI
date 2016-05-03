﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using System;
using System.Collections.Generic;

namespace Rampastring.XNAUI.DXControls
{
    /// <summary>
    /// A combo box, commonly also known as a drop-down box.
    /// </summary>
    public class DXDropDown : DXControl
    {
        public DXDropDown(WindowManager windowManager) : base(windowManager)
        {
            BorderColor = UISettings.PanelBorderColor;
            FocusColor = UISettings.FocusColor;
            BackColor = UISettings.BackgroundColor;
        }

        public delegate void SelectedIndexChangedEventHandler(object sender, EventArgs e);
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;

        int _itemHeight = 17;
        public int ItemHeight
        {
            get { return _itemHeight; }
            set { _itemHeight = value; }
        }

        public List<DXDropDownItem> Items = new List<DXDropDownItem>();

        /// <summary>
        /// Gets or sets the dropped-down status of the drop-down control.
        /// </summary>
        public bool IsDroppedDown { get; set; }

        bool _allowDropDown = true;

        /// <summary>
        /// Controls whether the drop-down control can be dropped down.
        /// </summary>
        public bool AllowDropDown
        {
            get { return _allowDropDown; }
            set { _allowDropDown = value; }
        }

        int _selectedIndex = -1;

        /// <summary>
        /// Gets or sets the selected index of the drop-down control.
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                int oldSelectedIndex = _selectedIndex;

                _selectedIndex = value;

                if (value != oldSelectedIndex && SelectedIndexChanged != null)
                    SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        public int FontIndex { get; set; }

        public Color BorderColor { get; set; }

        public Color FocusColor { get; set; }

        public Color BackColor { get; set; }

        Texture2D dropDownTexture { get; set; }
        Texture2D dropDownOpenTexture { get; set; }

        public SoundEffect ClickSoundEffect { get; set; }
        SoundEffectInstance _clickSoundInstance;

        bool leftClickHandled = false;

        int hoveredIndex = 0;

        #region AddItem methods

        /// <summary>
        /// Adds an item into the drop-down.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(DXDropDownItem item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Generates and adds an item with the specified text into the drop-down.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        public void AddItem(string text)
        {
            DXDropDownItem item = new DXDropDownItem();
            item.Text = text;
            item.TextColor = UISettings.AltColor;

            Items.Add(item);
        }

        /// <summary>
        /// Generates and adds an item with the specified text and texture
        /// into the drop-down.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="texture">The item's texture.</param>
        public void AddItem(string text, Texture2D texture)
        {
            DXDropDownItem item = new DXDropDownItem();
            item.Text = text;
            item.TextColor = UISettings.AltColor;
            item.Texture = texture;

            Items.Add(item);
        }

        /// <summary>
        /// Generates and adds an item with the specified text
        /// and text color into the drop-down control.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="color">The color of the item's text.</param>
        public void AddItem(string text, Color color)
        {
            DXDropDownItem item = new DXDropDownItem();
            item.Text = text;
            item.TextColor = color;

            Items.Add(item);
        }

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            dropDownTexture = AssetLoader.LoadTexture("comboBoxArrow.png");
            dropDownOpenTexture = AssetLoader.LoadTexture("openedComboBoxArrow.png");

            ClientRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, dropDownTexture.Height);

            if (ClickSoundEffect != null)
                _clickSoundInstance = ClickSoundEffect.CreateInstance();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Hide the drop-down if the left mouse button is clicked while the
            // cursor isn't on this control
            if (IsDroppedDown && Cursor.LeftClicked && !leftClickHandled)
                OnLeftClick(); 

            leftClickHandled = false;
        }

        public override void OnLeftClick()
        {
            base.OnLeftClick();

            if (_clickSoundInstance != null)
                _clickSoundInstance.Play();

            if (!IsDroppedDown)
            {
                Rectangle wr = WindowRectangle();

                IsDroppedDown = true;
                ClientRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, dropDownTexture.Height + 1 + ItemHeight * Items.Count);
                hoveredIndex = -1;
                leftClickHandled = true;
                return;
            }

            Point p = GetCursorPoint();

            if (p.Y > dropDownTexture.Height + 1)
            {
                int y = p.Y - dropDownTexture.Height + 1;
                int itemIndex = y / _itemHeight;

                if (itemIndex >= Items.Count || itemIndex < 0)
                    SelectedIndex = 0;
                else if (Items[itemIndex].Selectable)
                    SelectedIndex = itemIndex;
            }

            IsDroppedDown = false;
            ClientRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, dropDownTexture.Height);

            leftClickHandled = true;
        }

        public override void OnMouseMove()
        {
            base.OnMouseMove();

            if (!IsDroppedDown)
                return;

            Point p = GetCursorPoint();

            if (p.Y > dropDownTexture.Height + 1)
            {
                int y = p.Y - dropDownTexture.Height + 1;
                int itemIndex = y / _itemHeight;

                hoveredIndex = itemIndex;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle wr = WindowRectangle();

            Renderer.FillRectangle(new Rectangle(wr.X + 1, wr.Y + 1, wr.Width - 2, wr.Height - 2), BackColor);
            Renderer.DrawRectangle(new Rectangle(wr.X, wr.Y, wr.Width, dropDownTexture.Height), BorderColor);

            if (SelectedIndex > -1)
            {
                DXDropDownItem item = Items[SelectedIndex];

                int textX = 3;
                if (item.Texture != null)
                {
                    Renderer.DrawTexture(item.Texture, new Rectangle(wr.X + 1, wr.Y + 2, item.Texture.Width, item.Texture.Height), Color.White);
                    textX += item.Texture.Width + 1;
                }

                Renderer.DrawStringWithShadow(item.Text, FontIndex, new Vector2(wr.X + textX, wr.Y + 2), item.TextColor);
            }

            if (AllowDropDown)
            {
                Rectangle ddRectangle = new Rectangle(wr.X + wr.Width - dropDownTexture.Width,
                    wr.Y, dropDownTexture.Width, dropDownTexture.Height);

                if (IsDroppedDown)
                {
                    Renderer.DrawTexture(dropDownOpenTexture,
                        ddRectangle, GetColorWithAlpha(RemapColor));

                    Renderer.DrawRectangle(new Rectangle(wr.X, wr.Y + dropDownTexture.Height, wr.Width, wr.Height + 1 - dropDownTexture.Height), BorderColor);

                    for (int i = 0; i < Items.Count; i++)
                    {
                        DXDropDownItem item = Items[i];

                        int y = wr.Y + dropDownTexture.Height + 1 + i * ItemHeight;
                        if (hoveredIndex == i)
                        {
                            Renderer.FillRectangle(new Rectangle(wr.X + 1, y, wr.Width - 2, ItemHeight), FocusColor);
                        }
                        else
                            Renderer.FillRectangle(new Rectangle(wr.X + 1, y, wr.Width - 2, ItemHeight), BackColor);

                        int textX = 2;
                        if (item.Texture != null)
                        {
                            Renderer.DrawTexture(item.Texture, new Rectangle(wr.X + 1, y + 1, item.Texture.Width, item.Texture.Height), Color.White);
                            textX += item.Texture.Width + 1;
                        }

                        Renderer.DrawStringWithShadow(item.Text, FontIndex, new Vector2(wr.X + textX, y + 1), item.TextColor);
                    }
                }
                else
                    Renderer.DrawTexture(dropDownTexture, ddRectangle, RemapColor);
            }

            base.Draw(gameTime);
        }
    }

    public class DXDropDownItem
    {
        public Color TextColor { get; set; }

        public Texture2D Texture { get; set; }

        public string Text { get; set; }

        bool selectable = true;
        public bool Selectable
        {
            get { return selectable; }
            set { selectable = value; }
        }

        float alpha = 1.0f;
        public float Alpha
        {
            get { return alpha; }
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
}
