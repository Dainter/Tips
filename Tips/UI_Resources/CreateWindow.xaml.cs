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

        List<TaskStep> tasksteps;
        TipsDBDataSet tipsDBDataSet;
        TipsDBDataSetTableAdapters.tabCategoryTableAdapter tipsDBDataSetTabCategoryTableAdapter;
        TipsDBDataSetTableAdapters.tabQLevelTableAdapter tipsDBDataSetTabQLevelTableAdapter;
        TipsDBDataSetTableAdapters.tabTaskStepTableAdapter tipsDBDataSettabTaskStepTableAdapter;
        CollectionViewSource tabCategoryViewSource;
        CollectionViewSource tabQLevelViewSource;

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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
