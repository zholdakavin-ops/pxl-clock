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

    // ---- prepare small centered clock bounding box (HH:mm) ----
    var txt = now.ToString("HH:mm");
    const int charW = 3;
    const int charH = 5;
    const int charSpacing = 0;
    var charCount = txt.Length;
    var textWidth = charCount * charW + Math.Max(0, charCount - 1) * charSpacing;
    var textHeight = charH;
    var textX = (int)Math.Round((width - textWidth) / 2.0);
    var textY = (int)Math.Round((height - textHeight) / 2.0);

    var clockRectX1 = Math.Max(0, textX - 1);
    var clockRectY1 = Math.Max(0, textY - 1);
    var clockRectX2 = Math.Min(width - 1, textX + textWidth);
    var clockRectY2 = Math.Min(height - 1, textY + textHeight);

    // Compute a radial exclusion radius that fully contains the clock rectangle
    var cx = width / 2.0;
    var cy = height / 2.0;
    double maxClockDist = 0.0;
    for (int yy = clockRectY1; yy <= clockRectY2; yy++)
        for (int xx = clockRectX1; xx <= clockRectX2; xx++)
        {
            var ddx = (xx + 0.5) - cx;
            var ddy = (yy + 0.5) - cy;
            var d = Math.Sqrt(ddx * ddx + ddy * ddy);
            if (d > maxClockDist) maxClockDist = d;
        }
    // Add padding so animation won't visually touch the digits (prevents antialias/bleed)
    var clockExclusionRadius = maxClockDist + 1.5;

    // ---- background: radial wave, but SKIP any pixel whose center is within the exclusion radius ----
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            var dx = x - width / 2.0; // Distanz (horizontal) des "aktuellen" Punktes zum Mittelpunkt
            var dy = y - height / 2.0; // Distanz (vertikal) des "aktuellen" Punktes zum Mittelpunkt

            // Satz des Pythagoras
            var distToCenter = Math.Sqrt(dx * dx + dy * dy);

            // If this pixel is within the protective radius around the clock, skip it.
            if (distToCenter < clockExclusionRadius)
                continue;

            var wave = Math.Sin(distToCenter * 0.7 - time);
            var intensity = (wave + 1.0) / 2.0;

            var rr = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 0) * 127 + 128));
            var gg = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 2) * 127 + 128));
            var bb = (byte)(Math.Max(0, Math.Sin(time + x * 0.3 + 4) * 127 + 128));

            rr = (byte)(rr * intensity);
            gg = (byte)(gg * intensity);
            bb = (byte)(bb * intensity);

            Ctx.Point(x, y).Stroke.Solid(Color.FromArgb(255, rr, gg, bb));
        }
    }

    // --- draw small centered clock on top (no background) ---
    Ctx.Text.Var3x5(txt, textX, textY).Brush.Solid(Colors.White);
};



// await PXL.Simulate(scene);

await PXL.SendToDevice(scene, "192.168.178.110"); 

