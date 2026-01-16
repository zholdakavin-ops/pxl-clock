#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// ===================================================================
// GEWITTER-ANIMATION FÜR 24x24 PIXEL-MATRIX
// Dunkle Gewitterszene: Schwarzer Himmel, dramatische Wolken, intensive Blitze, starker Regen
// Professionelles Design mit starkem Kontrast
// ===================================================================

var mainScene = () =>
{
    // ---------------------------------------------------------------
    // 1. ZEITSTEUERUNG - ANIMATIONS-ZYKLUS
    // ---------------------------------------------------------------
    // Gesamtdauer: 30 Sekunden für einen vollständigen Zyklus
    var cycleDuration = 30.0;
    var t = Ctx.Now.TimeOfDay.TotalSeconds % cycleDuration;
    
    // Normalisierter Fortschritt (0.0 bis 1.0)
    var progress = t / cycleDuration;
    
    
    // ---------------------------------------------------------------
    // 2. DUNKLER GEWITTERHIMMEL (FAST SCHWARZ)
    // ---------------------------------------------------------------
    
    // Dunkler Himmel mit leichten Variationen und gelegentlichem Blitz-Aufleuchten
    (byte r, byte g, byte b) GetSkyColor(double prog, double heightRatio)
    {
        // Grundfarbe: Sehr dunkles Blau-Grau (fast schwarz)
        var baseR = 8;
        var baseG = 10;
        var baseB = 15;
        
        // Leichte Aufhellung zum Horizont für minimale Tiefe
        var horizonLift = (int)(heightRatio * 5);
        
        return (
            (byte)(baseR + horizonLift),
            (byte)(baseG + horizonLift),
            (byte)(baseB + horizonLift)
        );
    }
    
    // DUNKLER HIMMEL ZEICHNEN: Zeilen 0-17 (obere 18 Pixel)
    for (var y = 0; y < 18; y++)
    {
        var heightRatio = (double)y / 17.0;
        var (r, g, b) = GetSkyColor(progress, heightRatio);
        
        // Jede Zeile mit dunkler Farbe füllen
        Ctx.RectXyWh(0, y, 24, 1).Fill.Solid(Color.FromArgb(255, r, g, b));
    }
    
    // ---------------------------------------------------------------
    // 3. MEHRSCHICHTIGE BERGE MIT STARKEM KONTRAST & TIEFE
    // ---------------------------------------------------------------
    
    // Hilfsfunktion: Berechnet Berghöhe für eine Ebene an Position x
    // layer: 1=Hintergrund, 2=Mittelgrund, 3=Vordergrund
    double GetMountainHeight(double x, int layer)
    {
        // Jede Ebene hat unterschiedliche Wellenformen für mehr Variation
        if (layer == 1) // Hintergrundberge - sanfter, niedriger
        {
            var wave1 = Math.Sin(x * 0.3) * 1.5;
            var wave2 = Math.Sin(x * 0.6 + 2.0) * 0.8;
            return 3.5 + wave1 + wave2;
        }
        else if (layer == 2) // Mittelgrundberge - mittlere Höhe
        {
            var wave1 = Math.Sin(x * 0.35 + 1.0) * 2.0;
            var wave2 = Math.Sin(x * 0.7 + 3.0) * 1.0;
            var wave3 = Math.Sin(x * 1.4 + 1.5) * 0.5;
            return 4.5 + wave1 + wave2 + wave3;
        }
        else // layer == 3, Vordergrundberge - höchste, gezackte Gipfel
        {
            var wave1 = Math.Sin(x * 0.4 + 0.5) * 2.2;
            var wave2 = Math.Sin(x * 0.9 + 4.0) * 1.2;
            var wave3 = Math.Sin(x * 1.8 + 2.5) * 0.6;
            // Gezackte Spitzen durch schärfere Wellen
            var peaks = Math.Sin(x * 2.5) * 0.4;
            return 5.0 + wave1 + wave2 + wave3 + peaks;
        }
    }
    
    // Funktion: Zeichnet eine Berg-Ebene mit spezifischer Farbe
    void DrawMountainLayer(int layer)
    {
        for (var x = 0; x < 24; x++)
        {
            var mountainHeight = GetMountainHeight(x, layer);
            
            for (var y = 0; y < 6; y++)
            {
                var actualY = 18 + y;
                var pixelFromTop = (double)y;
                
                // Prüfen, ob dieser Pixel Teil des Berges ist
                if (pixelFromTop < mountainHeight)
                {
                    byte mountainR, mountainG, mountainB;
                    
                    // Alle Berge sehr dunkel (schwarz) für Gewitterszene
                    // Nur durch Blitze werden sie kurzzeitig sichtbar
                    
                    if (layer == 1)
                    {
                        // Hintergrundberge - tiefes Schwarz
                        mountainR = 5;
                        mountainG = 5;
                        mountainB = 8;
                    }
                    else if (layer == 2)
                    {
                        // Mittelgrundberge - sehr dunkel
                        mountainR = 8;
                        mountainG = 8;
                        mountainB = 10;
                    }
                    else
                    {
                        // Vordergrundberge - schwarze Silhouette
                        mountainR = 10;
                        mountainG = 10;
                        mountainB = 12;
                    }
                    
                    Ctx.RectXyWh(x, actualY, 1, 1).Fill.Solid(
                        Color.FromArgb(255, mountainR, mountainG, mountainB));
                }
            }
        }
    }
    
    // BERGE IN KORREKTER REIHENFOLGE ZEICHNEN (hinten nach vorne)
    DrawMountainLayer(1); // Hintergrundberge (schwarze Silhouette)
    DrawMountainLayer(2); // Mittelgrundberge (schwarze Silhouette)
    DrawMountainLayer(3); // Vordergrundberge (schwarze Silhouette)
    
    // ---------------------------------------------------------------
    // 4. DRAMATISCHE GEWITTERWOLKEN (1-2 GROSSE DUNKLE WOLKEN)
    // ---------------------------------------------------------------
    
    
    // Hilfsfunktion: Zeichnet eine dramatische Gewitterwolke
    // x, y: Position, size: Größe
    void DrawStormCloud(double x, double y, double size)
    {
        // Wolken außerhalb des Bildschirms nicht zeichnen
        if (x < -8 || x > 28) return;
        
        // DUNKLE GEWITTERWOLKE - massiv und bedrohlich
        // Sehr dunkles Grau mit leichten Aufhellungen
        
        var darkGray = Color.FromArgb(255, 25, 25, 30);
        var mediumGray = Color.FromArgb(255, 35, 35, 40);
        var lightGray = Color.FromArgb(255, 45, 45, 50);
        
        // Wolke aus vielen überlappenden Kreisen - massiv und voluminös
        // Untere Schicht (dunkelste)
        Ctx.Circle(x + 1.0 * size, y + 1.5 * size, 1.8 * size).Fill.Solid(darkGray);
        Ctx.Circle(x + 3.0 * size, y + 1.8 * size, 2.0 * size).Fill.Solid(darkGray);
        Ctx.Circle(x + 5.0 * size, y + 1.5 * size, 1.7 * size).Fill.Solid(darkGray);
        
        // Mittlere Schicht
        Ctx.Circle(x, y + 0.5 * size, 1.5 * size).Fill.Solid(mediumGray);
        Ctx.Circle(x + 2.0 * size, y, 2.2 * size).Fill.Solid(mediumGray);
        Ctx.Circle(x + 4.0 * size, y + 0.3 * size, 1.9 * size).Fill.Solid(mediumGray);
        Ctx.Circle(x + 6.0 * size, y + 0.5 * size, 1.4 * size).Fill.Solid(mediumGray);
        
        // Obere Schicht (hellste - aber immer noch dunkel)
        Ctx.Circle(x + 1.5 * size, y - 0.5 * size, 1.3 * size).Fill.Solid(lightGray);
        Ctx.Circle(x + 3.5 * size, y - 0.8 * size, 1.6 * size).Fill.Solid(lightGray);
        Ctx.Circle(x + 5.0 * size, y - 0.3 * size, 1.2 * size).Fill.Solid(lightGray);
    }
    
    // GEWITTERWOLKEN ZEICHNEN: Nur 2 große Wolken - professionelles Design
    
    // Wolke 1: Große Hauptwolke, langsam ziehend
    var stormCloud1X = (t * 0.5) % 36 - 8;
    DrawStormCloud(stormCloud1X, 4.0, 1.0);
    
    // Wolke 2: Zweite große Wolke, leicht versetzt
    var stormCloud2X = (t * 0.5 + 18) % 36 - 8;
    DrawStormCloud(stormCloud2X, 6.5, 0.95);
    
    
    // ---------------------------------------------------------------
    // 5. STARKER REGEN (DURCHGEHEND, INTENSIV)
    // ---------------------------------------------------------------
    
    // Hilfsfunktion: Zeichnet starken Gewitterregen
    void DrawStormRain(double cloudX, double cloudY, int dropCount, double speedFactor)
    {
        for (var i = 0; i < dropCount; i++)
        {
            // Jeder Tropfen hat eigene Position
            var offset = i * 1.8;
            
            // Tropfen-X: Unter und neben der Wolke verteilt
            var dropX = cloudX + offset;
            
            // Tropfen-Y: Fällt schnell
            var fallSpeed = 4.0 + speedFactor + (i % 3) * 0.8;
            var dropY = cloudY + 2.0 + ((t * fallSpeed + offset) % 14.0);
            
            // Prüfen, ob Tropfen im sichtbaren Bereich
            if (dropX >= 0 && dropX < 24 && dropY < 18)
            {
                // Prüfe Berghöhe
                var maxMountainHeight = GetMountainHeight((int)dropX, 3);
                var mountainTopY = 18 + (6 - maxMountainHeight);
                
                // Regen stoppt vor Berg
                if (dropY < mountainTopY - 0.5)
                {
                    // Helle Regentropfen für Kontrast gegen dunklen Himmel
                    var dropAlpha = (byte)(220);
                    
                    // Längere Tropfen für starken Regen-Effekt
                    Ctx.RectXyWh(dropX, dropY, 0.6, 1.5)
                        .Fill.Solid(Color.FromArgb(dropAlpha, 200, 220, 255));
                }
            }
        }
    }
    
    // STARKER REGEN von beiden Gewitterwolken
    DrawStormRain(stormCloud1X + 3, 4.0, 12, 1.2);
    DrawStormRain(stormCloud2X + 3, 6.5, 11, 1.0);
    
    
    // ---------------------------------------------------------------
    // 6. INTENSIVE BLITZE (HAUPT-EFFEKT DES GEWITTERS)
    // ---------------------------------------------------------------
    
    // Blitze erscheinen häufiger bei Gewitter
    var lightningTrigger = t % 4.5; // Alle 4.5 Sekunden
    var lightningActive = lightningTrigger < 0.25; // Länger sichtbar
    
    if (lightningActive)
    {
        // Blitz-Position: Variiert mit Zeit
        var lightningX = 6.0 + (t * 2.7) % 12.0;
        var lightningY = 2.0 + (t * 1.8) % 4.0;
        
        // Blitz-Helligkeit pulsiert sehr hell
        var lightningBrightness = Math.Abs(Math.Sin(t * 60)) * 0.6 + 0.4;
        var lightningAlpha = (byte)(255 * lightningBrightness);
        
        // HAUPT-BLITZ: Lange vertikale Linie mit dramatischem Zickzack
        for (var i = 0; i < 10; i++)
        {
            var zigzag = Math.Sin(i * 1.2) * 1.2; // Stärkerer Zickzack
            var segmentWidth = 1.0 + (i % 2) * 0.4;
            
            Ctx.RectXyWh(lightningX + zigzag, lightningY + i * 1.5, segmentWidth, 1.8)
                .Fill.Solid(Color.FromArgb(lightningAlpha, 255, 255, 240));
        }
        
        // INTENSIVER GLANZ um den Blitz - sehr groß und hell
        Ctx.Circle(lightningX, lightningY + 7, 6.0)
            .Fill.Solid(Color.FromArgb((byte)(120 * lightningBrightness), 255, 255, 230));
        
        Ctx.Circle(lightningX, lightningY + 7, 4.0)
            .Fill.Solid(Color.FromArgb((byte)(180 * lightningBrightness), 255, 255, 240));
        
        // BLITZ ERHELLT GESAMTE SZENE DRAMATISCH
        // Himmel wird kurzzeitig aufgehellt
        for (var y = 0; y < 18; y++)
        {
            var skyIllumination = (byte)(40 * lightningBrightness * (1.0 - y / 18.0));
            Ctx.RectXyWh(0, y, 24, 1)
                .Fill.Solid(Color.FromArgb(skyIllumination, 200, 210, 255));
        }
        
        // Berge werden dramatisch erhellt - STARKER KONTRAST
        for (var x = 0; x < 24; x++)
        {
            var distToLightning = Math.Abs(x - lightningX);
            var illumination = (1.0 - Math.Min(distToLightning / 12.0, 1.0)) * lightningBrightness;
            
            for (var y = 0; y < 6; y++)
            {
                var actualY = 18 + y;
                var fgMountainHeight = GetMountainHeight(x, 3);
                
                if (y < fgMountainHeight)
                {
                    // Sehr helle Aufhellung - Berge werden sichtbar!
                    var overlayAlpha = (byte)(180 * illumination);
                    Ctx.RectXyWh(x, actualY, 1, 1)
                        .Fill.Solid(Color.FromArgb(overlayAlpha, 230, 240, 255));
                }
            }
        }
        
        // Wolken werden von innen beleuchtet
        Ctx.Circle(lightningX, lightningY, 3.5)
            .Fill.Solid(Color.FromArgb((byte)(150 * lightningBrightness), 255, 255, 220));
    }
};

// ===================================================================
// ANLEITUNG & DOKUMENTATION
await PXL.SendToDevice(mainScene, "192.168.178.110");
