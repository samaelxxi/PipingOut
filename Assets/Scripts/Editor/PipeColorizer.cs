using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum PipeColor { White, Orange, Green, Blue, Teal, Red, Black};

public class PipeColorizer
{
    static PipeTypes _pipes = Resources.Load<PipeTypes>("ScriptableObjects/PipeTypes");

    [MenuItem("Pipes/Make white")]
    public static void MakeWhite() { ColorPipe(PipeColor.White); }

    [MenuItem("Pipes/Make orange")]
    public static void MakeOrange() { ColorPipe(PipeColor.Orange); }

    [MenuItem("Pipes/Make green")]
    public static void MakeGreen() { ColorPipe(PipeColor.Green); }

    [MenuItem("Pipes/Make blue")]
    public static void MakeBlue() { ColorPipe(PipeColor.Blue); }

    [MenuItem("Pipes/Make teal")]
    public static void MakeTeal() { ColorPipe(PipeColor.Teal); }

    [MenuItem("Pipes/Make red")]
    public static void MakeRed() { ColorPipe(PipeColor.Red); }

    [MenuItem("Pipes/Make black")]
    public static void MakeBlack() { ColorPipe(PipeColor.Black); }

    public static void ColorPipe(PipeColor color)
    {
        if (_pipes == null)
            _pipes = Resources.Load<PipeTypes>("ScriptableObjects/PipeTypes");

        foreach (var pipe in Selection.transforms)
            TraversePipe(pipe, color);
    }

    static void TraversePipe(Transform pipe, PipeColor color)
    {
        PaintPipe(pipe, color);
        for (int i = 0; i < pipe.childCount; i++)
            TraversePipe(pipe.GetChild(i), color);
    }

    static void PaintPipe(Transform pipe, PipeColor color)
    {
        if (!pipe.TryGetComponent<MeshRenderer>(out var mesh))
            return;
        if (pipe.gameObject.name.Contains("SmallCornerPipe"))
            mesh.material = _pipes._smallCornerPipeMaterials[(int)color];
        else if (pipe.gameObject.name.Contains("CornerPipe"))
            mesh.material = _pipes._bigCornerPipeMaterials[(int)color];
        else if (pipe.gameObject.name.Contains("Pipe"))
            mesh.material = _pipes._openPipeMaterials[(int)color];
    }

    [MenuItem("Pipes/Count pipes")]
    static void CountPipes()
    {
        var pipes = new Dictionary<string, int>();
        TraversePipe(Selection.activeTransform, pipes);

        string res = "";
        foreach (var p in pipes)
            res += $"{p.Key}: {p.Value}\n";
        Debug.Log(res);
    }

    static void TraversePipe(Transform pipe, Dictionary<string, int> pipes)
    {
        CountPipe(pipe, pipes);
        for (int i = 0; i < pipe.childCount; i++)
            TraversePipe(pipe.GetChild(i), pipes);
    }


    static void CountPipes(Transform parent, Dictionary<string, int> pipes = null)
    {
        if (pipes == null)
            pipes = new Dictionary<string, int>();
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            CountPipe(parent.GetChild(i), pipes);
            CountPipes(child.transform, pipes);
        }

        string res = "";
        foreach (var pipe in pipes)
            res += $"{pipe.Key}: {pipe.Value}\n";
        Debug.Log(res);
    }

    static void CountPipe(Transform pipe, Dictionary<string, int> pipes)
    {
        if (pipe.gameObject.TryGetComponent<MeshFilter>(out var filter))
            AddPipe(pipes, filter.sharedMesh.name);
    }

    static void AddPipe(Dictionary<string, int> pipes, string pipeName)
    {
        if (!pipes.ContainsKey(pipeName))
            pipes.Add(pipeName, 0);
        pipes[pipeName]++;
    }
}
