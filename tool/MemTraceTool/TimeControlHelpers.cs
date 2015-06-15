using MemTrace;
using MemTrace.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTraceTool
{
  class TimeControlHelpers
  {
    public static void CreateTimeControlMarks(TimeControl tc, TraceProcessorBase trace)
    {
      // Build set of times stamps from marks
      {
        var m = new List<TimeControl.Mark>();
        m.Add(new TimeControl.Mark { Name = "Start", Time = 0.0 });
        var marks = new List<TraceMark>();
        trace.MetaData.GetTraceMarks(marks);
        foreach (var mark in marks)
        {
          m.Add(new TimeControl.Mark
          {
            Name = mark.Name,
            Time = mark.TimeStamp / (double)trace.MetaData.TimerFrequency
          });
        }
        tc.MinTime = 0;
        tc.MaxTime = trace.MetaData.MaxTimeStamp / (double)trace.MetaData.TimerFrequency;
        m.Add(new TimeControl.Mark { Name = "End", Time = tc.MaxTime });
        tc.SetMarks(m);
      }
    }
  }
}
