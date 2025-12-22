#:package Pxl@0.0.34

using Pxl.Ui.CSharp; 
using static Pxl.Ui.CSharp.DrawingContext;

// === Physics parameters ===
var springStrength = 20.0;          // Stiffness of springs between neighbors (higher = faster wave propagation)
var groundSpringStiffness = 0.5;    // Stiffness of spring to ground (higher = waves dampen spatially faster)
var damping = 0.99;                 // Velocity damping per frame, 0-1 (lower = waves die out faster)
var mass = 0.3;                     // Entity mass (higher = slower/sluggish waves, lower = faster/snappier)

// === Drop parameters ===
var dropSpawnRect = (x: 3, y: 3, width: 18, height: 10);  // Rect within display (x,y=top-left corner)
var dropHeight = 20.0;              // Max height of drop (should match maxDisplayHeight for full brightness)
var dropEaseInDuration = 10.5;      // Seconds to rise to dropHeight
var dropStayDuration = 5;           // Seconds to hold at dropHeight
var dropEaseOutDuration = 0.5;      // Seconds to fall back to 0
var dropTimeMode = 1;               // 0=diff (interval after drop ends), 1=abs (fixed interval)
var dropTimeIntervalOrDiff = 10.0;  // Seconds between drops (mode 1) or after drop ends (mode 0)

// === Display parameters ===
var baseBrightness = 0.15;          // Lightness at height=0 (0-1)
var maxDisplayHeight = 10.0;        // Height that maps to maxLightness (negative = invert)
                                    // lower values = more contrast, higher values = more subtle
var velocityToSaturation = 0.5;     // Multiplier: velocity → saturation (higher = more color at low speeds)
var hueOffset = 10.0;               // Acceleration offset for hue (mode 0 only)
var hueScale = 0.0;                 // 0-1: acceleration sensitivity for hue (mode 0 only)
var baseHue = 0.0;                  // Fixed hue 0-360 (used when hueMode > 2)
var hueRange = (min: 180.0, max: 220.0);       // Hue range (min at low, max at high value)
var saturationRange = (min: 0.0, max: 1.0);   // Saturation range: min at rest, max at high velocity
var lightnessRange = (min: 0.0, max: 1.0);    // Lightness range: min=floor, max=ceiling for height mapping

// Hue mode:
//   0 = Acceleration: rainbow based on acceleration (uses hueOffset, hueScale)
//   1 = HeightSign: smooth hue based on height (uses maxDisplayHeight for range)
//   2 = VelocityDirection: smooth hue based on velocity
//   3 = Monochrome: grayscale (saturation forced to 0)
//   other = fixed baseHue
var hueMode = 1;



const int DisplaySize = 24;
const int PhysicalSize = 44;
const int Offset = 10;

var random = new Random();
var elapsedTime = 0.0;
var lastDropTime = double.MinValue;  // Trigger first drop immediately

// Calculated: how much height affects brightness
var brightnessFactor = (lightnessRange.max - baseBrightness) / maxDisplayHeight;

// Active drops: (x, y, startTime, initialHeight)
var drops = new List<(int X, int Y, double StartTime, double InitialHeight)>();

var grid = new (double height, double velocity, double acceleration)[PhysicalSize, PhysicalSize];
var newGrid = new (double height, double velocity, double acceleration)[PhysicalSize, PhysicalSize];

// Ease function (smooth step)
var easeInOut = (double t) => t * t * (3 - 2 * t);

