using System;
using System.Windows;
using System.Windows.Controls;

namespace Tips.UI_Resources
{
    public class StepItem : ListBoxItem
    {
        public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register("TaskStep", typeof(string), typeof(StepItem));

        public static DependencyProperty IsCompletedProperty =
            DependencyProperty.Register("IsCompleted", typeof(bool), typeof(StepItem));

        public string TaskStep
        {
            get { return (string)GetValue(StepProperty); }
        }

        public bool IsCompleted
        {
            get { return (bool)GetValue(IsCompletedProperty); }
            set { SetValue(IsCompletedProperty, value); }
        }

        public StepItem(string sName, bool bIsCompeted) : base()
        {
            SetValue(StepProperty, sName);
            SetValue(IsCompletedProperty, bIsCompeted);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
    }
}
