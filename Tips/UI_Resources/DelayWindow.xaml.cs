using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tips.Model;

namespace Tips.UI_Resources
{
    /// <summary>
    /// DelayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DelayWindow : Window
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
        //Global Elements
        bool bIsChanged = false;
        TaskPlan taskplan;
        //Data
        TipsDBDataSet tipsDBDataSet;
        TipsDBDataSetTableAdapters.tabProcessTaskTableAdapter tipsDBDataSettabProcessTaskTableAdapter;

        public bool IsChanged
        {
            get { return bIsChanged; }
        }

        public DelayWindow(TaskPlan tPlan)
        {
            InitializeComponent();
            taskplan = tPlan;
    }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Transparent;
            ExtendAeroGlass(this);
            DataInit();
            RefreshTaskListBox();
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

        private void DataInit()
        {
            tipsDBDataSet = ((TipsDBDataSet)(this.FindResource("tipsDBDataSet")));
            // 将数据加载到表 tabProcessTask 中。可以根据需要修改此代码。
            tipsDBDataSettabProcessTaskTableAdapter = new TipsDBDataSetTableAdapters.tabProcessTaskTableAdapter();
            tipsDBDataSettabProcessTaskTableAdapter.Fill(tipsDBDataSet.tabProcessTask);
        }

        private void RefreshTaskListBox()
        {
            TaskListBox.Items.Clear();

            foreach (TaskItem newItem in taskplan.DelayTaskList)
            {
                newItem.Template = TaskListBox.Resources["ListItemTemplate"] as ControlTemplate;
                newItem.ContextMenu = TaskListBox.Resources["TaskContexMenu"] as ContextMenu;
                TaskListBox.Items.Add(newItem);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TaskResumeCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TaskListBox.SelectedIndex > -1)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
            return;
        }

        private void TaskResumeCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TaskResume(taskplan.GetDelayKeybyIndex(TaskListBox.SelectedIndex));
            TaskListBox.Items.RemoveAt(TaskListBox.SelectedIndex);
            bIsChanged = true;
        }

        private void TaskResume(string start)
        {
            String strCommand = "UPDATE tabProcessTask SET [TaskStatusID] = 1, [DelayReason] = '' WHERE [StartDate] = #" + start + "#";
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabProcessTaskTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Close();
        }

    }
}
