#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;


// Das hier wird 40 mal pro Sekunde aufgerufen
var scene = () =>
{
    // links oben
    Ctx
        .Point(0, 0)
        .Stroke
        .Solid(Colors.Blue);

    // links unten
    Ctx
        .Point(0, Ctx.Height - 1)
        .Stroke
        .Solid(Colors.Red);
 
    // rechts oben
    Ctx
        .Point(Ctx.Width - 1, 0)
        .Stroke
        .Solid(Colors.Green);

    // rechts unten
    Ctx
        .Point(Ctx.Width - 1, Ctx.Height - 1)
        .Stroke
        .Solid(Colors.Yellow);

    // Console.WriteLine(Ctx.Width);
};



await PXL.Simulate(scene);

// await PXL.SendToDevice(scene, "DeviceIP_or_NameInNetwork");





