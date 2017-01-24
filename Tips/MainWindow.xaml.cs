using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Data;
using System.Runtime.InteropServices;
using Tips.UI_Resources;

namespace Tips
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : UI_Resources.GWindow
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
        TaskPlan taskplan;
        TipsDBDataSet tipsDBDataSet;
        TipsDBDataSetTableAdapters.tabProcessTaskTableAdapter tipsDBDataSettabProcessTaskTableAdapter;

        public MainWindow()
        {
            InitializeComponent();
            FrmMainInit();
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

        private void FrmMain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Transparent;
            ExtendAeroGlass(this);
            DataInit();
        }

        private void DataInit()
        {
            tipsDBDataSet = ((TipsDBDataSet)(this.FindResource("tipsDBDataSet")));
            // 将数据加载到表 tabProcessTask 中。可以根据需要修改此代码。
            tipsDBDataSettabProcessTaskTableAdapter = new TipsDBDataSetTableAdapters.tabProcessTaskTableAdapter();
            tipsDBDataSettabProcessTaskTableAdapter.Fill(tipsDBDataSet.tabProcessTask);
        }

        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void FrmMainInit()
        {
            RefreshTaskListBox();
        }

        private void RefreshTaskListBox()
        {
            TaskListBox.Items.Clear();
            taskplan = new TaskPlan();
            foreach (TaskItem newItem in taskplan.ProcessTaskList)
            {
                newItem.Template = TaskListBox.Resources["ListItemTemplate"] as ControlTemplate;
                newItem.ContextMenu = TaskListBox.Resources["TaskContexMenu"] as ContextMenu;
                TaskListBox.Items.Add(newItem);
            }
        }

        private void RefreshTaskSteps(int index)
        {
            taskplan.RefreshTaskSteps(index);
        }

        private void RemoveCurrentTask(int index)
        {
            TaskListBox.Items.RemoveAt(index);
            taskplan.RemoveTask(index);
        }

        private void FrmMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.AreaMenuItems[0].Checked = false;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateWindow WinTaskCreate = new CreateWindow();
            WinTaskCreate.Owner = this;
            WinTaskCreate.ShowDialog();
            if (WinTaskCreate.IsConfirm == true)
            {
                RefreshTaskListBox();
            }  
        }

        private void ShowTipsItem_Click(object sender, EventArgs e)
        {
            //显示主窗口
            if (this.Visibility == Visibility.Hidden)
            {
                this.Show();
                ((System.Windows.Forms.MenuItem)sender).Checked = true;
            }
            else if (this.Visibility == Visibility.Visible)
            {
                this.Hide();
                ((System.Windows.Forms.MenuItem)sender).Checked = false;
            }
        }

        private void DelayTaskItem_Click(object sender, EventArgs e)
        {
            //打开延迟任务列表
            DelayWindow WinTaskDelay = new DelayWindow(taskplan);
            WinTaskDelay.Owner = this;
            WinTaskDelay.ShowDialog();
            if (WinTaskDelay.IsChanged == true)
            {
                RefreshTaskListBox();
            }
        }

        private void ShowHistoryItem_Click(object sender, EventArgs e)
        {
            //打开历史记录窗口
        }

        private void ShowOptionItem_Click(object sender, EventArgs e)
        {
            //打开配置窗口
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TaskCompleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TaskListBox.SelectedIndex > -1)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
            return;
        }

        private void TaskCompleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TaskComplete(taskplan.GetKeybyIndex(TaskListBox.SelectedIndex));
            RemoveCurrentTask(TaskListBox.SelectedIndex);
        }

        private void TaskComplete(string start)
        {
            String strCommand = "UPDATE tabProcessTask SET [TaskStatusID] = 4, [CompleteDate] = #" + DateTime.Now.ToString() +"# WHERE [StartDate] = #" + start +"#";
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabProcessTaskTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Close();
        }

        private void TaskDelayCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TaskListBox.SelectedIndex > -1)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
            return;
        }

        private void TaskDelayCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string strReason;
            InputDialog DiaDelay = new InputDialog("请输入任务延迟的原因：");
            DiaDelay.Owner = this;
            DiaDelay.ShowDialog();
            strReason = DiaDelay.Output;
            TaskDelay(taskplan.GetKeybyIndex(TaskListBox.SelectedIndex), strReason);
            RemoveCurrentTask(TaskListBox.SelectedIndex);
        }

        private void TaskDelay(string start, string sReason)
        {
            String strCommand = "UPDATE tabProcessTask SET [TaskStatusID] = 2, [DelayReason] = '" + sReason + "' WHERE [StartDate] = #" + start + "#";
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabProcessTaskTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Close();
        }

        private void TaskAbortCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TaskListBox.SelectedIndex > -1)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
            return;
        }

        private void TaskAbortCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TaskAbort(taskplan.GetKeybyIndex(TaskListBox.SelectedIndex));
            RemoveCurrentTask(TaskListBox.SelectedIndex);
        }

        private void TaskAbort(string start)
        {
            String strCommand = "UPDATE tabProcessTask SET [TaskStatusID] = 3 WHERE [StartDate] = #" + start + "#";
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabProcessTaskTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSettabProcessTaskTableAdapter.Connection.Close();
        }

        private void TaskEditCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TaskListBox.SelectedIndex > -1)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
            return;
        }

        private void TaskEditCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditWindow WinTaskEdit = new EditWindow(taskplan.GetKeybyIndex(TaskListBox.SelectedIndex));
            WinTaskEdit.Owner = this;
            WinTaskEdit.ShowDialog();
            if (WinTaskEdit.IsConfirm == true)
            {
                RefreshTaskListBox();
            }
        }

        private void TaskStepsCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (TaskListBox.SelectedIndex > -1)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
            return;
        }

        private void TaskStepsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int index = TaskListBox.SelectedIndex;
            TaskStepsWindow WinTaskSteps = new TaskStepsWindow(taskplan.GetTaskbyIndex(index));
            WinTaskSteps.Owner = this;
            WinTaskSteps.ShowDialog();
            if (WinTaskSteps.IsChanged == true)
            {
                RefreshTaskSteps(index);
            }
        }
    }
}
