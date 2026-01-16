#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;
using System.Threading;


// Animated radial wave with per-pixel color cycling using PXL.Simulate
var scene = () =>
{
    // Use simulator-provided canvas size so it's not hard-coded
    var width = Ctx.Width; // var -> Fügt den Wert einer Variable hinzu; width -> Beschreibt Eigenschaft
    var height = Ctx.Height;

    // Use current time from the simulator
    var now = Ctx.Now;
    var time = now.TimeOfDay.TotalSeconds;

    // ---- prepare small centered clock bounding box (HH:mm) ----
    var txt = now.ToString("HH:mm");
    const int charW = 3;         //const -> Wert zuweisen; int -> Variable ganze Zahl; charW -> Name Variable     
    const int charH = 5;        //konstante Ganzzahlvariable
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
    var clockExclusionRadius = maxClockDist + 1.6;

    // --- creative radial HSV background ---
    // Parameters tuned for soft gradients and no hard edges.
    var cxCenter = width / 2.0;
    var cyCenter = height / 2.0;
    var maxR = Math.Sqrt(cxCenter * cxCenter + cyCenter * cyCenter);
    var baseHue = (time * 0.06) % 1.0; // slow hue drift

    // draw per-pixel background, skipping pixels too close to the clock (radial exclusion)
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            // center of pixel
            var px = x + 0.5;
            var py = y + 0.5;
            var dx = px - cxCenter;
            var dy = py - cyCenter;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            // Skip pixels inside exclusion radius so clock stays untouched
            if (dist <= clockExclusionRadius)
                continue;

            var nr = dist / maxR; // normalized radius
            var angle = Math.Atan2(dy, dx); // -PI..PI
            var angNorm = (angle + Math.PI) / (2.0 * Math.PI); // 0..1

            // Radial ripple + outward motion
            var ripple = 0.5 + 0.5 * Math.Sin(dist * 3.2 - time * 1.6);

            // Hue shifts with radius and angle and a slow global drift
            var hue = (baseHue + nr * 0.32 + 0.08 * Math.Sin(time * 0.4 + nr * 6.0) + 0.06 * angNorm) % 1.0;
            var sat = 0.6 + 0.35 * (1.0 - nr);

            // value controlled by ripple and a smooth radial falloff
            var falloff = Math.Clamp(1.0 - Math.Pow(nr, 1.8), 0.0, 1.0);
            var val = 0.08 + 0.9 * (0.45 * ripple + 0.55 * falloff);

            // soft rim highlight (rotating)
            var rimFactor = Math.Exp(-Math.Max(0.0, (nr - 0.85) * 12.0));
            var rimPulse = 0.6 + 0.4 * Math.Sin(time * 3.2 + angNorm * 10.0);

            // mix hue slightly for rim rotation
            var rimHue = (hue + 0.15 * Math.Sin(time * 0.9 + nr * 5.0)) % 1.0;
            var finalHue = hue * (1 - rimFactor) + rimHue * rimFactor;
            var finalVal = Math.Clamp(val + 0.6 * rimFactor * rimPulse, 0.0, 1.0);

            // convert HSV -> RGB (simple conversion using float math mapped to bytes)
            double hh = finalHue * 6.0;
            int i = (int)Math.Floor(hh) % 6;
            double f = hh - Math.Floor(hh);
            double p = finalVal * (1 - sat);
            double q = finalVal * (1 - f * sat);
            double tcol = finalVal * (1 - (1 - f) * sat);
            double rd = 0, gd = 0, bd = 0;
            switch (i)
            {
                case 0: rd = finalVal; gd = tcol; bd = p; break;
                case 1: rd = q; gd = finalVal; bd = p; break;
                case 2: rd = p; gd = finalVal; bd = tcol; break;
                case 3: rd = p; gd = q; bd = finalVal; break;
                case 4: rd = tcol; gd = p; bd = finalVal; break;
                case 5: rd = finalVal; gd = p; bd = q; break;
            }

            var R = (byte)(Math.Clamp(rd, 0.0, 1.0) * 255);
            var G = (byte)(Math.Clamp(gd, 0.0, 1.0) * 255);
            var B = (byte)(Math.Clamp(bd, 0.0, 1.0) * 255);

            // paint pixel
            Ctx.RectXyWh(x, y, 1, 1).Fill.Solid(Color.FromArgb(255, R, G, B));
        }
    }

    // --- decorative rotating rim dots (outside clock) ---
    // These provide a tasteful motion cue near the edge and pulse over time.
    var rimCount = 12;
    for (int k = 0; k < rimCount; k++)
    {
        // angle rotates over time
        var a = time * 0.6 + k * (2.0 * Math.PI / rimCount);
        var rpos = maxR * (0.9 + 0.06 * Math.Sin(time * 1.8 + k)); // slight radial wobble
        var px = cxCenter + Math.Cos(a) * rpos;
        var py = cyCenter + Math.Sin(a) * rpos;

        var ix = (int)Math.Round(px - 0.5);
        var iy = (int)Math.Round(py - 0.5);

        // skip if inside clock exclusion or outside canvas
        var dxC = (ix + 0.5) - cxCenter;
        var dyC = (iy + 0.5) - cyCenter;
        var dC = Math.Sqrt(dxC * dxC + dyC * dyC);
        if (dC <= clockExclusionRadius) continue;
        if (ix < 0 || ix >= width || iy < 0 || iy >= height) continue;

        // color for rim dot — bright complementary hue to base
        var dotHue = (baseHue + 0.5 + k * 0.02) % 1.0;
        var dotSat = 0.85;
        var pulse = 0.6 + 0.4 * Math.Sin(time * 4.0 + k);
        var dotVal = 0.5 + 0.5 * pulse;

        // HSV->RGB small helper
        double dh = dotHue * 6.0;
        int di = (int)Math.Floor(dh) % 6;
        double df = dh - Math.Floor(dh);
        double dp = dotVal * (1 - dotSat);
        double dq = dotVal * (1 - df * dotSat);
        double dtc = dotVal * (1 - (1 - df) * dotSat);
        double rdd = 0, gdd = 0, bdd = 0;
        switch (di)
        {
            case 0: rdd = dotVal; gdd = dtc; bdd = dp; break;
            case 1: rdd = dq; gdd = dotVal; bdd = dp; break;
            case 2: rdd = dp; gdd = dotVal; bdd = dtc; break;
            case 3: rdd = dp; gdd = dq; bdd = dotVal; break;
            case 4: rdd = dtc; gdd = dp; bdd = dotVal; break;
            case 5: rdd = dotVal; gdd = dp; bdd = dq; break;
        }

        var dR = (byte)(Math.Clamp(rdd, 0.0, 1.0) * 255);
        var dG = (byte)(Math.Clamp(gdd, 0.0, 1.0) * 255);
        var dB = (byte)(Math.Clamp(bdd, 0.0, 1.0) * 255);

        Ctx.RectXyWh(ix, iy, 1, 1).Fill.Solid(Color.FromArgb(255, dR, dG, dB));
    }

    // --- draw small centered clock on top (no background) ---
    // Clock uses high-contrast white to remain readable against gradients
    Ctx.Text.Var3x5(txt, textX, textY).Brush.Solid(Colors.White);
};



// await PXL.Simulate(scene);

await PXL.SendToDevice(scene, "192.168.178.110");



