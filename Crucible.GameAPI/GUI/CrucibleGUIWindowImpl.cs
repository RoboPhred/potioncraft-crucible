// <copyright file="CrucibleGUIWindowImpl.cs" company="RoboPhredDev">
// This file is part of the Crucible Modding Framework.
//
// Crucible is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// Crucible is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Crucible; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

namespace RoboPhredDev.PotionCraft.Crucible.GameAPI.GUI
{
    using System;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Implementation of a Crucible GUI window.
    /// This overlays a Unity <see cref="GUILayout"/> window on top of a PotionCraft <see cref="DebugWindow"/>.
    /// </summary>
    internal class CrucibleGUIWindowImpl : ICrucibleGUIWindow
    {
        private readonly Action<ICrucibleGUIWindow> render;

        private readonly DebugWindow debugWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrucibleGUIWindowImpl"/> class.
        /// </summary>
        /// <param name="title">The initial title of the window.</param>
        /// <param name="render">The window render function.</param>
        public CrucibleGUIWindowImpl(string title, Action<ICrucibleGUIWindow> render)
        {
            this.render = render;

            DebugWindow.CheckWindowContainer();
            var gameObject = UnityEngine.Object.Instantiate(Managers.Debug.settings.debugWindowPrefab, Managers.Debug.debugWindowContainer);
            gameObject.name = $"CrucibleGUI Window \"{title}\"";
            var debugWindow = gameObject.GetComponent<DebugWindow>();
            debugWindow.captionText.text = title;
            debugWindow.bodyText.text = Environment.NewLine;
            debugWindow.Visible = true;
            debugWindow.gameObject.SetActive(true);
            this.debugWindow = debugWindow;

            Traverse.Create(this.debugWindow).Method("FixOutOfBoundsCase").GetValue();
        }

        /// <summary>
        /// Raised when the window wants to close.
        /// </summary>
        public event EventHandler OnCloseRequested;

        /// <inheritdoc/>
        public string Title
        {
            get
            {
                return this.debugWindow.captionText.text;
            }

            set
            {
                this.debugWindow.captionText.text = value;
                this.debugWindow.name = $"CrucibleGUI Window \"{value}\"";
            }
        }

        /// <inheritdoc/>
        public bool Visible
        {
            get
            {
                return this.debugWindow.Visible;
            }

            set
            {
                this.debugWindow.Visible = value;
            }
        }

        /// <inheritdoc/>
        public float Width
        {
            get
            {
                return this.debugWindow.colliderBackground.size.x;
            }

            set
            {
                this.Resize(value, this.Height);
            }
        }

        /// <inheritdoc/>
        public float Height
        {
            get
            {
                return this.debugWindow.colliderBackground.size.y;
            }

            set
            {
                this.Resize(this.Width, value);
            }
        }

        /// <inheritdoc/>
        public float X
        {
            get
            {
                return this.debugWindow.transform.position.x;
            }

            set
            {
                this.debugWindow.transform.position = new Vector2(value, this.Y);
            }
        }

        /// <inheritdoc/>
        public float Y
        {
            get
            {
                return this.debugWindow.transform.position.y;
            }

            set
            {
                this.debugWindow.transform.position = new Vector2(this.X, value);
            }
        }

        /// <inheritdoc/>
        public Rect LayoutRect
        {
            get
            {
                var headerScreenOffset = new Vector2(0, 35);
                var margin = new Vector2(5, 5);
                var windowSize = this.WindowSizeToScreen(this.debugWindow.colliderBackground.size) - headerScreenOffset - (margin * 2);
                var windowPos = this.WindowPosToScreen(this.debugWindow.transform.position) + headerScreenOffset + margin;
                return new Rect(windowPos, windowSize);
            }
        }

        /// <inheritdoc/>
        public void Close()
        {
            this.OnCloseRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Destroys the window.
        /// </summary>
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this.debugWindow.gameObject);
        }

        /// <summary>
        /// Renders the window.
        /// </summary>
        /// <remarks>
        /// This should be called inside a Unity behavior's <c>OnGUI</c> function.
        public void OnGUI()
        {
            var defaultContentColor = GUI.contentColor;
            var defaultBackgroundColor = GUI.backgroundColor;
            GUI.contentColor = Color.black;
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            try
            {
                GUILayout.BeginArea(this.LayoutRect);
                this.render(this);
                GUILayout.EndArea();
            }
            finally
            {
                GUI.contentColor = defaultContentColor;
                GUI.backgroundColor = defaultBackgroundColor;
            }
        }

        private void Resize(float width, float height)
        {
            var vector2 = new Vector2(width, height);
            this.debugWindow.colliderBackground.size = vector2;
            this.debugWindow.colliderBackground.offset = vector2 / 2f * (Vector2.right + Vector2.down);
            this.debugWindow.spriteBackground.size = vector2;
            this.debugWindow.spriteScratches.size = vector2;
            var localPosition = this.debugWindow.headTransform.localPosition;
            this.debugWindow.headTransform.localPosition = new Vector3(vector2.x - 0.06f, localPosition.y, localPosition.z);

            Traverse.Create(this.debugWindow).Method("FixOutOfBoundsCase").GetValue();
        }

        private Vector2 WindowPosToScreen(Vector2 windowPosition)
        {
            // Window coords are centered at 0,0
            // Leftmost is -12.8ish (seems to have more space on the left than the right?)
            // Top is around 7.22 and inverted compared to screen.
            var x = windowPosition.x;
            var y = windowPosition.y;

            x = (x + 12.8f) / (12.8f * 2) * Screen.width;
            y = (-y + 7.22f) / (7.22f * 2) * Screen.height;

            return new Vector2(x, y);
        }

        private Vector2 WindowSizeToScreen(Vector2 windowSize)
        {
            return windowSize *= 75;
        }
    }
}
