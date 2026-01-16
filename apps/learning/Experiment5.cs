#:package Pxl@0.0.34

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;


byte r = 0;
byte g = 60;
byte b = 120;

var x = 0;





var scene = () =>
{
    var background = Color.FromArgb(255, r, g, b);


    Ctx.Background.Solid(background);

    //Leichte Animation: schmale Linie von links nach rechts

    r++;
    g++;
    b++;

    // Werte wieder auf 0 setzen, wenn 255 überschritten
    if (r > 255) r = 0;
    if (g > 255) g = 0;
    if (b > 255) b = 0;

    //1.zahl: x-Koordinate(links nach rechts), 2.zahl y-Koordinate (oben nach unten), 3.Zahl: breite des kastens, 4.Zahl: Höhe des Kastens.  
    var untenlinks = Ctx.RectXyWh(0, 14, 4, 10).Fill.Solid(Color.FromArgb(255, 100, 0, 0));
    //-> 1.Zahl: Transparenz, 2.Zahl; Helligkeit/Farbe, 3.Zahl: farbe, 4.Zahl:farbe

    var untenrechts = Ctx.RectXyWh(20, 14, 4, 10).Fill.Solid(Color.FromArgb(255, 100, 0, 0));

    var obenlinks = Ctx.RectXyWh(0, 0, 4, 10).Fill.Solid(Color.FromArgb(255, 100, 0, 0));

    var obenrechts = Ctx.RectXyWh(20, 0, 4, 10).Fill.Solid(Color.FromArgb(255, 100, 0, 0));

    Ctx.Line(x, 10, x, 14).Stroke.Solid(Colors.Beige);
    //Ctx.Line(0, 10, x, 14).Stroke.Solid(Colors.Beige);


    x = x + 1;     

    if (x >= Ctx.Width)
        x = 0;

};





// PXL.Simulate(scene);
await PXL.SendToDevice(scene, "192.168.178.110");
