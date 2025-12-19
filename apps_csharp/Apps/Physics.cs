#:package Pxl@0.0.31

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// parameters
var springStrength = 50.0;   // Strong coupling between neighbors
var groundSpring = 2.5;      // Weak ground anchor (allows wave to spread)
var damping = 0.99;         // Very light damping for sustained waves
var mass = 1.0;

var dropHeight = 20.0;
var dropTimeInterval = 5.0;
var dropEaseInDuration = 0.5;
var dropStayDuration = 0.2;
var dropEaseOutDuration = 1.5;

var baseBrightness = 0.3;   // Base brightness (0-1)
var brightnessFactor = 0.03; // How much height affects brightness
var velocityToSaturation = 0.2; // How much velocity affects saturation



const int DisplaySize = 24;
const int PhysicalSize = 44;
const int Offset = 10;

var random = new Random();
var elapsedTime = 0.0;
var lastDropTime = -dropTimeInterval;  // Trigger first drop immediately

// Active drops: (x, y, startTime)
var drops = new List<(int X, int Y, double StartTime)>();

var grid = new (double Height, double Velocity, double RestHeight)[PhysicalSize, PhysicalSize];
var newGrid = new (double Height, double Velocity, double RestHeight)[PhysicalSize, PhysicalSize];

// Ease function (smooth step)
var easeInOut = (double t) => t * t * (3 - 2 * t);

var getDropHeight = (double startTime) =>
{
    var age = elapsedTime - startTime;
    var totalDuration = dropEaseInDuration + dropStayDuration + dropEaseOutDuration;
    
    if (age < 0 || age > totalDuration)
        return 0.0;
    
    if (age < dropEaseInDuration)
    {
        // Ease in
        var t = age / dropEaseInDuration;
        return easeInOut(t) * dropHeight;
    }
    else if (age < dropEaseInDuration + dropStayDuration)
    {
        // Stay
        return dropHeight;
    }
    else
    {
        // Ease out
        var t = (age - dropEaseInDuration - dropStayDuration) / dropEaseOutDuration;
        return (1 - easeInOut(t)) * dropHeight;
    }
};

var createDrop = () =>
{
    var x = Offset + random.Next(4, DisplaySize - 4);
    var y = Offset + random.Next(4, DisplaySize - 4);
    drops.Add((x, y, elapsedTime));
};

var getHeight = (int x, int y) =>
    x < 0 || x >= PhysicalSize || y < 0 || y >= PhysicalSize
        ? 0.0
        : grid[x, y].Height;

// Check if position has an active drop
var getActiveDropHeight = (int x, int y) =>
{
    foreach (var drop in drops)
    {
        if (drop.X == x && drop.Y == y)
            return getDropHeight(drop.StartTime);
    }
    return (double?)null;
};

var updatePhysics = (double dt) =>
{
    // First pass: calculate new velocities and heights based on current state
    for (var x = 0; x < PhysicalSize; x++)
    {
        for (var y = 0; y < PhysicalSize; y++)
        {
            var entity = grid[x, y];
            
            // Check if this is an active drop position
            var activeHeight = getActiveDropHeight(x, y);
            if (activeHeight.HasValue)
            {
                // Keep the entity at the forced height, but preserve velocity for smooth release
                newGrid[x, y] = (activeHeight.Value, entity.Velocity, 0.0);
                continue;
            }

            // Spring forces from neighbors
            // If a neighbor is higher than us, it pulls us up (positive force)
            // If a neighbor is lower than us, we get pulled down (negative force)
            var springForce = 0.0;
            springForce += getHeight(x - 1, y) - entity.Height;
            springForce += getHeight(x + 1, y) - entity.Height;
            springForce += getHeight(x, y - 1) - entity.Height;
            springForce += getHeight(x, y + 1) - entity.Height;
            springForce *= springStrength;

            // Ground spring pulls entity back to rest height
            var groundForce = groundSpring * (entity.RestHeight - entity.Height);

            var totalForce = springForce + groundForce;
            var acceleration = totalForce / mass;
            var newVelocity = (entity.Velocity + acceleration * dt) * damping;
            var newHeight = entity.Height + newVelocity * dt;

            newGrid[x, y] = (newHeight, newVelocity, entity.RestHeight);
        }
    }

    // Second pass: copy new state back to grid
    for (var x = 0; x < PhysicalSize; x++)
    {
        for (var y = 0; y < PhysicalSize; y++)
        {
            grid[x, y] = newGrid[x, y];
        }
    }
};

var scene = () =>
{
    var dt = 1.0 / 30.0;
    elapsedTime += dt;

    // Create new drop if interval passed
    if (elapsedTime - lastDropTime >= dropTimeInterval)
    {
        createDrop();
        lastDropTime = elapsedTime;
    }

    // Remove finished drops
    var totalDropDuration = dropEaseInDuration + dropStayDuration + dropEaseOutDuration;
    drops.RemoveAll(d => elapsedTime - d.StartTime > totalDropDuration);

    updatePhysics(dt);

    // Display only the middle 24x24 square
    for (var x = 0; x < DisplaySize; x++)
    {
        for (var y = 0; y < DisplaySize; y++)
        {
            var cell = grid[x + Offset, y + Offset];
            var height = cell.Height;
            var velocity = cell.Velocity;

            // Hue based on height (blue=low, cyan, green, yellow, red=high)
            var hue = (height + 10) * 12;  // Map height to hue (0-360)
            hue = ((hue % 360) + 360) % 360; // Wrap around

            // Saturation based on velocity magnitude
            var saturation = Math.Clamp(Math.Abs(velocity) * velocityToSaturation, 0.3, 1.0);

            // Lightness based on height with base brightness
            var lightness = Math.Clamp(baseBrightness + height * brightnessFactor, 0.1, 0.9);

            // HSL to RGB conversion
            var c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            var hPrime = hue / 60.0;
            var xVal = c * (1 - Math.Abs(hPrime % 2 - 1));
            var m = lightness - c / 2;

            double r1, g1, b1;
            if (hPrime < 1) { r1 = c; g1 = xVal; b1 = 0; }
            else if (hPrime < 2) { r1 = xVal; g1 = c; b1 = 0; }
            else if (hPrime < 3) { r1 = 0; g1 = c; b1 = xVal; }
            else if (hPrime < 4) { r1 = 0; g1 = xVal; b1 = c; }
            else if (hPrime < 5) { r1 = xVal; g1 = 0; b1 = c; }
            else { r1 = c; g1 = 0; b1 = xVal; }

            var r = (byte)Math.Clamp((r1 + m) * 255, 0, 255);
            var g = (byte)Math.Clamp((g1 + m) * 255, 0, 255);
            var b = (byte)Math.Clamp((b1 + m) * 255, 0, 255);

            var pixelIndex = x * DisplaySize + y;
            Ctx.Pixels[pixelIndex] = Color.FromRgb(r, g, b);
        }
    }
};


// await PXL.Simulate(scene);
await PXL.SendToDevice(scene, "192.168.178.52");
