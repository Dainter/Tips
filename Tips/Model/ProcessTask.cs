using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Model
{
    public enum TaskStatus
    {
        Created = 1,
        Delay =2,
        Abort = 3,
        Completed = 4,
    }

    public class ProcessTask
    {
        string strName;
        DateTime datStart;
        DateTime datDeadline;
        double dubPriority;
        List<TaskStep> tasksteps;
        
        public string Name
        {
            get { return strName; }
        }

        public String StartTime
        {
            get { return datStart.ToString(); }
        }

        public String DeadLine
        {
            get { return datDeadline.ToString(); }
        }

        public double Priority
        {
            get { return dubPriority; }
        }

        public List<TaskStep> TaskSteps
        {
            get { return tasksteps; }
        }

        public ProcessTask(string sName, DateTime dStart, DateTime dDead, double dPriority)
        {
            strName = sName;
            datStart = dStart;
            datDeadline = dDead;
            dubPriority = dPriority;
            TaskStepInit(datStart.ToString());
        }

        private void TaskStepInit(string sDatetime)
        {
            tasksteps = new List<TaskStep>();


        }

    }
}
