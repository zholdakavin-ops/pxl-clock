#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;



var scene = () =>
{
    var color = Colors.White;

    for (var x = 0; x < Ctx.Width; x++)
    {
        for (var y = 0; y < Ctx.Height; y++)
        {
            // wenn x und y gerade Zahlen sind, dann ...
            // Modulo-Division:   %  - der Rest einer Division
            // &&  -  logische Verundung
            // ==  -  Vergleichs-Operator
            if (x % 4 == 0 && y % 2 == 0)
            {
                Ctx.Point(x, y).Stroke.Solid(color);
            }
        }
    }
};

// await PXL.Simulate(scene);
await PXL.SendToDevice(scene, "192.168.178.110");
