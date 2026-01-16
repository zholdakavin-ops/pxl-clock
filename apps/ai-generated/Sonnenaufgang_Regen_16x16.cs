#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// ===================================================================
// ABENDGEWITTER-ANIMATION FÜR 24x24 PIXEL-MATRIX
// Dramatische Abend-Gewitterszene mit intensiven Blitzen und starkem Kontrast
// Anspruchsvolles, kinematographisches Design
// ===================================================================

var mainScene = () =>
{
    // ---------------------------------------------------------------
    // 1. ZEITSTEUERUNG - ANIMATIONS-ZYKLUS
    // ---------------------------------------------------------------
    // Gesamtdauer: 60 Sekunden für einen vollständigen Zyklus (länger, cinematischer)
    var cycleDuration = 60.0;
    var t = Ctx.Now.TimeOfDay.TotalSeconds % cycleDuration;
    
    // Normalisierter Fortschritt (0.0 bis 1.0)
    var progress = t / cycleDuration;
    
    
    // ---------------------------------------------------------------
    // 2. DRAMATISCHER ABENDHIMMEL MIT VERLAUF
    // ---------------------------------------------------------------
    
    // Abendhimmel mit tiefem Blau-Violett und dramatischen Farbverläufen
    (byte r, byte g, byte b) GetSkyColor(double prog, double heightRatio)
    {
        // Abendhimmel: Dunkles Violett-Blau oben, etwas heller zum Horizont
        // Starker Kontrast für Blitze
        
        if (heightRatio < 0.4)
        {
            // Oberer Himmel: Sehr dunkles Violett-Blau
            return (
                (byte)(12 + heightRatio * 15),
                (byte)(8 + heightRatio * 10),
                (byte)(25 + heightRatio * 20)
            );
        }
        else if (heightRatio < 0.7)
        {
            // Mittlerer Himmel: Übergang zu dunkelblau
            var t = (heightRatio - 0.4) / 0.3;
            return (
                (byte)(18 + t * 10),
                (byte)(12 + t * 8),
                (byte)(35 + t * 15)
            );
        }
        else
        {
            // Unterer Himmel (Horizont): Leicht aufgehellt mit violettem Ton
            var t = (heightRatio - 0.7) / 0.3;
            return (
                (byte)(28 + t * 12),
                (byte)(20 + t * 10),
                (byte)(50 + t * 15)
            );
        }
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
                    
                    // Berge in dunklem Abendlicht - deutliche Silhouetten
                    // Genug Kontrast zum Himmel, aber dunkel genug für Blitz-Effekt
                    
                    if (layer == 1)
                    {
                        // Hintergrundberge - dunkelblau-grau
                        mountainR = 15;
                        mountainG = 12;
                        mountainB = 20;
                    }
                    else if (layer == 2)
                    {
                        // Mittelgrundberge - dunkler
                        mountainR = 12;
                        mountainG = 10;
                        mountainB = 16;
                    }
                    else
                    {
                        // Vordergrundberge - fast schwarz mit blauem Ton
                        mountainR = 8;
                        mountainG = 8;
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
    // 4. MASSIVE GEWITTERWOLKEN MIT DETAILS
    // ---------------------------------------------------------------
    
    
    // Hilfsfunktion: Zeichnet eine dramatische Gewitterwolke mit mehr Details
    // x, y: Position, size: Größe, variation: Farbvariation
    void DrawStormCloud(double x, double y, double size, double variation)
    {
        // Wolken außerhalb des Bildschirms nicht zeichnen
        if (x < -8 || x > 28) return;
        
        // ABEND-GEWITTERWOLKE - massiv mit violett-grauen Tönen
        // Verschiedene Schichten für Tiefe und visuelles Interesse
        
        var veryDark = Color.FromArgb(255, (byte)(20 + variation * 5), (byte)(18 + variation * 3), (byte)(30 + variation * 5));
        var darkGray = Color.FromArgb(255, (byte)(30 + variation * 8), (byte)(28 + variation * 5), (byte)(40 + variation * 8));
        var mediumGray = Color.FromArgb(255, (byte)(40 + variation * 10), (byte)(38 + variation * 8), (byte)(50 + variation * 10));
        var lightGray = Color.FromArgb(255, (byte)(50 + variation * 12), (byte)(48 + variation * 10), (byte)(60 + variation * 12));
        
        // Untere Schicht (dunkelste, massivste)
        Ctx.Circle(x + 0.5 * size, y + 1.8 * size, 2.0 * size).Fill.Solid(veryDark);
        Ctx.Circle(x + 2.5 * size, y + 2.0 * size, 2.2 * size).Fill.Solid(veryDark);
        Ctx.Circle(x + 4.5 * size, y + 1.7 * size, 1.9 * size).Fill.Solid(veryDark);
        Ctx.Circle(x + 6.0 * size, y + 1.5 * size, 1.6 * size).Fill.Solid(veryDark);
        
        // Zweite Schicht
        Ctx.Circle(x + 1.0 * size, y + 1.2 * size, 1.7 * size).Fill.Solid(darkGray);
        Ctx.Circle(x + 3.0 * size, y + 1.4 * size, 2.0 * size).Fill.Solid(darkGray);
        Ctx.Circle(x + 5.0 * size, y + 1.1 * size, 1.8 * size).Fill.Solid(darkGray);
        
        // Dritte Schicht
        Ctx.Circle(x, y + 0.5 * size, 1.5 * size).Fill.Solid(mediumGray);
        Ctx.Circle(x + 2.0 * size, y + 0.2 * size, 2.0 * size).Fill.Solid(mediumGray);
        Ctx.Circle(x + 4.0 * size, y + 0.4 * size, 1.9 * size).Fill.Solid(mediumGray);
        Ctx.Circle(x + 6.0 * size, y + 0.6 * size, 1.5 * size).Fill.Solid(mediumGray);
        
        // Oberste Schicht (Ränder, Details)
        Ctx.Circle(x + 1.5 * size, y - 0.3 * size, 1.2 * size).Fill.Solid(lightGray);
        Ctx.Circle(x + 3.5 * size, y - 0.6 * size, 1.5 * size).Fill.Solid(lightGray);
        Ctx.Circle(x + 5.0 * size, y - 0.2 * size, 1.1 * size).Fill.Solid(lightGray);
    }
    
    // GEWITTERWOLKEN ZEICHNEN: 3 Wolken für mehr Tiefe - sehr langsam ziehend
    
    // Wolke 1: Große Hauptwolke, sehr langsam
    var stormCloud1X = (t * 0.3) % 40 - 10;
    DrawStormCloud(stormCloud1X, 3.5, 1.1, 0.0);
    
    // Wolke 2: Zweite große Wolke, leicht versetzt und höher
    var stormCloud2X = (t * 0.3 + 20) % 40 - 10;
    DrawStormCloud(stormCloud2X, 6.0, 1.05, 1.0);
    
    // Wolke 3: Dritte Wolke für zusätzliche Tiefe
    var stormCloud3X = (t * 0.3 + 35) % 40 - 10;
    DrawStormCloud(stormCloud3X, 8.5, 0.95, -1.0);
    
    
    // ---------------------------------------------------------------
    // 5. SPEKTAKULÄRE BLITZE - HAUPT-VISUELLES ELEMENT
    // ---------------------------------------------------------------
    
    
    // Mehrere Blitz-Systeme für mehr Varianz und häufigere Sichtbarkeit
    
    // HAUPT-BLITZ-SYSTEM: Häufig und dramatisch
    var lightningTrigger1 = t % 5.0; // Alle 5 Sekunden
    var lightningActive1 = lightningTrigger1 < 0.4; // Länger sichtbar (0.4s)
    
    // ZWEITES BLITZ-SYSTEM: Versetztes Timing
    var lightningTrigger2 = (t + 2.5) % 6.5; // Versetzt um 2.5s, alle 6.5 Sekunden
    var lightningActive2 = lightningTrigger2 < 0.35;
    
    // FERNES BLITZ-SYSTEM: Subtiler, im Hintergrund
    var lightningTrigger3 = (t + 4.0) % 8.0;
    var lightningActive3 = lightningTrigger3 < 0.25;
    
    // HAUPT-BLITZ 1
    if (lightningActive1)
    {
        // Blitz-Position: Variiert mit Zeit
        var lightningX = 5.0 + (t * 3.2) % 14.0;
        var lightningY = 1.5 + (t * 2.1) % 4.5;
        
        // Blitz-Helligkeit pulsiert sehr hell
        var lightningBrightness = Math.Abs(Math.Sin(t * 70)) * 0.5 + 0.5;
        var lightningAlpha = (byte)(255 * lightningBrightness);
        
        // HAUPT-BLITZ: Lange vertikale Linie mit dramatischem Zickzack
        for (var i = 0; i < 12; i++)
        {
            var zigzag = Math.Sin(i * 1.4) * 1.5; // Stärkerer Zickzack
            var segmentWidth = 1.2 + (i % 2) * 0.5;
            
            Ctx.RectXyWh(lightningX + zigzag, lightningY + i * 1.4, segmentWidth, 1.9)
                .Fill.Solid(Color.FromArgb(lightningAlpha, 255, 255, 245));
            
            // Zusätzliche Verzweigungen für Realismus
            if (i % 3 == 0 && i > 2)
            {
                var branchX = lightningX + zigzag + (i % 2 == 0 ? 1.5 : -1.5);
                Ctx.RectXyWh(branchX, lightningY + i * 1.4, 0.8, 2.5)
                    .Fill.Solid(Color.FromArgb((byte)(lightningAlpha * 0.7), 255, 255, 240));
            }
        }
        
        // INTENSIVER GLANZ um den Blitz - sehr groß und hell
        Ctx.Circle(lightningX, lightningY + 8, 7.5)
            .Fill.Solid(Color.FromArgb((byte)(100 * lightningBrightness), 255, 255, 235));
        
        Ctx.Circle(lightningX, lightningY + 8, 5.0)
            .Fill.Solid(Color.FromArgb((byte)(160 * lightningBrightness), 255, 255, 245));
            
        Ctx.Circle(lightningX, lightningY + 8, 2.5)
            .Fill.Solid(Color.FromArgb((byte)(220 * lightningBrightness), 255, 255, 250));
        
        // BLITZ ERHELLT GESAMTE SZENE EXTREM DRAMATISCH
        // Himmel wird kurzzeitig stark aufgehellt
        for (var y = 0; y < 18; y++)
        {
            var skyIllumination = (byte)(60 * lightningBrightness * (1.0 - y / 18.0));
            Ctx.RectXyWh(0, y, 24, 1)
                .Fill.Solid(Color.FromArgb(skyIllumination, 220, 225, 255));
        }
        
        // Berge werden extrem erhellt - MAXIMALER KONTRAST
        for (var x = 0; x < 24; x++)
        {
            var distToLightning = Math.Abs(x - lightningX);
            var illumination = (1.0 - Math.Min(distToLightning / 14.0, 1.0)) * lightningBrightness;
            
            for (var y = 0; y < 6; y++)
            {
                var actualY = 18 + y;
                var fgMountainHeight = GetMountainHeight(x, 3);
                
                if (y < fgMountainHeight)
                {
                    // Sehr helle Aufhellung - Berge werden spektakulär sichtbar!
                    var overlayAlpha = (byte)(200 * illumination);
                    Ctx.RectXyWh(x, actualY, 1, 1)
                        .Fill.Solid(Color.FromArgb(overlayAlpha, 240, 245, 255));
                }
            }
        }
        
        // Wolken werden von innen dramatisch beleuchtet
        Ctx.Circle(lightningX, lightningY + 2, 4.5)
            .Fill.Solid(Color.FromArgb((byte)(180 * lightningBrightness), 255, 255, 230));
    }
    
    // ZWEITER BLITZ (versetzt)
    if (lightningActive2)
    {
        var lightning2X = 16.0 + (t * 2.8) % 6.0;
        var lightning2Y = 2.5 + (t * 1.5) % 3.5;
        
        var brightness2 = Math.Abs(Math.Sin(t * 65)) * 0.4 + 0.6;
        var alpha2 = (byte)(255 * brightness2);
        
        // Zweiter Blitz - etwas anders geformt
        for (var i = 0; i < 10; i++)
        {
            var zigzag = Math.Cos(i * 1.1) * 1.3;
            var segmentWidth = 1.0 + (i % 3 == 0 ? 0.6 : 0.3);
            
            Ctx.RectXyWh(lightning2X + zigzag, lightning2Y + i * 1.5, segmentWidth, 1.8)
                .Fill.Solid(Color.FromArgb(alpha2, 255, 255, 240));
        }
        
        // Glanz für zweiten Blitz
        Ctx.Circle(lightning2X, lightning2Y + 7, 5.5)
            .Fill.Solid(Color.FromArgb((byte)(120 * brightness2), 255, 255, 235));
        
        Ctx.Circle(lightning2X, lightning2Y + 7, 3.5)
            .Fill.Solid(Color.FromArgb((byte)(180 * brightness2), 255, 255, 245));
        
        // Himmel-Aufhellung
        for (var y = 0; y < 18; y++)
        {
            var skyIllum = (byte)(40 * brightness2 * (1.0 - y / 20.0));
            Ctx.RectXyWh(0, y, 24, 1)
                .Fill.Solid(Color.FromArgb(skyIllum, 210, 220, 255));
        }
        
        // Berg-Beleuchtung
        for (var x = 0; x < 24; x++)
        {
            var dist = Math.Abs(x - lightning2X);
            var illum = (1.0 - Math.Min(dist / 12.0, 1.0)) * brightness2;
            
            for (var y = 0; y < 6; y++)
            {
                var actualY = 18 + y;
                var mountainH = GetMountainHeight(x, 3);
                
                if (y < mountainH)
                {
                    var overlayA = (byte)(160 * illum);
                    Ctx.RectXyWh(x, actualY, 1, 1)
                        .Fill.Solid(Color.FromArgb(overlayA, 230, 240, 255));
                }
            }
        }
    }
    
    // FERNER BLITZ (subtiler Hintergrund-Effekt)
    if (lightningActive3)
    {
        var lightning3X = 10.0 + (t * 1.9) % 8.0;
        var lightning3Y = 3.0 + (t * 1.2) % 3.0;
        
        var brightness3 = Math.Abs(Math.Sin(t * 55)) * 0.3 + 0.4;
        
        // Subtiler Himmels-Blitz (nur Aufhellung, kein direkter Blitz sichtbar)
        for (var y = 0; y < 18; y++)
        {
            var skyIllum = (byte)(25 * brightness3 * (1.0 - y / 22.0));
            Ctx.RectXyWh(0, y, 24, 1)
                .Fill.Solid(Color.FromArgb(skyIllum, 200, 215, 255));
        }
        
        // Diffuser Glanz (simuliert fernen Blitz hinter Wolken)
        Ctx.Circle(lightning3X, lightning3Y, 6.0)
            .Fill.Solid(Color.FromArgb((byte)(60 * brightness3), 240, 245, 255));
    }
};

// ===================================================================
// ANLEITUNG & DOKUMENTATION
await PXL.SendToDevice(mainScene, "192.168.178.110");
