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
        bool bIsConfirm, bIsChanged = false;
        string strName, strCategory, strQlevel, strCategoryID;
        DateTime sDate, dDate;
        List<TaskStep> tasksteps;
        List<string> insertSteps;
        List<TaskStep> deleteSteps;
        string strTaskKey; 
        TipsDBDataSet tipsDBDataSet;
        TipsDBDataSetTableAdapters.ViewEditTaskTableAdapter tipsDBDataSetViewEditTaskTableAdapter;
        TipsDBDataSetTableAdapters.tabCategoryTableAdapter tipsDBDataSetTabCategoryTableAdapter;
        TipsDBDataSetTableAdapters.tabQLevelTableAdapter tipsDBDataSetTabQLevelTableAdapter;
        TipsDBDataSetTableAdapters.tabProcessTaskTableAdapter tipsDBDataSetTabProcessTaskTableAdapter;
        TipsDBDataSetTableAdapters.tabTaskStepTableAdapter tipsDBDataSettabTaskStepTableAdapter;
        //CollectionViewSource viewEditTaskViewSource;
        CollectionViewSource tabCategoryViewSource;
        CollectionViewSource tabQLevelViewSource;

        public bool IsConfirm
        {
            get { return bIsConfirm; }
        }

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

            tipsDBDataSetTabCategoryTableAdapter = new TipsDBDataSetTableAdapters.tabCategoryTableAdapter();
            tipsDBDataSetTabCategoryTableAdapter.Fill(tipsDBDataSet.tabCategory);
            tabCategoryViewSource = ((CollectionViewSource)(this.FindResource("tabCategoryViewSource")));

            tipsDBDataSetTabQLevelTableAdapter = new TipsDBDataSetTableAdapters.tabQLevelTableAdapter();
            tipsDBDataSetTabQLevelTableAdapter.Fill(tipsDBDataSet.tabQLevel);
            tabQLevelViewSource = ((CollectionViewSource)(this.FindResource("tabQLevelViewSource")));

            tipsDBDataSetTabProcessTaskTableAdapter = new TipsDBDataSetTableAdapters.tabProcessTaskTableAdapter();
            tipsDBDataSetTabProcessTaskTableAdapter.Fill(tipsDBDataSet.tabProcessTask);

            tipsDBDataSettabTaskStepTableAdapter = new TipsDBDataSetTableAdapters.tabTaskStepTableAdapter();
            tipsDBDataSettabTaskStepTableAdapter.Fill(tipsDBDataSet.tabTaskStep);

        }

        private void LoadTaskInfo()
        {
            TipsDBDataSet.ViewEditTaskDataTable table = new TipsDBDataSet.ViewEditTaskDataTable();
            tipsDBDataSetViewEditTaskTableAdapter.Fill(table);
            
            foreach (DataRow currentRow in table.Rows)
            {
                sDate = (DateTime)currentRow["StartDate"];
                if (sDate.ToString() != strTaskKey)
                {
                    continue;
                }
                startDateDatePicker.SelectedDate = sDate;
                strName = (string)currentRow["TaskName"];
                taskNameTextBox.Text = strName;
                dDate = (DateTime)currentRow["DeadDate"];
                deadDateDatePicker.SelectedDate = dDate;
                strCategory = (string)currentRow["Category"];
                categoryComboBox.Text = strCategory;
                strQlevel = (string)currentRow["Qlevel"];
                qlevelComboBox.Text = strQlevel;
                break;
            }
            LoadStepsInfo();
        }

        private void LoadStepsInfo()
        {
            tasksteps = new List<TaskStep>();
            int Index;
            string strName;
            
            string strCommand = "SELECT [ID],[TaskStep] FROM tabTaskStep WHERE [StartDate] = #" + sDate.ToString() + "#";
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

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            //合法性校验
            if (DataVarification() == false)
            {
                return;
            }
            if (bIsChanged == false)
            {
                bIsConfirm = false;
                this.Close();
                return;
            }
            //数据组织
            strCategoryID = GetCategoryIndex(strCategory);
            //保存数据
            UpdateTask();
            bIsConfirm = true;
            this.Close();
        }

        private bool DataVarification()
        {
            bool bolIsExist;
            bIsChanged = false;
            //任务名
            if (taskNameTextBox.Text == "")
            {
                InputWarning.PlacementTarget = taskNameTextBox;
                WarningInfo.Text = "Please enter a non-empty value.";
                InputWarning.IsOpen = true;
                return false;
            }
            if(strName != taskNameTextBox.Text)
            {
                strName = taskNameTextBox.Text;
                bIsChanged = true;
            }
            
            //完成期限
            if (deadDateDatePicker.SelectedDate == null)
            {
                InputWarning.PlacementTarget = deadDateDatePicker;
                WarningInfo.Text = "Please select a deadline for the task.";
                InputWarning.IsOpen = true;
                return false;
            }
            if (deadDateDatePicker.SelectedDate.Value.CompareTo(sDate) <= 0)
            {
                InputWarning.PlacementTarget = deadDateDatePicker;
                WarningInfo.Text = "The deadline must later than the start date.";
                InputWarning.IsOpen = true;
                return false;
            }
            if (deadDateDatePicker.SelectedDate.Value.CompareTo(dDate) != 0)
            {
                dDate = new DateTime(deadDateDatePicker.SelectedDate.Value.Year,
                                                    deadDateDatePicker.SelectedDate.Value.Month,
                                                    deadDateDatePicker.SelectedDate.Value.Day,
                                                    17,
                                                    0,
                                                    0);
                bIsChanged = true;
            }
            //任务类别
            if (categoryComboBox.Text == "")
            {
                InputWarning.PlacementTarget = categoryComboBox;
                WarningInfo.Text = "Please select a category for the task.";
                InputWarning.IsOpen = true;
                return false;
            }
            if (strCategory != categoryComboBox.Text)
            {
                strCategory = categoryComboBox.Text;
                bIsChanged = true;
            }
            //任务级别
            if (strQlevel == "")
            {
                InputWarning.PlacementTarget = qlevelComboBox;
                WarningInfo.Text = "Please select a Q level for the task.";
                InputWarning.IsOpen = true;
                return false;
            }
            if (strQlevel != qlevelComboBox.Text)
            {
                strQlevel = qlevelComboBox.Text;
                bIsChanged = true;
            }
            //步骤
            if (stepItemListBox.Items.Count == 0)
            {
                tasksteps = null;
                return true;
            }
            //比较两表生成一个Insert表，一个Delete表
            deleteSteps = new List<TaskStep>();
            foreach (TaskStep curStep in tasksteps)
            {
                bolIsExist = false;
                foreach (string sItem in stepItemListBox.Items)
                {
                    if (curStep.StepName == sItem)
                    {
                        bolIsExist = true;
                    }
                }
                if (bolIsExist == false)
                {
                    deleteSteps.Add(curStep);
                }
            }
            insertSteps = new List<string>();
            foreach (string sItem in stepItemListBox.Items)
            {
                bolIsExist = false;
                foreach (TaskStep curStep in tasksteps)
                {
                    if (curStep.StepName == sItem)
                    {
                        bolIsExist = true;
                    }
                }
                if (bolIsExist == false)
                {
                    insertSteps.Add(sItem);
                }
            }
            if (deleteSteps.Count == 0 && insertSteps.Count == 0)
            {
                bIsChanged = true;
            }
            return true;
        }

        private string GetCategoryIndex(string sCategory)
        {
            foreach (DataRow currentRow in tipsDBDataSet.tabCategory)
            {
                if (sCategory == (string)currentRow["Category"])
                {
                    return currentRow["ID"].ToString();
                }
            }
            return "0";
        }

        private void UpdateTask()
        {
            //存入主数据到tabProcessTask
            UpdateTaskItem(strName, sDate.ToString(), dDate.ToString(), strCategoryID, strQlevel);

            if (deleteSteps.Count != 0)
            {
                //删除任务步骤
                foreach (TaskStep curStep in deleteSteps)
                {
                    DeleteTaskStep(curStep.Index.ToString());
                }
            }
            if (insertSteps.Count != 0)
            {
                //存入任务步骤到tabTaskStep
                foreach (string step in insertSteps)
                {
                    InsertTaskStep(step, sDate.ToString());
                }
            }
            return;
        }

        private void UpdateTaskItem(string sName, string ssDate, string sdDate, string sCategory, string sQlevel)
        {
            String strCommand = "UPDATE tabProcessTask SET [TaskName] ='"+ sName +
                                                                                        "', [DeadDate]= #"+ sdDate +
                                                                                        "#, [Qlevel] = '"+ sQlevel +
                                                                                        "', [CategoryID] = " + sCategory +
                                                                                        " WHERE [StartDate] = #" + ssDate + "#";
            tipsDBDataSetTabProcessTaskTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSetTabProcessTaskTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSetTabProcessTaskTableAdapter.Connection.Close();
        }

        private void DeleteTaskStep(string sStepID)
        {
            String strCommand = "DELETE FROM tabTaskStep WHERE [ID] ="+ sStepID;
            tipsDBDataSettabTaskStepTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabTaskStepTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSettabTaskStepTableAdapter.Connection.Close();
        }

        private void InsertTaskStep(string sStepName, string strKey)
        {
            String strCommand = "INSERT INTO tabTaskStep ([TaskStep],[StartDate],[StepCompleted]) VALUES ('" + sStepName + "', #" + strKey + "#, 0)";
            tipsDBDataSettabTaskStepTableAdapter.Connection.Open();
            OleDbCommand command = new OleDbCommand(strCommand, tipsDBDataSettabTaskStepTableAdapter.Connection);
            int iCount = command.ExecuteNonQuery();
            tipsDBDataSet.GetChanges();
            tipsDBDataSettabTaskStepTableAdapter.Connection.Close();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            bIsConfirm = false;
            this.Close();
        }
    }
}
