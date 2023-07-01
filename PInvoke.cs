﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

#pragma warning disable CS1591,CS1573,CS0465,CS0649,CS8019,CS1570,CS1584,CS1658,CS0436,CS8981
using global::System;
using global::System.Diagnostics;
using global::System.Diagnostics.CodeAnalysis;
using global::System.Runtime.CompilerServices;
using global::System.Runtime.InteropServices;
using global::System.Runtime.Versioning;
using winmdroot = global::Windows.Win32;
namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>Passes message information to the specified window procedure. (Unicode)</summary>
        /// <param name="lpPrevWndFunc">
        /// <para>Type: <b>WNDPROC</b> The previous window procedure. If this value is obtained by calling the <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-getwindowlonga">GetWindowLong</a> function with the <i>nIndex</i> parameter set to <b>GWL_WNDPROC</b> or <b>DWL_DLGPROC</b>, it is actually either the address of a window or dialog box procedure, or a special internal value meaningful only to <b>CallWindowProc</b>.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="hWnd">
        /// <para>Type: <b>HWND</b> A handle to the window procedure to receive the message.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="Msg">
        /// <para>Type: <b>UINT</b> The message.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="wParam">
        /// <para>Type: <b>WPARAM</b> Additional message-specific information. The contents of this parameter depend on the value of the <i>Msg</i> parameter.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <param name="lParam">
        /// <para>Type: <b>LPARAM</b> Additional message-specific information. The contents of this parameter depend on the value of the <i>Msg</i> parameter.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#parameters">Read more on docs.microsoft.com</see>.</para>
        /// </param>
        /// <returns>
        /// <para>Type: <b>LRESULT</b> The return value specifies the result of the message processing and depends on the message sent.</para>
        /// </returns>
        /// <remarks>
        /// <para>Use the <b>CallWindowProc</b> function for window subclassing. Usually, all windows with the same class share one window procedure. A subclass is a window or set of windows with the same class whose messages are intercepted and processed by another window procedure (or procedures) before being passed to the window procedure of the class. The <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-setwindowlonga">SetWindowLong</a> function creates the subclass by changing the window procedure associated with a particular window, causing the system to call the new window procedure instead of the previous one. An application must pass any messages not processed by the new window procedure to the previous window procedure by calling <b>CallWindowProc</b>. This allows the application to create a chain of window procedures. If <b>STRICT</b> is defined, the <i>lpPrevWndFunc</i> parameter has the data type <b>WNDPROC</b>. The <b>WNDPROC</b> type is declared as follows:</para>
        /// <para></para>
        /// <para>This doc was truncated.</para>
        /// <para><see href="https://docs.microsoft.com/windows/win32/api/winuser/nf-winuser-callwindowprocw#">Read more on docs.microsoft.com</see>.</para>
        /// </remarks>
        [DllImport("USER32.dll", ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
#if !NETFRAMEWORK
        [SupportedOSPlatform("windows5.0")]
#endif
        /*
         * We need to match the delegate used by XNA.
         *
         * CsWin32 Generated:
         * internal static extern winmdroot.Foundation.LRESULT CallWindowProcW(winmdroot.UI.WindowsAndMessaging.WNDPROC lpPrevWndFunc, winmdroot.Foundation.HWND hWnd, uint Msg, winmdroot.Foundation.WPARAM wParam, winmdroot.Foundation.LPARAM lParam);
        */
        internal static extern winmdroot.Foundation.LRESULT CallWindowProcW(winmdroot.UI.WindowsAndMessaging.WndProcDelegate lpPrevWndFunc, winmdroot.Foundation.HWND hWnd, uint Msg, winmdroot.Foundation.WPARAM wParam, winmdroot.Foundation.LPARAM lParam);
    }
}