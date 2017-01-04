using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Model
{
    public class TaskStep
    {
        string strStepName;
        bool bolCompleted;

        public string StepName
        {
            get{ return strStepName; }
        }

        public bool IsCompleted
        {
            get { return IsCompleted; }
            set { IsCompleted = value; }
        }

        public TaskStep(string sName, bool bCompleted)
        {
            strStepName = sName;
            bolCompleted = bCompleted;
        }
    }
}
