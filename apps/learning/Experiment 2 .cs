#:package Pxl@0.0.34

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;


byte r = 0;
byte g = 60;
byte b = 120;


var scene = () =>
{

    //Leichte Animation: schmale Linie von links nach rechts

    r++;
    g++;
    b++;

    // Werte wieder auf 0 setzen, wenn 255 überschritten
    if (r > 255) r = 0;
    if (g > 255) g = 0;
    if (b > 255) b = 0;
    
    



        //1.zahl: x-Koordinate(links nach rechts), 2.zahl y-Koordinate (oben nach unten), 3.Zahl: breite des kastens, 4.Zahl: Höhe des Kastens.  

    var backgrounduntenrechts = Ctx.RectXyWh(23, 23,1, 1).Fill.Solid (Color.FromArgb(255, r, g, b));

    var backgrounduntenlinks = Ctx.RectXyWh(0, 23, 1, 1).Fill.Solid (Color.FromArgb(255, r, g, b));

    var backgroundobenrechts = Ctx.RectXyWh(23, 0, 1, 1).Fill.Solid (Color.FromArgb(255, r, g, b));

    var backgroundobenlinks = Ctx.RectXyWh(0, 0, 1, 1).Fill.Solid (Color.FromArgb(255, r, g, b));



    




    // 3Uhrzeit holen
    var now = Ctx.Now;
    var text = now.ToString("HH:mm");

    // Uhr zentrieren
    int textX = (Ctx.Width - (text.Length * 3) - 1) / 2;
    int textY = (Ctx.Height - 5) / 2;

    //  Uhr zeichnen
    Ctx.Text.Var3x5(text, textX, textY)
        .Brush
        .Solid(Colors.White);
};

//await PXL.Simulate(scene);

// await PXL.SendToDevice(scene, "192.168.178.100");



// await PXL.Simulate(scene);

await PXL.SendToDevice(scene, "192.168.178.110");