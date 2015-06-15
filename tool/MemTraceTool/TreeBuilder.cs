using MemTrace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemTraceTool
{
  public enum EventScope : byte
  {
    None,
    Asset,
    Component,
  }


  abstract class TreeBuilderBase
  {
    protected TraceReplayStateful Replay { get; private set; }
    protected TraceReplayStateful.StatusDelegate StatusCallback { get; private set; }
    protected MemTreeNode Root { get; private set; }
    protected TreePerspective Perspective { get; private set; }
    protected HashSet<string> SuppressedSymbols { get; private set; }

    protected TreeBuilderBase(TraceReplayStateful replay, TraceReplayStateful.StatusDelegate status_delegate, TreePerspective perspective, Options options)
    {
      Replay = replay;
      StatusCallback = status_delegate;
      Perspective = perspective;
      Root = new MemTreeNode(null, "Everything");
      SuppressedSymbols = new HashSet<string>();
      if (options.EnableSuppression)
      {
        foreach (var sym in options.SuppressedSymbols)
        {
          SuppressedSymbols.Add(sym);
        }
      }
    }

    protected void BuildBackTraceNodes(StackBackTrace backtrace, ref MemTreeNode leaf, bool reverse)
    {
      var frames = backtrace.Frames;
      int start, end, dir;

      if (reverse)
      {
        start = 0;
        end = frames.Count - 1;
        dir = 1;
      }
      else
      {
        start = frames.Count - 1;
        end = 0;
        dir = -1;
      }

      for (int i = start; i != end; i += dir)
      {
        var frame = frames[i];
        var name = frame.Symbol;
        var filename = frame.FileName;
        var lineno = frame.LineNumber;

        if (SuppressedSymbols.Contains(name))
          continue;

        leaf = leaf.GetChild(name);
        leaf.Icon = "symbol";
        leaf.FileName = filename;
        leaf.LineNumber = lineno;
      }
    }

    protected void UpdateReplayStatus()
    {
      //++m_EventStatusCount;
      //if (m_EventStatusCount % 10000 == 1 && EndTimeStamp != 0)
      //{
        //StatusCallback((double)Replay.CurrentTimeStamp / (double)EndTimeStamp);
      //}
    }

    protected void UpdateTree(HeapAllocationInfo info)
    {
      MemTreeNode leaf = Root;
      var heap = info.Heap;
      var stack = info.BackTrace;
      var scope = (EventScope) info.ScopeType;
      var scope_data = info.ScopeData;

      foreach (var axis in Perspective.Axes)
      {
        switch (axis)
        {
          case TreeAxis.Heap:
            leaf = leaf.GetChild(heap != null ? heap.Name : "(unknown heap)");
            leaf.Icon = "heap";
            break;

          case TreeAxis.CallStackReverse:
            BuildBackTraceNodes(stack, ref leaf, true);
            break;

          case TreeAxis.CallStack:
            BuildBackTraceNodes(stack, ref leaf, false);
            break;

          case TreeAxis.FileName:
            for (int i = stack.Frames.Count - 1; i >= 0; --i)
            {
              var frame = stack.Frames[i];

              if (SuppressedSymbols.Contains(frame.Symbol))
                continue;

              leaf = leaf.GetChild(Path.GetFileName(frame.FileName));
              leaf.Icon = "file";
              leaf.FileName = frame.FileName;
              leaf.LineNumber = frame.LineNumber;
            }
            break;

          case TreeAxis.AssetPath:
            if (scope == EventScope.Asset)
            {
              string[] elems = scope_data.Split('\\', '/');
              for (int i = 0; i < elems.Length - 1; ++i)
              {
                leaf = leaf.GetChild(elems[i]);
                leaf.Icon = "folder";
              }
              leaf = leaf.GetChild(elems[elems.Length - 1]);
              leaf.Icon = "asset";
            }
            else
            {
              leaf = leaf.GetChild("(no asset)");
            }
            break;

          case TreeAxis.AssetType:
            if (scope == EventScope.Asset)
            {
              leaf = leaf.GetChild(Path.GetExtension(scope_data));
              leaf.Icon = "asset";
            }
            else
            {
              leaf = leaf.GetChild("(no asset)");
            }
            break;

          case TreeAxis.ComponentType:
            if (scope == EventScope.Component)
            {
              leaf = leaf.GetChild(scope_data);
              leaf.Icon = "component";
            }
            else
            {
              leaf = leaf.GetChild("(no component)");
            }
            break;
        }
      }

      do
      {
        leaf.SizeBytes += (long) info.SizeBytes;
        leaf.Count += 1;
        leaf = leaf.Parent;
      } while (leaf != null);
    }

    public abstract MemTreeNode BuildReportTree();
  }

  sealed class SnapshotTreeBuilder : TreeBuilderBase
  {
    public SnapshotTreeBuilder(TraceReplayStateful t, double time, TraceReplayStateful.StatusDelegate status_delegate, TreePerspective perspective, Options options)
      : base(t, status_delegate, perspective, options)
    {
      Replay.SeekTo(time, StatusCallback);
    }

    // Return the tree nodes from a tree built from this snapshot.
    public override MemTreeNode BuildReportTree()
    {
      foreach (var alloc in Replay.HeapAllocations.Values)
      {
        UpdateTree(alloc);
      }

      Root.Freeze();

      return Root;
    }
  }

  sealed class DeltaTreeBuilder : TreeBuilderBase
  {
    private double m_StartTime;

    public DeltaTreeBuilder(TraceReplayStateful t, double startTime, double endTime, TraceReplayStateful.StatusDelegate status_delegate, TreePerspective perspective, Options options)
      : base(t, status_delegate, perspective, options)
    {
      m_StartTime = startTime;

      Replay.SeekTo(endTime, StatusCallback);
    }

    // Return the tree nodes from a tree built from this snapshot.
    public override MemTreeNode BuildReportTree()
    {
      foreach (var alloc in Replay.HeapAllocations.Values)
      {
        if (alloc.TimeCreated >= m_StartTime)
        {
          UpdateTree(alloc);
        }
      }

      Root.Freeze();

      return Root;
    }
  }

}
