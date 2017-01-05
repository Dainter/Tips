using System;
using System.Windows;
using System.Windows.Controls;

namespace Tips.UI_Resources
{
    public class TaskItem: ListBoxItem
    {
        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("TaskName", typeof(string), typeof(TaskItem));

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(int), typeof(TaskItem));

        public string TaskName
        {
            get { return (string)GetValue(TaskProperty); }
            set { SetValue(TaskProperty, value); }
        }

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
    }
}
