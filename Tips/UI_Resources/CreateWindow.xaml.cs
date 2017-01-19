using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
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
    /// CreateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateWindow : Window
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
        bool bIsConfirm;
        string strName, strCategory, strQlevel, strCategoryID;
        DateTime sDate, dDate;
        List<string> tasksteps;
        //Database
        TipsDBDataSet tipsDBDataSet;
        TipsDBDataSetTableAdapters.tabCategoryTableAdapter tipsDBDataSetTabCategoryTableAdapter;
        TipsDBDataSetTableAdapters.tabQLevelTableAdapter tipsDBDataSetTabQLevelTableAdapter;
        TipsDBDataSetTableAdapters.tabTaskStepTableAdapter tipsDBDataSettabTaskStepTableAdapter;
        CollectionViewSource tabCategoryViewSource;
        CollectionViewSource tabQLevelViewSource;

        public bool IsConfirm
        {
            get { return bIsConfirm; }
        }

        public CreateWindow()
        {
            InitializeComponent();
        }

        private void WinCreateTask_Loaded(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Transparent;
            ExtendAeroGlass(this);
            DataInit();
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
            // 将数据加载到表 ViewEditTask 中。可以根据需要修改此代码。
            tipsDBDataSetTabCategoryTableAdapter = new TipsDBDataSetTableAdapters.tabCategoryTableAdapter();
            tipsDBDataSetTabCategoryTableAdapter.Fill(tipsDBDataSet.tabCategory);
            tabCategoryViewSource = ((CollectionViewSource)(this.FindResource("tabCategoryViewSource")));

            tipsDBDataSetTabQLevelTableAdapter = new TipsDBDataSetTableAdapters.tabQLevelTableAdapter();
            tipsDBDataSetTabQLevelTableAdapter.Fill(tipsDBDataSet.tabQLevel);
            tabQLevelViewSource = ((CollectionViewSource)(this.FindResource("tabQLevelViewSource")));

            tipsDBDataSettabTaskStepTableAdapter = new TipsDBDataSetTableAdapters.tabTaskStepTableAdapter();
            tipsDBDataSettabTaskStepTableAdapter.Fill(tipsDBDataSet.tabTaskStep);

        }

        private void WinCreateTask_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e)
        {
            string strStepName;
            InputDialog DiaNewStep = new InputDialog("请输入新步骤名称：");
            DiaNewStep.Owner = this;
            DiaNewStep.ShowDialog();
            strStepName = DiaNewStep.Output;
            stepItemListBox.Items.Add(strStepName);
        }

        private void RemoveStepButton_Click(object sender, RoutedEventArgs e)
        {
            List<object> SelectItemList;

            if (stepItemListBox.SelectedItems.Count <= 0)
            {
                return;
            }
            SelectItemList = new List<object>();
            foreach (object StepItem in stepItemListBox.SelectedItems)
            {
                SelectItemList.Add(StepItem);
            }
            foreach (object StepItem in SelectItemList)
            {
                stepItemListBox.Items.Remove(StepItem);
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            //合法性校验
            if (InputVarification() == false)
            {
                return;
            }
            //数据组织
            strCategoryID = GetCategoryIndex(strCategory);
            //数据插入数据表
            //存入任务步骤到tabTaskStep
            if (tasksteps != null)
            {

            }
            //存入主数据到tabProcessTask

            bIsConfirm = true;
            this.Close();
        }

        private bool InputVarification()
        {
            //任务名
            strName = taskNameTextBox.Text;
            if (strName == "")
            {
                InputWarning.PlacementTarget = taskNameTextBox;
                WarningInfo.Text = "Please enter a non-empty value.";
                InputWarning.IsOpen = true;
                return false;
            }
            //起始时间
            sDate = DateTime.Now;
            if (IsNowCheckBox.IsChecked == false)
            {
                if (startDateDatePicker.SelectedDate == null)
                {
                    InputWarning.PlacementTarget = startDateDatePicker;
                    WarningInfo.Text = "Please select a start date for the task.";
                    InputWarning.IsOpen = true;
                    return false;
                }
                sDate = new DateTime(startDateDatePicker.SelectedDate.Value.Year,
                                                    startDateDatePicker.SelectedDate.Value.Month,
                                                    startDateDatePicker.SelectedDate.Value.Day,
                                                    DateTime.Now.Hour,
                                                    DateTime.Now.Minute,
                                                    DateTime.Now.Second);
            }
            //完成期限
            if (deadDateDatePicker.SelectedDate == null)
            {
                InputWarning.PlacementTarget = deadDateDatePicker;
                WarningInfo.Text = "Please select a deadline for the task.";
                InputWarning.IsOpen = true;
                return false;
            }
            dDate = new DateTime(deadDateDatePicker.SelectedDate.Value.Year,
                                                    deadDateDatePicker.SelectedDate.Value.Month,
                                                    deadDateDatePicker.SelectedDate.Value.Day,
                                                    DateTime.Now.Hour,
                                                    DateTime.Now.Minute,
                                                    DateTime.Now.Second);
            if (dDate.CompareTo(sDate) <= 0)
            {
                InputWarning.PlacementTarget = deadDateDatePicker;
                WarningInfo.Text = "The deadline must later than the start date.";
                InputWarning.IsOpen = true;
                return false;
            }
            //任务类别
            strCategory = categoryComboBox.Text;
            if (strCategory == "")
            {
                InputWarning.PlacementTarget = categoryComboBox;
                WarningInfo.Text = "Please select a category for the task.";
                InputWarning.IsOpen = true;
                return false;
            }
            //任务级别
            strQlevel = qlevelComboBox.Text;
            if (strQlevel == "")
            {
                InputWarning.PlacementTarget = qlevelComboBox;
                WarningInfo.Text = "Please select a Q level for the task.";
                InputWarning.IsOpen = true;
                return false;
            }
            //步骤
            if(stepItemListBox.Items.Count ==0 )
            {
                tasksteps = null;
                return true;
            }
            tasksteps = new List<string>();
            foreach (string sItem in stepItemListBox.Items)
            {
                tasksteps.Add(sItem);
            }
            return true;
        }

        private string GetCategoryIndex(string sCategory)
        {
            foreach(DataRow currentRow in tipsDBDataSet.tabCategory)
            {
                if(sCategory == (string)currentRow["Category"])
                {
                    return currentRow["ID"].ToString();
                }
            }
            return "0";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            bIsConfirm = false;
            this.Close();
        }
    }
}
