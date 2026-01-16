#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;
using System.Threading;


// Animated radial wave with per-pixel color cycling using PXL.Simulate
var scene = () =>
{
    // Use simulator-provided canvas size so it's not hard-coded
    var width = Ctx.Width;
    var height = Ctx.Height;

    // Use current time from the simulator
    var now = Ctx.Now;
    var time = now.TimeOfDay.TotalSeconds;

    // ---- background: keep your current animated radial wave per-pixel ----
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            var dx = x - width / 2.0;
            var dy = y - height / 2.0;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            var wave = Math.Sin(dist * 0.7 - time);
            var intensity = (wave + 1.0) / 2.0;

            var rr = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 0) * 127 + 128));
            var gg = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 2) * 127 + 128));
            var bb = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 4) * 127 + 128));

            rr = (byte)(rr * intensity);
            gg = (byte)(gg * intensity);
            bb = (byte)(bb * intensity);

            Ctx.RectXyWh(x, y, 1, 1).Fill.Solid(Color.FromArgb(255, rr, gg, bb));
        }
    }

    // mask out the center: draw a filled black circle so the middle is empty
    var cx = Ctx.Width / 2.0;
    var cy = Ctx.Height / 2.0;
    var innerRadius = Math.Min(Ctx.Width, Ctx.Height) / 2.0 - 4.0; // leave colored border
    if (innerRadius > 0)
        Ctx.Circle(cx, cy, innerRadius).Fill.Solid(Colors.Black);
};



// await PXL.Simulate(scene);

await PXL.SendToDevice(scene, "192.168.178.110");



