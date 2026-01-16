#:package Pxl@0.0.34

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;



var scene = () =>
{
var scene = () =>
{
    // Zeit (läuft automatisch)
    double t = Ctx.Now.TimeOfDay.TotalSeconds;

    // Sanfte, endlose Farbänderung
    byte r = (byte)(Math.Sin(t) * 127 + 128);
    byte g = (byte)(Math.Sin(t + 2) * 127 + 128);
    byte b = (byte)(Math.Sin(t + 4) * 127 + 128);

    // Hintergrund füllen
    Ctx.Background.Solid(Color.FromArgb(255, r, g, b));

    // Uhrzeit
    var text = Ctx.Now.ToString("HH:mm");

    // Zentrierung
    int x = (Ctx.Width - text.Length * 3) / 2;
    int y = (Ctx.Height - 5) / 2;

    // Uhr zeichnen
    Ctx.Text.Var3x5(text, x, y)
        .Brush
        .Solid(Colors.White);
};

    };
// Simulator
// await PXL.Simulate(scene);

// Gerät
await PXL.SendToDevice(scene, "192.168.178.110");
        








// await PXL.Simulate(scene);

await PXL.SendToDevice(scene, "192.168.178.110");