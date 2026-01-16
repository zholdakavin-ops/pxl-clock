#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

var scene = () =>
{
    for (var x = 0; x < Ctx.Width; x++)
    {
        for (var y = 0; y < Ctx.Height; y++)
        {
            // x und y stehen und hier zur Verfügung
            var brightness = x * 10;
            var color = Color.FromArgb(255, (byte)brightness, (byte)brightness, (byte)brightness);
            Ctx.Point(x, y).Stroke.Solid(color);
        }
    }
};

// await PXL.Simulate(scene);
await PXL.SendToDevice(scene, "192.168.178.110");
