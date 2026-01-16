#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;
using System.Threading;


// Animated radial wave with per-pixel color cycling using PXL.Simulate
var scene = () =>
{
    int width = 24;
    int height = 24;

    // Use current time from the simulator
    var now = Ctx.Now;
    double time = now.TimeOfDay.TotalSeconds;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            double dx = x - width / 2.0;
            double dy = y - height / 2.0;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            double wave = Math.Sin(dist * 0.7 - time);
            double intensity = (wave + 1.0) / 2.0;

            byte r = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 0) * 127 + 128));
            byte g = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 2) * 127 + 128));
            byte b = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 4) * 127 + 128));

            r = (byte)(r * intensity);
            g = (byte)(g * intensity);
            b = (byte)(b * intensity);

            Ctx.RectXyWh(x, y, 1, 1).Fill.Solid(Color.FromArgb(255, r, g, b));
        }
    }
};


await PXL.Simulate(scene);

// await PXL.SendToDevice(scene, "DeviceIP_or_NameInNetwork");


