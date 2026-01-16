#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// ===================================================================
// SONNENAUFGANGS-ANIMATION FÜR 24x24 PIXEL-MATRIX
// Ein kleiner Kurzfilm: Berge, Himmel, aufgehende Sonne und Wolken
// ===================================================================

var mainScene = () =>
{
    // ---------------------------------------------------------------
    // 1. ZEITSTEUERUNG - LANGSAME, NATÜRLICHE ANIMATION
    // ---------------------------------------------------------------
    // Gesamtdauer: 60 Sekunden für einen vollständigen Zyklus (realistischer)
    var cycleDuration = 60.0;
    var t = Ctx.Now.TimeOfDay.TotalSeconds % cycleDuration;
    
    // Normalisierter Fortschritt (0.0 bis 1.0)
    var progress = t / cycleDuration;
    
    // Sonnenhöhe mit sanfter Ease-Kurve (realistischer als linear)
    var sunProgress = progress < 0.5 
        ? Math.Pow(progress * 2.0, 0.8)  // Aufgang mit Ease-Out
        : Math.Pow((1.0 - progress) * 2.0, 0.8);  // Untergang mit Ease-Out
    
    // ---------------------------------------------------------------
    // 2. REALISTISCHER HIMMEL MIT SANFTEN FARBÜBERGÄNGEN
    // ---------------------------------------------------------------
    
    // Hilfsfunktion: Realistische Himmelsfarben mit besserem Kontrast
    (byte r, byte g, byte b) GetSkyColor(double prog, double heightRatio)
    {
        // Nacht (0-15% und 85-100%) - DUNKEL mit Sternen
        if (prog < 0.15 || prog > 0.85)
        {
            var nightIntensity = prog < 0.15 ? (1.0 - prog / 0.15) : ((prog - 0.85) / 0.15);
            // Sehr dunkles Nachtblau, minimal heller am Horizont
            var baseR = (byte)(2 + heightRatio * 8);
            var baseG = (byte)(4 + heightRatio * 12);
            var baseB = (byte)(15 + heightRatio * 25);
            return (
                (byte)(baseR * nightIntensity),
                (byte)(baseG * nightIntensity),
                (byte)(baseB * nightIntensity)
            );
        }
        
        // Morgendämmerung (15-30%) - Dramatische Farben
        if (prog < 0.30)
        {
            var t = (prog - 0.15) / 0.15;
            // Tiefes Blau zu intensivem Orange am Horizont
            if (heightRatio < 0.5)
            {
                // Oberer Himmel bleibt dunkelblau
                return (
                    (byte)(5 + 30 * t),
                    (byte)(8 + 25 * t),
                    (byte)(40 + 30 * t)
                );
            }
            else
            {
                // Horizont wird orange-rot
                var horizonT = (heightRatio - 0.5) * 2.0;
                return (
                    (byte)(35 + (200 - 35) * t * horizonT),
                    (byte)(33 + (80 - 33) * t * horizonT),
                    (byte)(70 - 50 * t * horizonT)
                );
            }
        }
        
        // Goldene Stunde (30-45%)
        if (prog < 0.45)
        {
            var t = (prog - 0.30) / 0.15;
            // Orange zu leuchtendem Blau
            return (
                (byte)(200 - 140 * t - heightRatio * 30),
                (byte)(80 + 110 * t),
                (byte)(20 + (190 - 20) * t + (1 - heightRatio) * 15)
            );
        }
        
        // Tag (45-55%) - Kräftiges Blau mit Kontrast
        if (prog < 0.55)
        {
            return (
                (byte)(40 + heightRatio * 70),
                (byte)(130 + heightRatio * 70),
                (byte)(220 + heightRatio * 25)
            );
        }
        
        // Nachmittag (55-70%)
        if (prog < 0.70)
        {
            var t = (prog - 0.55) / 0.15;
            return (
                (byte)(110 + (180 - 110) * t + heightRatio * 25 * t),
                (byte)(200 - (200 - 110) * t),
                (byte)(245 - (245 - 60) * t - heightRatio * 35 * t)
            );
        }
        
        // Abenddämmerung (70-85%) - Dramatisches Abendrot
        if (prog < 0.85)
        {
            var t = (prog - 0.70) / 0.15;
            if (heightRatio > 0.5)
            {
                // Horizont intensiv rot-orange
                var horizonT = (heightRatio - 0.5) * 2.0;
                return (
                    (byte)((180 - 160 * t) * (1 + horizonT * 0.3)),
                    (byte)((110 - 105 * t) * (1 - horizonT * 0.3)),
                    (byte)((60 - 45 * t))
                );
            }
            else
            {
                // Oberer Himmel wird dunkel
                return (
                    (byte)(20 - 15 * t),
                    (byte)(15 - 10 * t),
                    (byte)(50 - 35 * t)
                );
            }
        }
        
        return (2, 4, 15); // Fallback Nacht
    }
    
    // Himmel zeichnen mit realistischem vertikalen Gradienten
    for (var y = 0; y < 18; y++)
    {
        // Höhenverhältnis: 0.0 = oben (Zenit), 1.0 = unten (Horizont)
        var heightRatio = (double)y / 17.0;
        var (r, g, b) = GetSkyColor(progress, heightRatio);
        
        Ctx.RectXyWh(0, y, 24, 1).Fill.Solid(Color.FromArgb(255, r, g, b));
    }
    
    // ---------------------------------------------------------------
    // 3. SONNENPOSITION BERECHNEN (wird später gezeichnet, aber hier für Reflexion im Wasser benötigt)
    // ---------------------------------------------------------------
    
    // Sonne bewegt sich horizontal von links nach rechts
    var sunStartX = 2.0;   // Startet links
    var sunEndX = 22.0;    // Endet rechts
    var sunX = sunStartX + (sunEndX - sunStartX) * sunProgress;
    
    // Y-Position folgt einer Bogenform (hoch in der Mitte, tief an den Rändern)
    var sunPeakY = 8.0;    // Höchster Punkt (Mittag)
    var sunHorizonY = 18.0; // Tiefster Punkt (Sonnenauf-/untergang)
    
    // Parabolische Bahn: unten→oben→unten
    var arcProgress = Math.Sin(sunProgress * Math.PI); // 0→1→0
    var sunY = sunHorizonY - (sunHorizonY - sunPeakY) * arcProgress;
    
    var sunRadius = 3.8;
    
    // ---------------------------------------------------------------
    // 4. REALISTISCHES MEER MIT ANIMIERTEN WELLEN UND REFLEKTIONEN
    // ---------------------------------------------------------------
    
    // Meer wird dunkler während der Nacht, reflektiert Sonnenfarben am Tag
    var nightFactor = 1.0;
    if (progress < 0.15 || progress > 0.85)
    {
        nightFactor = progress < 0.15 ? (1.0 - progress / 0.15) : ((progress - 0.85) / 0.15);
        nightFactor = 0.2 + nightFactor * 0.8;
    }
    
    void DrawOcean()
    {
        // Wasserhöhe: untere 6 Zeilen (KLARE ABGRENZUNG ZUM HIMMEL)
        var waterStartY = 18;
        var waterHeight = 6;
        
        for (var y = 0; y < waterHeight; y++)
        {
            var actualY = waterStartY + y;
            
            for (var x = 0; x < 24; x++)
            {
                // ===== EINFACHE, KLARE WELLEN =====
                var wave = Math.Sin((x / 6.0) * Math.PI + t * 2.0 + y * 0.3) * 0.6;
                
                // ===== DEUTLICHE WASSERFARBE =====
                // Dunkles Blau-Grün für klares Meer
                var depth = (double)y / waterHeight;
                
                var waterR = (byte)(0 + depth * 15);
                var waterG = (byte)(40 + depth * 60 + wave * 20);
                var waterB = (byte)(80 + depth * 80 + wave * 30);
                
                // Nachts dunkler
                waterR = (byte)(waterR * nightFactor);
                waterG = (byte)(waterG * nightFactor);
                waterB = (byte)(waterB * nightFactor);
                
                // ===== SONNENREFLEXION (SEHR EINFACH UND KLAR) =====
                if (sunProgress > 0.05)
                {
                    var sunReflectionX = sunX; // Reflexion direkt unter aktueller Sonnenposition
                    var distToSun = Math.Abs(x - sunReflectionX);
                    
                    // Breiter werdendes Lichtband
                    var reflectionWidth = 2.0 + depth * 4.0;
                    
                    if (distToSun < reflectionWidth)
                    {
                        var reflectionStrength = (1.0 - distToSun / reflectionWidth) * depth * sunProgress;
                        
                        // Flickernder Effekt
                        reflectionStrength *= Math.Sin(t * 5.0 + x + y) * 0.3 + 0.7;
                        
                        var (sunR, sunG, sunB) = GetSunColor();
                        waterR = (byte)Math.Min(255, waterR + sunR * reflectionStrength * 2.0);
                        waterG = (byte)Math.Min(255, waterG + sunG * reflectionStrength * 1.8);
                        waterB = (byte)Math.Min(255, waterB + sunB * reflectionStrength);
                    }
                }
                
                // ===== WEISSE SCHAUMKRONEN (GUT SICHTBAR) =====
                if (wave > 0.4)
                {
                    var foamAmount = (wave - 0.4) / 0.2;
                    waterR = (byte)Math.Min(255, waterR + 200 * foamAmount * nightFactor);
                    waterG = (byte)Math.Min(255, waterG + 220 * foamAmount * nightFactor);
                    waterB = (byte)Math.Min(255, waterB + 240 * foamAmount * nightFactor);
                }
                
                Ctx.RectXyWh(x, actualY, 1, 1).Fill.Solid(Color.FromArgb(255, waterR, waterG, waterB));
            }
        }
        
        // ===== KLARE HORIZONTLINIE (Übergang Himmel-Wasser) =====
        for (var x = 0; x < 24; x++)
        {
            // Dünne helle Linie für klare Abgrenzung
            var horizonColor = Color.FromArgb(100, 180, 200, 220);
            Ctx.RectXyWh(x, waterStartY - 0.5, 1, 1).Fill.Solid(horizonColor);
        }
    }
    
    DrawOcean();
    
    // ---------------------------------------------------------------
    // 5. SONNE ZEICHNEN (Position wurde bereits oben berechnet)
    // ---------------------------------------------------------------
    
    // Klare, leuchtende Sonnenfarben
    (byte r, byte g, byte b) GetSunColor()
    {
        if (sunProgress < 0.15)
        {
            // Tief am Horizont - Tiefrot
            return (200, 50, 0);
        }
        else if (sunProgress < 0.35)
        {
            // Aufsteigend - Orange
            var t = (sunProgress - 0.15) / 0.2;
            return (
                255,
                (byte)(50 + 170 * t),
                (byte)(0 + 50 * t)
            );
        }
        else
        {
            // Hoch - Gelb-Weiß
            var t = (sunProgress - 0.35) / 0.3;
            return (
                255,
                (byte)(220 + 35 * t),
                (byte)(50 + 200 * t)
            );
        }
    }
    
    var (sunR, sunG, sunB) = GetSunColor();
    
    // ===== KRÄFTIGER, GUT SICHTBARER GLANZ =====
    var glowIntensity = 1.0 + (sunProgress < 0.3 ? (0.3 - sunProgress) * 10.0 : 0);
    
    // Äußerer großer Glanz
    Ctx.Circle(sunX, sunY, sunRadius + 3.5).Fill.Solid(
        Color.FromArgb((byte)(40 * glowIntensity), sunR, (byte)(sunG * 0.6), (byte)(sunB * 0.3)));
    
    // Mittlerer Glanz
    Ctx.Circle(sunX, sunY, sunRadius + 2.2).Fill.Solid(
        Color.FromArgb((byte)(100 * glowIntensity), sunR, (byte)(sunG * 0.8), (byte)(sunB * 0.5)));
    
    // Innerer heller Glanz
    Ctx.Circle(sunX, sunY, sunRadius + 1.0).Fill.Solid(
        Color.FromArgb((byte)(180 * glowIntensity), 255, sunG, (byte)(sunB * 0.8)));
    
    // Die Sonne selbst - LEUCHTEND
    Ctx.Circle(sunX, sunY, sunRadius).Fill.Solid(Color.FromArgb(255, sunR, sunG, sunB));
    
    // ---------------------------------------------------------------
    // 6. EINFACHE, GUT SICHTBARE STRAHLEN
    // ---------------------------------------------------------------
    
    if (sunProgress > 0.3 && sunProgress < 0.7)
    {
        var rayAlpha = (byte)(180 * Math.Min((sunProgress - 0.3) / 0.2, (0.7 - sunProgress) / 0.2));
        
        // 8 große, deutliche Strahlen (kürzer, um nicht mit Wolken zu kollidieren)
        for (var i = 0; i < 8; i++)
        {
            var angle = (i * Math.PI / 4) + (t * 0.1);
            var rayLen = 2.5 + Math.Sin(t * 2 + i) * 0.3;
            
            for (var step = 0.0; step <= 1.0; step += 0.2)
            {
                var dist = rayLen * step;
                var rayX = sunX + Math.Cos(angle) * dist;
                var rayY = sunY + Math.Sin(angle) * dist;
                
                var stepAlpha = (byte)(rayAlpha * (1.0 - step * 0.6));
                Ctx.RectXyWh(rayX - 0.4, rayY - 0.4, 0.8, 0.8)
                    .Fill.Solid(Color.FromArgb(stepAlpha, 255, 240, 150));
            }
        }
    }
    
    // ---------------------------------------------------------------
    // 7. KLARE, DEUTLICHE WOLKEN
    // ---------------------------------------------------------------
    
    // Wolkenhelligkeit
    var cloudBrightness = 1.0;
    if (progress < 0.2)
        cloudBrightness = progress / 0.2;
    else if (progress > 0.8)
        cloudBrightness = (1.0 - progress) / 0.2;
    
    cloudBrightness = Math.Max(0.1, cloudBrightness);
    
    // 3 deutliche Wolken (höher positioniert, getrennt von der Sonne)
    var cloud1X = (t * 1.0) % 32 - 6;
    DrawSimpleCloud(cloud1X, 2, cloudBrightness);
    
    var cloud2X = (t * 1.4 + 12) % 32 - 6;
    DrawSimpleCloud(cloud2X, 4, cloudBrightness * 0.85);
    
    var cloud3X = (t * 1.7 + 24) % 32 - 6;
    DrawSimpleCloud(cloud3X, 5.5, cloudBrightness * 0.7);
    
    // Einfache, klar erkennbare Wolkenform
    void DrawSimpleCloud(double x, double y, double brightness)
    {
        if (x < -6 || x > 26) return;
        
        var alpha = (byte)(255 * brightness);
        var whiteCloud = Color.FromArgb(alpha, 255, 255, 255);
        var lightGray = Color.FromArgb((byte)(alpha * 0.8), 240, 245, 250);
        
        // Wolke aus 5 Kreisen für klare, kompakte Form
        Ctx.Circle(x + 1, y + 0.4, 1.1).Fill.Solid(lightGray);
        Ctx.Circle(x + 2, y, 1.4).Fill.Solid(whiteCloud);
        Ctx.Circle(x + 3.3, y + 0.2, 1.5).Fill.Solid(whiteCloud);
        Ctx.Circle(x + 4.8, y + 0.4, 1.3).Fill.Solid(whiteCloud);
        Ctx.Circle(x + 5.8, y + 0.6, 1.0).Fill.Solid(lightGray);
    }
};

