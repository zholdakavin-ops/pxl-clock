#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// Main scene with dark green background and animated rectangle
var mainScene = () =>
{
    // Fill background with dark green
    Ctx.RectXyWh(0, 0, 24, 24).Fill.Solid(Color.FromArgb(255, 0, 100, 0));
    
    // Get current time for animation
    var t = Ctx.Now.TimeOfDay.TotalSeconds;
    
    // Animate X position from left (0) to right (16) - cycle repeats every 2 seconds
    var xPos = ((t * 12) % 24) - 4; // Range from -4 to 20, creating left-to-right motion
    
    // Draw animated lime rectangle
    Ctx.RectXyWh(xPos, 10, 8, 4).Fill.Solid(Colors.Lime);
};


// await PXL.Simulate(mainScene);
await PXL.SendToDevice(mainScene, "192.168.178.110");

