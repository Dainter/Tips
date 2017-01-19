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
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
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
        string strTaskKey;
        List<TaskStep> tasksteps;
        TipsDBDataSet tipsDBDataSet;
        TipsDBDataSetTableAdapters.ViewEditTaskTableAdapter tipsDBDataSetViewEditTaskTableAdapter;
        TipsDBDataSetTableAdapters.tabCategoryTableAdapter tipsDBDataSetTabCategoryTableAdapter;
        TipsDBDataSetTableAdapters.tabQLevelTableAdapter tipsDBDataSetTabQLevelTableAdapter;
        TipsDBDataSetTableAdapters.tabTaskStepTableAdapter tipsDBDataSettabTaskStepTableAdapter;
        //CollectionViewSource viewEditTaskViewSource;
        CollectionViewSource tabCategoryViewSource;
        CollectionViewSource tabQLevelViewSource;

        public EditWindow(string sDate)
        {
            InitializeComponent();
            strTaskKey = sDate;
        }

        private void WinEditTask_Loaded(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Transparent;
            ExtendAeroGlass(this);
            DataInit();
            LoadTaskInfo();
        }

        private void DataInit()
        {
            tipsDBDataSet = ((TipsDBDataSet)(this.FindResource("tipsDBDataSet")));
            // 将数据加载到表 ViewEditTask 中。可以根据需要修改此代码。
            tipsDBDataSetViewEditTaskTableAdapter = new TipsDBDataSetTableAdapters.ViewEditTaskTableAdapter();
            tipsDBDataSetViewEditTaskTableAdapter.Fill(tipsDBDataSet.ViewEditTask);
            //viewEditTaskViewSource = ((CollectionViewSource)(this.FindResource("viewEditTaskViewSource")));
            //viewEditTaskViewSource.View.MoveCurrentToFirst();

            tipsDBDataSetTabCategoryTableAdapter = new TipsDBDataSetTableAdapters.tabCategoryTableAdapter();
            tipsDBDataSetTabCategoryTableAdapter.Fill(tipsDBDataSet.tabCategory);
            tabCategoryViewSource = ((CollectionViewSource)(this.FindResource("tabCategoryViewSource")));

            tipsDBDataSetTabQLevelTableAdapter = new TipsDBDataSetTableAdapters.tabQLevelTableAdapter();
            tipsDBDataSetTabQLevelTableAdapter.Fill(tipsDBDataSet.tabQLevel);
            tabQLevelViewSource = ((CollectionViewSource)(this.FindResource("tabQLevelViewSource")));

            tipsDBDataSettabTaskStepTableAdapter = new TipsDBDataSetTableAdapters.tabTaskStepTableAdapter();
            tipsDBDataSettabTaskStepTableAdapter.Fill(tipsDBDataSet.tabTaskStep);

        }

        private void LoadTaskInfo()
        {
            DateTime start;
            TipsDBDataSet.ViewEditTaskDataTable table = new TipsDBDataSet.ViewEditTaskDataTable();
            tipsDBDataSetViewEditTaskTableAdapter.Fill(table);
            
            foreach (DataRow currentRow in table.Rows)
            {
                start = (DateTime)currentRow["StartDate"];
                if (start.ToString() != strTaskKey)
                {
                    continue;
                }
                taskNameTextBox.Text = (string)currentRow["TaskName"];
                startDateDatePicker.SelectedDate = start;
                deadDateDatePicker.SelectedDate = (DateTime)currentRow["DeadDate"];
                categoryComboBox.Text = (string)currentRow["Category"];
                qlevelComboBox.Text = (string)currentRow["Qlevel"];
            }
            LoadStepsInfo();
        }

        private void LoadStepsInfo()
        {
            tasksteps = new List<TaskStep>();
            int Index;
            string strName;
            
            string strCommand = "SELECT [ID],[TaskStep] FROM tabTaskStep WHERE [StartDate] = #" + strTaskKey + "#";
            tipsDBDataSettabTaskStepTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabTaskStepTableAdapter.Connection);
            OleDbDataReader DataReader = command.ExecuteReader();
            while(DataReader.Read() == true)
            {
                Index = (int)DataReader["ID"];
                strName = (string)DataReader["TaskStep"];
                tasksteps.Add(new TaskStep(Index, strName));
                stepItemListBox.Items.Add(strName);
            }
            tipsDBDataSettabTaskStepTableAdapter.Connection.Close();

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

        private void WinEditTask_MouseMove(object sender, MouseEventArgs e)
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

            if(stepItemListBox.SelectedItems.Count <=0)
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
    }

    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
        {
            if ((int)value > 0)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
