namespace Rampastring.XNAUI.PlatformSpecific;

using System;
#if WINFORMS
using System.Windows.Forms;
#endif

internal interface IGameWindowManager
{
#if WINFORMS
    event EventHandler GameWindowClosing;

    event EventHandler ClientSizeChanged;

    void AllowClosing();

#if !NETFRAMEWORK
    [System.Runtime.Versioning.SupportedOSPlatform("windows5.1.2600")]
#endif
    void FlashWindow();

    void HideWindow();

    void MaximizeWindow();

    void MinimizeWindow();

    void PreventClosing();

    void SetControlBox(bool value);

    void SetIcon(string path);

    void ShowWindow();

    int GetWindowWidth();

    int GetWindowHeight();

    void SetFormBorderStyle(FormBorderStyle borderStyle);

#endif
    IntPtr GetWindowHandle();

    bool HasFocus();

    void CenterOnScreen();

    void SetBorderlessMode(bool value);
}