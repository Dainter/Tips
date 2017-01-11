using System;
using System.Windows;
using System.Windows.Controls;

namespace Tips.UI_Resources
{
    public class TaskItem: ListBoxItem
    {
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("TaskName", typeof(string), typeof(TaskItem));

        public static DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(int), typeof(TaskItem));

        public string TaskName
        {
            get { return (string)GetValue(TaskProperty); }
        }

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public TaskItem(string sName, int iProgress) : base()
        {
            SetValue(TaskProperty, sName);
            SetValue(ProgressProperty, iProgress);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
    }
}
