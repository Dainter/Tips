using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Data;
using System.Runtime.InteropServices;
using Tips.Model;
using Tips.UI_Resources;

namespace Tips
{
    /// <summary>
    /// TaskStepsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TaskStepsWindow : Window
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
        ProcessTask currentTask;
        public bool IsChanged = false;
        TipsDBDataSet tipsDBDataSet;
        TipsDBDataSetTableAdapters.tabTaskStepTableAdapter tipsDBDataSettabTaskStepTableAdapter;

        public TaskStepsWindow(ProcessTask cTask)
        {
            InitializeComponent();
            currentTask = cTask;
            TaskNameBox.Text = currentTask.TaskName + " 任务步骤";
        }

        private void WinTaskSteps_Loaded(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Transparent;
            ExtendAeroGlass(this);
            TaskStepsInit();
            tipsDBDataSet = ((TipsDBDataSet)(this.FindResource("tipsDBDataSet")));
            // 将数据加载到表 tabTaskStep 中。可以根据需要修改此代码。
            tipsDBDataSettabTaskStepTableAdapter = new TipsDBDataSetTableAdapters.tabTaskStepTableAdapter();
            tipsDBDataSettabTaskStepTableAdapter.Fill(tipsDBDataSet.tabTaskStep);
            CollectionViewSource tabTaskStepViewSource = ((CollectionViewSource)(this.FindResource("tabTaskStepViewSource")));
            tabTaskStepViewSource.View.MoveCurrentToFirst();
        }

        private void TaskStepsInit()
        {
            StepItem newStep;
            foreach (TaskStep curStep in currentTask.TaskSteps)
            {
                newStep = new StepItem(curStep.StepName, curStep.IsCompleted);
                newStep.Template = StepListBox.Resources["StepItemTemplate"] as ControlTemplate;
                StepListBox.Items.Add(newStep);
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

        private void WinTaskSteps_MouseMove(object sender, MouseEventArgs e)
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

        private void CheckModify()
        {
            StepItem curStepItem;
            int index = 0;

            IsChanged = false;
            foreach (TaskStep curStep in currentTask.TaskSteps)
            {
                curStepItem = StepListBox.Items[index] as StepItem;
                if (curStepItem.TaskStep != curStep.StepName)
                {
                    continue;
                }
                if (curStepItem.IsCompleted != curStep.IsCompleted)
                {
                    SaveModification(curStep.Index, curStepItem.IsCompleted);
                    IsChanged = true;
                }
                index++;
            }
            return;
        }

        private void SaveModification(int Index, bool bIsCompleted)
        {
            string strCommand = "UPDATE tabTaskStep SET [StepCompleted] = " + bIsCompleted.ToString() + " WHERE [ID] = "+ Index.ToString();
            tipsDBDataSettabTaskStepTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabTaskStepTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSettabTaskStepTableAdapter.Connection.Close();
        }

        private void WinTaskSteps_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CheckModify();
        }
    }
}
