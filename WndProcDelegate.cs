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
    namespace UI.WindowsAndMessaging
    {
        /*
         * We need to match the delegate used by XNA.
         *
         * Original Microsoft.Xna.Framework.Input.WindowMessageHooker.Hook.WndProcDelegate:
         * private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
         *
         * CsWin32 Generated winmdroot.UI.WindowsAndMessaging.WNDPROC:
         * [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
         * [global::System.CodeDom.Compiler.GeneratedCode("Microsoft.Windows.CsWin32", "0.3.18-beta+dc807e7787")]
         * internal unsafe delegate winmdroot.Foundation.LRESULT WNDPROC(winmdroot.Foundation.HWND param0, uint param1, winmdroot.Foundation.WPARAM param2, winmdroot.Foundation.LPARAM param3);
        */
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr WndProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    }
}