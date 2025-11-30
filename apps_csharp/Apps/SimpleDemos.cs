using System;
using System.Threading;
using System.Threading.Tasks;
using Pxl.Ui.CSharp;
using SkiaSharp;
using static Pxl.Ui.CSharp.Drawing;

static class SimpleDemos
{
    // Scene 1: Line demonstration - diagonal cross
    public static Action LineScene = () =>
    {
        // Diagonal line from top-left to bottom-right
        Ctx.Line(0, 0, 24, 24).Stroke.Solid(Color.Cyan);

        // Diagonal line from top-right to bottom-left
        Ctx.Line(24, 0, 0, 24).Stroke.Solid(Color.Magenta);

        // Horizontal center line
        Ctx.Line(0, 12, 24, 12).Stroke.Solid(Color.Yellow);

        // Vertical center line
        Ctx.Line(12, 0, 12, 24).Stroke.Solid(Color.Lime);
    };

    // Scene 2: Rectangle demonstration
    public static Action RectScene = () =>
    {
        // Outer rectangle with stroke only
        Ctx.RectXyWh(2, 2, 20, 20)
            .Stroke.Solid(Color.Red, strokeWidth: 2);

        // Inner filled rectangle
        Ctx.RectXyWh(8, 8, 8, 8);
        Ctx.RectXyWh(8, 8, 8, 8).Fill.Solid(Color.Blue);
        Ctx.RectXyWh(8, 8, 8, 8).Stroke.Solid(Color.White);

        // Alternative way using DrawRect2 (x1, y1, x2, y2)
        Ctx.RectXyXy(4, 4, 10, 10).Fill.Solid(Color.Green);
        Ctx.RectXyXy(4, 4, 10, 10).Stroke.Solid(Color.Yellow);
    };

    // Scene 3: Circle demonstration
    public static Action CircleScene = () =>
    {
        // Large circle with stroke only
        Ctx.Circle(12, 12, 10)
            .Stroke.Solid(Color.Cyan, strokeWidth: 2);

        // Medium filled circle
        Ctx.Circle(12, 12, 7).Fill.Solid(Color.Magenta);
        Ctx.Circle(12, 12, 7).Stroke.Solid(Color.Yellow);

        // Small center circle
        Ctx.Circle(12, 12, 3).Fill.Solid(Color.White);
    };

    // Scene 4: Combined demonstration - face
    public static Action CombinedScene = () =>
    {
        // Face outline (circle)
        Ctx.Circle(12, 12, 10).Fill.Solid(Color.Yellow);
        Ctx.Circle(12, 12, 10).Stroke.Solid(Color.Black);

        // Left eye
        Ctx.Circle(9, 10, 1.5).Fill.Solid(Color.Black);

        // Right eye
        Ctx.Circle(15, 10, 1.5).Fill.Solid(Color.Black);

        // Smile (using a rectangle for simplicity)
        Ctx.RectXyWh(8, 15, 8, 2).Fill.Solid(Color.Red);
        Ctx.RectXyWh(8, 15, 8, 2).Stroke.Solid(Color.Black);
    };

    // Scene 5: Grid pattern
    public static Action GridScene = () =>
    {
        // Vertical lines
        for (int x = 0; x <= 24; x += 4)
            Ctx.Line(x, 0, x, 24).Stroke.Solid(Color.DarkGray);

        // Horizontal lines
        for (int y = 0; y <= 24; y += 4)
            Ctx.Line(0, y, 24, y).Stroke.Solid(Color.DarkGray);

        // Corner markers
        Ctx.Circle(0, 0, 2).Fill.Solid(Color.Red);
        Ctx.Circle(24, 0, 2).Fill.Solid(Color.Green);
        Ctx.Circle(0, 24, 2).Fill.Solid(Color.Blue);
        Ctx.Circle(24, 24, 2).Fill.Solid(Color.Yellow);
    };

    // Scene 6: Linear Gradient demonstration
    public static Action LinearGradientScene = () =>
    {
        // Horizontal gradient rectangle
        Ctx.RectXyWh(2, 2, 20, 8).Fill.HorizontalGradient(20, Color.Red, Color.Yellow, Color.Green);

        // Vertical gradient rectangle
        Ctx.RectXyWh(2, 14, 20, 8).Fill.VerticalGradient(8, Color.Blue, Color.Cyan, Color.White);

        // Diagonal gradient circle
        Ctx.Circle(12, 12, 10).Fill.LinearGradient(
            (2, 2),
            (22, 22),
            [Color.Magenta, Color.Purple, Color.Blue]
        );
    };

    // Scene 7: Radial Gradient demonstration
    public static Action RadialGradientScene = () =>
    {
        // Background rectangle with radial gradient
        Ctx.RectXyWh(0, 0, 24, 24).Fill.RadialGradient(
            (12, 12),
            12,
            [Color.Yellow, Color.Orange, Color.Red, Color.DarkRed]
        );

        // Overlapping circles with radial gradients
        Ctx.Circle(8, 8, 6).Fill.RadialGradient(
            (8, 8),
            6,
            [Color.White, Color.Cyan, Color.Blue]
        );

        Ctx.Circle(16, 16, 6).Fill.RadialGradient(
            (16, 16),
            6,
            [Color.White, Color.Lime, Color.Green]
        );
    };

