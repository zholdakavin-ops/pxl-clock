#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;


byte r = 0;
byte g = 0;
byte b = 0;




var scene = () =>
{
    var background = Color.FromArgb(255, r, g, b);

    // // Hintergrund füllen
    // for (int y = 0; y < Ctx.Height; y++)
    // {
    //     for (int x = 0; x < Ctx.Width; x++)
    //     {
    //         Ctx.Point(x, y).Stroke.Solid(background);
    //     }
    // }

    Ctx.Background.Solid(background);

    //Leichte Animation: schmale Linie von links nach rechts //// 1.Rotwert um 1 erhöhen //2.Wenn Rot größer als 255 //3.wieder bei 0 anfangen

    r++;
    if (r > 255) r = 0;

    // g++;
    // if (g > 255) g = 0;

    // b++;
    // if (b > 255) b = 0;

    Ctx.RectXyWh(5, 5, 3, 10).Fill.Solid(Color.FromArgb(150, 0, 0, 255));
    Ctx.RectXyWh(3, 9, 6, 10).Fill.Solid(Color.FromArgb(130, 0, 0, 255));
};





await PXL.Simulate(scene);

// await PXL.SendToDevice(scene, "192.168.178.100");


