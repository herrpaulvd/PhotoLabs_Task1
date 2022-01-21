using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using Python.Runtime;

(dynamic, dynamic) Get2(dynamic value)
{
    dynamic[] arr = value;
    return (arr[0], arr[1]);
}
dynamic Tuple(params dynamic[] objs)
    => objs;

try
{
    Runtime.PythonDLL = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python39_64\python39.dll";
    using var _ = Py.GIL();

    dynamic np = Py.Import("numpy");
    dynamic plt = Py.Import("matplotlib.pyplot");

    var x = Enumerable.Range(-10, 21);
    var y = x.Select(ev =>
    {
        const string directory = @"..\..\..\..\pictures\";
        const int w = 3000;
        const int h = 4000;
        const int frameW = 32;
        const int frameH = 32;
        const int frameX = w / 2 - frameW / 2;
        const int frameY = h / 2 - (frameH / 2);

        using var bitmap = new Bitmap(directory + ev + ".jpg");
        var rangeX = Enumerable.Range(frameX, frameW);
        var rangeY = Enumerable.Range(frameY, frameH);
        var lightness = rangeX.SelectMany(
            x => rangeY.Select(
                y => { var px = bitmap.GetPixel(x, y); return (px.R + px.G + px.B) / 3.0; }))
        .Average();
        return Math.Log(lightness);
    });

    var (fig, ax) = Get2(plt.subplots());
    ax.plot(x.ToArray(), y.ToArray());
    ax.set(xlabel: "EV", ylabel: "Ln(яркость)", title: "Динамический диапазон камеры моего смартфона");
    ax.grid();
    fig.savefig("result.jpg");
    Console.WriteLine("Successful");
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
finally
{
    PythonEngine.Shutdown();
}

