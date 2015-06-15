using System;

namespace MemTraceTool
{
  enum TreeAxis
  {
    CallStack,
    CallStackReverse,
    FileName,
    Heap,
    AssetType,
    AssetPath,
    ComponentType
  }

  sealed class TreePerspective
  {
    internal static readonly TreePerspective Default = new TreePerspective("Heap/Stack", new TreeAxis[] { TreeAxis.Heap, TreeAxis.CallStack });
    internal static readonly TreePerspective Reverse = new TreePerspective("Heap/Stack (Reverse)", new TreeAxis[] { TreeAxis.Heap, TreeAxis.CallStackReverse });
    internal static readonly TreePerspective Asset = new TreePerspective("Heap/Asset Type/Asset Path", new TreeAxis[] { TreeAxis.Heap, TreeAxis.AssetType, TreeAxis.AssetPath, TreeAxis.CallStack });
    internal static readonly TreePerspective AssetType = new TreePerspective("Asset Type/Heap", new TreeAxis[] { TreeAxis.AssetType, TreeAxis.Heap });
    internal static readonly TreePerspective Component = new TreePerspective("Heap/Component Type/Stack", new TreeAxis[] { TreeAxis.Heap, TreeAxis.ComponentType, TreeAxis.CallStack });

    internal static readonly TreePerspective[] AllItems = new TreePerspective[] {
      Default,
      Reverse,
      Asset,
      AssetType,
      Component
    };

    internal string Name { get; private set; }
    internal TreeAxis[] Axes { get; private set; }

    private TreePerspective(string name, TreeAxis[] axes)
    {
      Name = name;
      Axes = axes;
    }

    public override string ToString()
    {
      return Name;
    }
  }
}