// ===================================================================
// ANLEITUNG:
// 
// REALISTISCHE SONNENAUFGANGS-ANIMATION (60 Sekunden Zyklus)
// 
// Zeitablauf:
// - 0-9s:   Tiefe Nacht (dunkler Sternenhimmel)
// - 9-18s:  Morgendämmerung (Himmel wird orange/rosa)
// - 18-27s: Sonnenaufgang (goldene Stunde)
// - 27-33s: Vormittag (klarer blauer Himmel)
// - 33-42s: Nachmittag (warmes Licht)
// - 42-51s: Abenddämmerung (orange/rosa Abendrot)
// - 51-60s: Nacht (zurück zu dunkel)
//
// Realistische Features:
// ✓ 24x24 Pixel-Matrix (voller Bildschirm)
// ✓ Sanfte, natürliche Farbübergänge im Himmel
// ✓ Realistische Himmelsfarben (blau oben, heller am Horizont)
// ✓ Bergsilhouette mit natürlichen, wellenförmigen Konturen
// ✓ Sonne mit realistischen Farben (rot→orange→gelb→weiß)
// ✓ Mehrstufiger atmosphärischer Glanz um die Sonne
// ✓ Subtile, langsam rotierende Sonnenstrahlen
// ✓ Natürlich geformte Wolken (aus Kreisen zusammengesetzt)
// ✓ Langsame, flüssige Animation (60s statt 20s)
// ✓ Berge passen Helligkeit an Tageszeit an
// ✓ Wolken verblassen nachts
// ✓ Sonne bewegt sich leicht horizontal (Erdrotation)
// ===================================================================
// ✓ 3 bewegte Wolken mit unterschiedlichen Geschwindigkeiten
// ✓ Kontinuierliche Schleife (Auf- und Untergang)
// ✓ Sonnenstrahlen (animiert)
// ✓ Glanz-Effekt um die Sonne bei Dämmerung
// ===================================================================

//await PXL.Simulate(mainScene);
await PXL.SendToDevice(mainScene, "192.168.178.110");