var getDropHeight = (double startTime, double initialHeight) =>
{
    var age = elapsedTime - startTime;
    var totalDuration = dropEaseInDuration + dropStayDuration + dropEaseOutDuration;

    if (age < 0 || age > totalDuration)
        return 0.0;

    if (age < dropEaseInDuration)
    {
        // Ease in from initial height to dropHeight
        var t = age / dropEaseInDuration;
        return initialHeight + easeInOut(t) * (dropHeight - initialHeight);
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
    // x controls row, y controls column in the pixel layout
    var x = Offset + dropSpawnRect.y + random.Next(0, dropSpawnRect.height);  // row
    var y = Offset + dropSpawnRect.x + random.Next(0, dropSpawnRect.width);   // column
    var initialHeight = grid[x, y].height;
    drops.Add((x, y, elapsedTime, initialHeight));
};

// Get height at position, accounting for active drops
var getHeight = (int x, int y) =>
{
    if (x < 0 || x >= PhysicalSize || y < 0 || y >= PhysicalSize)
        return 0.0;
    
    // Check for active drop at this position
    foreach (var drop in drops)
    {
        if (drop.X == x && drop.Y == y)
            return getDropHeight(drop.StartTime, drop.InitialHeight);
    }
    
    return grid[x, y].height;
};

// Check if position has an active drop
var getActiveDropHeight = (int x, int y) =>
{
    foreach (var drop in drops)
    {
        if (drop.X == x && drop.Y == y)
            return getDropHeight(drop.StartTime, drop.InitialHeight);
    }
    return (double?)null;
};

var updatePhysics = (double dt) =>
{
    // First pass: calculate new velocities and heights based on current state
    foreach (var (x, y) in Grid(PhysicalSize))
    {
        var entity = grid[x, y];

        // Check if this is an active drop position
        var activeHeight = getActiveDropHeight(x, y);
        if (activeHeight.HasValue)
        {
            // Keep the entity at the forced height, but preserve velocity for smooth release
            newGrid[x, y] = (activeHeight.Value, entity.velocity, 0.0);
            continue;
        }

        // Spring forces from neighbors
        // If a neighbor is higher than us, it pulls us up (positive force)
        // If a neighbor is lower than us, we get pulled down (negative force)
        var springForce = 0.0;
        springForce += getHeight(x - 1, y) - entity.height;
        springForce += getHeight(x + 1, y) - entity.height;
        springForce += getHeight(x, y - 1) - entity.height;
        springForce += getHeight(x, y + 1) - entity.height;
        springForce *= springStrength;

        // Ground spring pulls entity back to rest height (0)
        var groundForce = groundSpringStiffness * (0.0 - entity.height);

        var totalForce = springForce + groundForce;
        var acceleration = totalForce / mass;
        var newVelocity = (entity.velocity + acceleration * dt) * damping;
        var newHeight = entity.height + newVelocity * dt;

        var measuredAcceleration = (newVelocity - entity.velocity) / dt;
        newGrid[x, y] = (newHeight, newVelocity, measuredAcceleration);
    }

    // Second pass: copy new state back to grid
    foreach (var (x, y) in Grid(PhysicalSize))
        grid[x, y] = newGrid[x, y];
};

var scene = () =>
{
    var dt = 1.0 / 30.0;
    elapsedTime += dt;

    // Create new drop based on timing mode
    var totalDropDuration = dropEaseInDuration + dropStayDuration + dropEaseOutDuration;
    var spawnInterval = dropTimeMode == 0 
        ? totalDropDuration + dropTimeIntervalOrDiff  // diff: wait for drop to finish + interval
        : dropTimeIntervalOrDiff;                      // abs: fixed interval
    
    if (elapsedTime - lastDropTime >= spawnInterval)
    {
        createDrop();
        lastDropTime = elapsedTime;
    }

    // Remove finished drops
    drops.RemoveAll(d => elapsedTime - d.StartTime > totalDropDuration);

    updatePhysics(dt);

    // Display only the middle 24x24 square
    foreach (var (x, y) in Grid(DisplaySize))
    {
        var gx = x + Offset;
        var gy = y + Offset;
        var cell = grid[gx, gy];
        var height = cell.height;
        var velocity = cell.velocity;
        var acceleration = cell.acceleration;

        // Hue based on selected mode
        var hue = hueMode switch
        {
            0 => ((acceleration + hueOffset) * hueScale * 360 % 360 + 360) % 360,  // Acceleration
            1 => hueRange.min + (hueRange.max - hueRange.min) * Math.Clamp((height / maxDisplayHeight + 1) / 2, 0, 1),
            2 => hueRange.min + (hueRange.max - hueRange.min) * Math.Clamp((velocity / 50.0 + 1) / 2, 0, 1),
            _ => baseHue
        };

        // Saturation based on velocity magnitude (0 for monochrome mode)
        var saturation = hueMode == 3 
            ? 0.0 
            : Math.Clamp(Math.Abs(velocity) * velocityToSaturation, saturationRange.min, saturationRange.max);

        // Lightness based on height with base brightness
        var lightness = Math.Clamp(baseBrightness + height * brightnessFactor, lightnessRange.min, lightnessRange.max);

        var pixelIndex = x * DisplaySize + y;
        Ctx.Pixels[pixelIndex] = Color.FromHsl360(hue, saturation, lightness);
    }
};


// await PXL.Simulate(scene);
// await PXL.SendToDevice(scene, "192.168.178.52");
await PXL.SimulateAndSendToDevice(scene, "192.168.178.52");

