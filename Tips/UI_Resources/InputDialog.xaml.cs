﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Tips.UI_Resources
{
    /// <summary>
    /// DelayDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InputDialog : Window
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        };

        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref MARGINS pMarInset);
        //Global Element
        private string strTitle;
        private string strOutput;
        public string Output
        {
            get { return strOutput; }
        }

        public InputDialog(string sTitle)
        {
            InitializeComponent();
            strTitle = sTitle;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ExtendAeroGlass(Window window)
        {
            try
            {
                // 为WPF程序获取窗口句柄
                IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                // 设置Margins
                MARGINS margins = new MARGINS();

                // 扩展Aero Glass
                margins.cxLeftWidth = -1;
                margins.cxRightWidth = -1;
                margins.cyTopHeight = -1;
                margins.cyBottomHeight = -1;

                int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                if (hr < 0)
                {
                    MessageBox.Show("DwmExtendFrameIntoClientArea Failed");
                }
            }
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Transparent;
            ExtendAeroGlass(this);
            TitleBox.Text = strTitle;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            strOutput = InputBox.Text;
            if (strOutput == "")
            {
                InputWarning.IsOpen = true;
                return;
            }
            this.Close();
        }
    }
}
