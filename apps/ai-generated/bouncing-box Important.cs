#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// Small person jumping between two platforms
var mainScene = () =>
{
    // Dark green background
    Ctx.RectXyWh(0, 0, 24, 24).Fill.Solid(Color.FromArgb(255, 0, 80, 0));
    
    // Platform definitions
    var platform1X = 2.0;
    var platform1Y = 18.0;
    var platform2X = 14.0;
    var platform2Y = 12.0;
    var platformWidth = 6.0;
    var platformHeight = 2.0;
    
    // Draw platforms
    Ctx.RectXyWh(platform1X, platform1Y, platformWidth, platformHeight).Fill.Solid(Colors.Brown);
    Ctx.RectXyWh(platform2X, platform2Y, platformWidth, platformHeight).Fill.Solid(Colors.Brown);
    
    // Person dimensions (small rectangle)
    var personWidth = 2.0;
    var personHeight = 3.0;
    
    // Animation timing
    var cycleDuration = 3.0; // Total cycle in seconds
    var t = Ctx.Now.TimeOfDay.TotalSeconds % cycleDuration;
    
    // Jump parameters
    var jumpDuration = 1.5; // Time for the jump
    
    var personX = 0.0;
    var personY = 0.0;
    
    if (t < jumpDuration)
    {
        // Jumping from platform 1 to platform 2
        var progress = t / jumpDuration;
        
        // Horizontal movement (linear)
        var startX = platform1X + (platformWidth - personWidth) / 2.0;
        var endX = platform2X + (platformWidth - personWidth) / 2.0;
        personX = startX + (endX - startX) * progress;
        
        // Vertical movement (realistic parabolic arc)
        var startY = platform1Y - personHeight; // Standing on platform 1
        var endY = platform2Y - personHeight;   // Standing on platform 2
        var jumpHeight = 8.0; // Peak height above the starting platform
        
        // Parabolic arc: goes up then down
        // At progress=0: startY, at progress=0.5: peak, at progress=1: endY
        var arcHeight = -4.0 * jumpHeight * progress * (progress - 1.0); // Parabola peaking at 0.5
        personY = startY - arcHeight + (endY - startY) * progress;
    }
    else
    {
        // Standing on platform 2
        personX = platform2X + (platformWidth - personWidth) / 2.0;
        personY = platform2Y - personHeight;
    }
    
    // Draw person (small rectangle)
    Ctx.RectXyWh(personX, personY, personWidth, personHeight).Fill.Solid(Colors.Yellow);
};

// await PXL.Simulate(mainScene);
await PXL.SendToDevice(mainScene, "192.168.178.110");
