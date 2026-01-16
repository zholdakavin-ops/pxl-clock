// SkiaSharp not used to avoid Color type ambiguity
#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;
using System;
// no SkiaSharp import to avoid color type ambiguity

// Radial background animation with centered digital clock (HH:MM).
// - Clock is rendered with a transparent background and is centered.
// - Background animation runs only outside the clock bounding box.
// - Smooth HSV-based gradients, hue-shifting over time, soft radial waves,
//   and optional outer pulsation/rotation for the rim.

// Convert HSV (0..1) to RGB bytes. Caller chooses target color type to avoid type ambiguity.
(byte R, byte G, byte B) HsvToRgbBytes(double h, double s, double v)
{
    var hh = h * 6.0;
    var i = (int)Math.Floor(hh) % 6;
    var f = hh - Math.Floor(hh);
    var p = v * (1 - s);
    var q = v * (1 - f * s);
    var t = v * (1 - (1 - f) * s);

    double rd = 0, gd = 0, bd = 0;
    switch (i)
    {
        case 0: rd = v; gd = t; bd = p; break;
        case 1: rd = q; gd = v; bd = p; break;
        case 2: rd = p; gd = v; bd = t; break;
        case 3: rd = p; gd = q; bd = v; break;
        case 4: rd = t; gd = p; bd = v; break;
        case 5: rd = v; gd = p; bd = q; break;
    }

    byte R = (byte)(Math.Clamp(rd, 0.0, 1.0) * 255);
    byte G = (byte)(Math.Clamp(gd, 0.0, 1.0) * 255);
    byte B = (byte)(Math.Clamp(bd, 0.0, 1.0) * 255);
    return (R, G, B);
}

var scene = () =>
{
    var w = Ctx.Width;            // expected 24
    var h = Ctx.Height;           // expected 24
    var now = Ctx.Now;
    var t = now.TimeOfDay.TotalSeconds;

    // --- Clock text setup (separate concern) ---
    // Use a very compact font and tight spacing so the full `HH:mm` fits small and centered.
    var txt = now.ToString("HH:mm");
    const int charW = 3;   // Var3x5 glyph width
    const int charH = 5;   // Var3x5 glyph height
    const int charSpacing = 0; // no extra spacing to keep it compact
    var charCount = txt.Length;
    var textWidth = charCount * charW + Math.Max(0, charCount - 1) * charSpacing; // tight total width
    var textHeight = charH;
    // center the tiny text precisely
    var textX = (int)Math.Round((w - textWidth) / 2.0);
    var textY = (int)Math.Round((h - textHeight) / 2.0);

    // Bounding rectangle of the clock — background animation must NOT draw here.
    // Add a 1px padding around the glyphs to avoid any overlap with anti-aliased strokes.
    var clockRectX1 = Math.Max(0, textX - 1);
    var clockRectY1 = Math.Max(0, textY - 1);
    var clockRectX2 = Math.Min(w - 1, textX + textWidth);
    var clockRectY2 = Math.Min(h - 1, textY + textHeight);

    // --- Background / radial animation (outside clock bounding box) ---
    var cx = w / 2.0;
    var cy = h / 2.0;
    var maxR = Math.Sqrt(cx * cx + cy * cy);

    // Base hue that slowly shifts over time
    var baseHue = (t * 0.02) % 1.0; // slow global hue rotation

    for (int yy = 0; yy < h; yy++)
    {
        for (int xx = 0; xx < w; xx++)
        {
            // Skip pixels inside clock bounding box to keep the clock area transparent
            if (xx >= clockRectX1 && xx <= clockRectX2 && yy >= clockRectY1 && yy <= clockRectY2)
                continue;

            // Radial distance from center
            var dx = xx + 0.5 - cx; // center of pixel
            var dy = yy + 0.5 - cy;
            var r = Math.Sqrt(dx * dx + dy * dy);
            var nr = r / maxR; // normalized radius 0..1

            // Expanding wave that moves outward over time
            var waveSpeed = 1.8; // controls expansion speed
            var wave = 0.5 + 0.5 * Math.Sin(r * 2.5 - t * waveSpeed);

            // Smooth radial falloff so center is softer
            var falloff = 1.0 - Math.Pow(nr, 1.5);
            falloff = Math.Clamp(falloff, 0.0, 1.0);

            // Outer rim pulsation and slight rotation effect
            var rim = Math.Exp(-Math.Max(0.0, (nr - 0.8) * 8.0)); // near edges
            var rimPulse = 0.6 + 0.4 * Math.Sin(t * 3.0 + r * 3.0);

            // Compute hue per-pixel: base hue + radius offset + small angular shift
            var angle = Math.Atan2(dy, dx); // -PI..PI
            var angNorm = (angle + Math.PI) / (2 * Math.PI); // 0..1
            var hue = (baseHue + nr * 0.25 + angNorm * 0.05 + 0.1 * Math.Sin(t * 0.4)) % 1.0;

            // Saturation and value controlled by wave + falloff, keep smooth
            var sat = 0.6 + 0.4 * wave;
            var val = 0.12 + 0.88 * (0.4 * wave + 0.6 * falloff * rimPulse * rim);

            // Make outermost pixels slightly brighter and rotating hue
            var rotHue = (hue + 0.2 * Math.Sin(t * 0.8 + nr * 6.0)) % 1.0;
            var mixHue = hue * (1 - rim) + rotHue * rim;

            var (rB, gB, bB) = HsvToRgbBytes(mixHue, sat, val);
            var pxCol = Pxl.Ui.CSharp.Color.FromArgb(255, rB, gB, bB);

            Ctx.RectXyWh(xx, yy, 1, 1).Fill.Solid(pxCol);
        }
    }

    // --- Clock rendering (draw AFTER background so it sits on top). ---
    // Use a contrasting color calculated from the complementary hue for readability.
    var clockHue = (baseHue + 0.5) % 1.0;
    var (cr, cg, cb) = HsvToRgbBytes(clockHue, 0.0, 1.0);
    var clockColor = Pxl.Ui.CSharp.Color.FromArgb(255, cr, cg, cb);

    // Render text with VAR 3x5 font and no background (transparent).
    Ctx.Text.Var3x5(txt, textX, textY).Brush.Solid(clockColor);
};

await PXL.Simulate(scene);

// To send to a real device uncomment and replace target name or IP:
// await PXL.SendToDevice(scene, "DeviceIP_or_NameInNetwork");