    // Scene 8: Sweep Gradient demonstration
    public static Action SweepGradientScene = () =>
    {
        // Rainbow sweep gradient circle
        Ctx.Circle(12, 12, 10).Fill.SweepGradient(
            (12, 12),
            [Color.Red, Color.Yellow, Color.Green, Color.Cyan, Color.Blue, Color.Magenta, Color.Red]
        );

        // Small center circle
        Ctx.Circle(12, 12, 3).Fill.Solid(Color.White);
    };

    // Scene 9: Mixed gradients
    public static Action MixedGradientsScene = () =>
    {
        // Background with vertical gradient
        Ctx.RectXyWh(0, 0, 24, 24).Fill.VerticalGradient(24, Color.DarkBlue, Color.Blue, Color.Cyan);

        // Sun with radial gradient
        Ctx.Circle(6, 6, 4).Fill.RadialGradient(
            (6, 6),
            4,
            [Color.Yellow, Color.Orange]
        );

        // Bottom rectangle with horizontal gradient
        Ctx.RectXyWh(0, 18, 24, 6).Fill.HorizontalGradient(24, Color.Green, Color.DarkGreen, Color.Green);
    };

    // Scene 10: Gradient Smiley Face
    public static Action GradientSmileyScene = () =>
    {
        // Background with radial gradient (light to dark)
        Ctx.RectXyWh(0, 0, 24, 24).Fill.RadialGradient(
            (12, 12),
            17,
            [Color.LightBlue, Color.DarkBlue]
        );

        // Face with radial gradient (yellow to orange)
        Ctx.Circle(12, 12, 10).Fill.RadialGradient(
            (10, 10),
            12,
            [Color.Yellow, Color.Gold, Color.Orange]
        );

        // Face outline
        Ctx.Circle(12, 12, 10).Stroke.Solid(Color.DarkOrange, strokeWidth: 1);

        // Left eye with gradient
        Ctx.Circle(9, 10, 1.5).Fill.RadialGradient(
            (9, 10),
            1.5f,
            [Color.White, Color.Blue, Color.Black]
        );

        // Right eye with gradient
        Ctx.Circle(15, 10, 1.5).Fill.RadialGradient(
            (15, 10),
            1.5f,
            [Color.White, Color.Blue, Color.Black]
        );

        // Smile with gradient (red to dark red)
        Ctx.RectXyWh(8, 15, 8, 2.5).Fill.VerticalGradient(
            2.5f,
            Color.Red, Color.DarkRed
        );

        // Smile outline
        Ctx.RectXyWh(8, 15, 8, 2.5).Stroke.Solid(Color.Maroon);

        // Rosy cheeks with radial gradients
        Ctx.Circle(7, 13, 1.5).Fill.RadialGradient(
            (7, 13),
            1.5f,
            [Color.Pink, Color.LightPink, new SKColor(255, 192, 203, 0)]
        );

        Ctx.Circle(17, 13, 1.5).Fill.RadialGradient(
            (17, 13),
            1.5f,
            [Color.Pink, Color.LightPink, new SKColor(255, 192, 203, 0)]
        );
    };

    // Scene 11: PacMan Game Scene
    public static Action PacManScene = () =>
    {
        // PacMan - yellow arc using center and radius (270 degrees, mouth open 45 degrees on each side)
        Ctx.ArcCenter(12, 12, 6, 45, 270).Fill.Solid(Color.Yellow, isAntialias: false);
    };

    // Scene 12: Text demonstration
    public static Action TextScene = () =>
    {
        // Title with different fonts
        Ctx.Text.Mono6x6("FONTS", 0, 0).Brush.Solid(Color.White);

        // Small fonts
        Ctx.Text.Var3x5("3x5", 0, 7).Brush.Solid(Color.Cyan);
        Ctx.Text.Mono4x5("4x5", 0, 13).Brush.Solid(Color.Yellow);

        // Medium font with gradient
        Ctx.Text.Mono6x6("Hi!", 0, 18).Brush.HorizontalGradient(18, Color.Red, Color.Orange, Color.Yellow);
    };

    // Scene 13: Text with graphics combined
    public static Action TextGraphicsScene = () =>
    {
        // Score display
        Ctx.Text.Mono4x5("SCORE", 1, 1).Brush.Solid(Color.White);
        Ctx.Text.Mono6x6("PXL", 1, 7).Brush.Solid(Color.Yellow);

        Ctx.Text.Var3x5("PAC", 12, 16).Brush.Solid(Color.White);
        Ctx.Text.Var3x5("MAN", 12, 20).Brush.Solid(Color.White);
    };
}
