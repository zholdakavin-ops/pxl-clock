#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;


// Das hier wird 40 mal pro Sekunde aufgerufen

// x ist "extern" definiert - d.h.: "extern" (ausserhalb) in Bezug auf die "scene" Funktion
var x = 0;

// "scene" ist eine Funktion (so ähnlich eiw eine Methode. z.B. "Line").
// Dise Funktion nimmer "nichts" entgegen (leere Klammer).
// Dierse Funktion kann aufgerufen werden.
// Ganz unten steht: PXL.Simulate(scene);
// "Simulate" ist selbst eine Methode von "PXL".
// Die "Simulate" Methode braucht eine aufrufbare Funktion.
// "Simulate" ruft dann (irgendwie) die übergebene Funktion 40 mal in der Sekunde auf.
// D.h.: Der ganze Code, der zwischen { } steht, wird 40 mal in der Sekunde aufgerufen.
// Die Varaible "x" steht außerhalb von { }.
// Wir können uns von innerhalb der {} auf die "x" Variable beziehen.
// Der Wert von "x" bleibt von Aufruf zu Aufruf der "scene" Funktion der selbe.
var scene = () => {
    // links unten
    // Ctx.Line(0, Ctx.Height, Ctx.Width, 0).Stroke.Solid(Colors.Aqua);
    // Ctx.Line(Ctx.Width, Ctx.Height, 0, 0).Stroke.Solid(Colors.Red);
    // Ctx.Line(Ctx.Width, 0, 0, 0).Stroke.Solid(Colors.Aqua);
    // Ctx.Line(0, 0, 0, Ctx.Height).Stroke.Solid(Colors.Yellow);
    // Ctx.Line(0, Ctx.Height, Ctx.Width, Ctx.Height);
    // Ctx.Line(Ctx.Width, Ctx.Height, Ctx.Width, 0);

    // Eine Linie mit 2 Punkten (x1, y1, x2, y2).
    // Die Linier MUSS vertikal sein, da die X-Koordinaten der beiden Punkte immer gleich sind.
    Ctx.Line(
        x, 0, 
        x, Ctx.Height
    ).Stroke.Solid(Colors.Yellow);

    // der neue Wert von x wird hier berechnet:
    // "=" bedeutet: der Wert wird neu zugewiesen.
    // "Wir geben der Variable x einen neuen Wert".
    // Der neue Wert steht rechts vom Gleichheitszeichen.
    // die Variable, die den Wert bekommt, steht links davon.
    x = x + 1;      //x++;

    if (x >= Ctx.Width / 2) 
        x = 0; 
    
};



// await PXL.Simulate(scene);

await PXL.SendToDevice(scene, "192.168.178.110");





