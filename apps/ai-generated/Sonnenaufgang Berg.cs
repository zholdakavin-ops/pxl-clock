#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// ===================================================================
// BERG-SONNENAUFGANG MIT REGEN UND WOLKEN
// Vollständige animierte Szene: Berge, Himmel, Sonne, Wolken, Regen, Blitze
// 24x24 Pixel-Matrix | Endlos wiederholbar | Modular aufgebaut
// ===================================================================

// ===================================================================
// GLOBALE VARIABLEN & KONFIGURATION
// Diese Werte steuern die gesamte Animation
// ===================================================================

// Animation Parameter
var width = 24;              // Pixel-Breite der Matrix
var cycleDuration = 120.0;   // Dauer eines vollständigen Tag-Nacht-Zyklus (in Sekunden)

// Regentropfen-Verwaltung
var raindrops = new List<(double x, double y, double speed, byte brightness, double lastTime)>();
var maxRaindrops = 25;       // Maximale Anzahl gleichzeitiger Regentropfen
var rainSpawnChance = 0.35;  // Wahrscheinlichkeit für neuen Tropfen (35%)

// Blitz-Verwaltung
var lastLightningTime = -10.0;
var lightningDuration = 0.2; // Wie lange ein Blitz sichtbar bleibt (Sekunden)
var lightningChance = 0.015; // Wahrscheinlichkeit für Blitz pro Frame (1.5%)

// Zufallsgenerator
var random = new Random();

// ===================================================================
// HAUPTSZENE - WIRD KONTINUIERLICH AUFGERUFEN
// Diese Funktion orchestriert alle Elemente der Animation
// ===================================================================

var mainScene = () =>
{
    // ---------------------------------------------------------------
    // 1. ZEITSTEUERUNG - ANIMATIONS-ZYKLUS
    // Berechnet den aktuellen Fortschritt im Tag-Nacht-Zyklus
    // ---------------------------------------------------------------
    
    // Aktuelle Zeit in Sekunden seit Mitternacht
    var currentTime = Ctx.Now.TimeOfDay.TotalSeconds;
    
    // Zeit innerhalb des Animations-Zyklus (wiederholt sich nach cycleDuration)
    var t = currentTime % cycleDuration;
    
    // Normalisierter Fortschritt: 0.0 = Zyklusstart, 1.0 = Zyklusende
    // Dieser Wert wird für alle zeitabhängigen Berechnungen verwendet
    var progress = t / cycleDuration;
    
    // Sonnenhöhe mit sanfter Easing-Kurve (macht den Sonnenaufgang natürlicher)
    // pow(x, 0.75) bedeutet: langsamer Start, schnelle Mitte, langsames Ende
    var sunProgress = progress < 0.5
        ? Math.Pow(progress * 2.0, 0.75)  // Aufgang (0-50%)
        : Math.Pow((1.0 - progress) * 2.0, 0.75);  // Untergang (50-100%)
    
    // ---------------------------------------------------------------
    // 2. HIMMEL MIT FARBVERLAUF ZEICHNEN
    // Dynamischer Gradient von Nacht über Dämmerung zu Tag
    // ---------------------------------------------------------------
    
    /// <summary>
    /// Berechnet die Himmelsfarbe basierend auf Tageszeit und Höhe
    /// - prog: Zeitfortschritt im Zyklus (0.0 - 1.0)
    /// - heightRatio: Vertikale Position (0.0 = oben/Zenit, 1.0 = unten/Horizont)
    /// Rückgabe: RGB-Farbtupel (r, g, b)
    /// </summary>
    (byte r, byte g, byte b) GetSkyColor(double prog, double heightRatio)
    {
        // === NACHT (0-15% und 85-100%) ===
        // Tiefes Nachtblau, dunkler oben, leicht heller am Horizont
        if (prog < 0.15 || prog > 0.85)
        {
            // Intensität der Nacht (1.0 = tiefste Nacht, 0.0 = Übergang zur Dämmerung)
            var nightIntensity = prog < 0.15 ? (1.0 - prog / 0.15) : ((prog - 0.85) / 0.15);
            
            // Grundfarben: sehr dunkles Blau
            var baseR = (byte)(3 + heightRatio * 10);    // Leicht rötlicher am Horizont
            var baseG = (byte)(5 + heightRatio * 15);    // Leicht grünlicher am Horizont
            var baseB = (byte)(20 + heightRatio * 30);   // Blau wird heller am Horizont
            
            return (
                (byte)(baseR * (0.3 + nightIntensity * 0.7)),
                (byte)(baseG * (0.3 + nightIntensity * 0.7)),
                (byte)(baseB * (0.3 + nightIntensity * 0.7))
            );
        }
        
        // === MORGENDÄMMERUNG (15-30%) ===
        // Übergang von Nachtblau zu dramatischem Violett und Orange
        if (prog < 0.30)
        {
            var dawnProgress = (prog - 0.15) / 0.15;  // 0.0 - 1.0 während Dämmerung
            
            if (heightRatio < 0.5)  // Oberer Himmel (Zenit)
            {
                // Bleibt dunkelblau mit violetten Tönen
                return (
                    (byte)(10 + 50 * dawnProgress),           // Leicht rötlicher
                    (byte)(10 + 30 * dawnProgress),           // Wenig Grün
                    (byte)(50 + 60 * dawnProgress)            // Blau dominant
                );
            }
            else  // Unterer Himmel (Horizont)
            {
                // Intensives Orange/Rosa am Horizont
                var horizonIntensity = (heightRatio - 0.5) * 2.0;  // 0.0 - 1.0
                return (
                    (byte)(40 + (220 - 40) * dawnProgress * horizonIntensity),
                    (byte)(30 + (100 - 30) * dawnProgress * horizonIntensity),
                    (byte)(70 - 50 * dawnProgress * horizonIntensity)
                );
            }
        }
        
        // === GOLDENE STUNDE / SONNENAUFGANG (30-45%) ===
        // Warme goldene und orange Töne
        if (prog < 0.45)
        {
            var goldenProgress = (prog - 0.30) / 0.15;
            
            // Übergang von Orange zu Hellblau
            return (
                (byte)(220 - 120 * goldenProgress - heightRatio * 40),
                (byte)(100 + 120 * goldenProgress + heightRatio * 20),
                (byte)(40 + 180 * goldenProgress + (1 - heightRatio) * 30)
            );
        }
        
        // === TAG (45-55%) ===
        // Klarer, heller blauer Himmel
        if (prog < 0.55)
        {
            return (
                (byte)(60 + heightRatio * 50),         // Heller am Horizont
                (byte)(150 + heightRatio * 60),        // Kräftiges Cyan-Blau
                (byte)(240 + heightRatio * 15)         // Sehr helles Blau
            );
        }
        
        // === NACHMITTAG (55-70%) ===
        // Himmel beginnt wärmer zu werden
        if (prog < 0.70)
        {
            var afternoonProgress = (prog - 0.55) / 0.15;
            
            return (
                (byte)(110 + (200 - 110) * afternoonProgress + heightRatio * 30 * afternoonProgress),
                (byte)(210 - (210 - 120) * afternoonProgress),
                (byte)(255 - (255 - 80) * afternoonProgress - heightRatio * 40 * afternoonProgress)
            );
        }
        
        // === ABENDDÄMMERUNG (70-85%) ===
        // Dramatisches Abendrot und Sonnenuntergang
        if (prog < 0.85)
        {
            var duskProgress = (prog - 0.70) / 0.15;
            
            if (heightRatio > 0.5)  // Horizont
            {
                // Intensives Rot-Orange am Horizont
                var horizonIntensity = (heightRatio - 0.5) * 2.0;
                return (
                    (byte)((200 - 180 * duskProgress) * (1 + horizonIntensity * 0.4)),
                    (byte)((120 - 110 * duskProgress) * (1 - horizonIntensity * 0.4)),
                    (byte)((80 - 60 * duskProgress))
                );
            }
            else  // Oberer Himmel
            {
                // Wird dunkel
                return (
                    (byte)(30 - 25 * duskProgress),
                    (byte)(20 - 15 * duskProgress),
                    (byte)(60 - 45 * duskProgress)
                );
            }
        }
        
        // Fallback: Tiefe Nacht
        return (3, 5, 20);
    }
    
    // Zeichne Himmel: obere 18 Zeilen (Zeilen 0-17)
    // Die unteren 6 Zeilen (18-23) sind für die Berge reserviert
    for (var y = 0; y < 18; y++)
    {
        // Höhenverhältnis: 0.0 = oben (Zenit), 1.0 = unten (kurz über Bergen/Horizont)
        var heightRatio = (double)y / 17.0;
        
        // Hole Farbe für diese Höhe und Tageszeit
        var (r, g, b) = GetSkyColor(progress, heightRatio);
        
        // Zeichne horizontale Linie über die gesamte Breite
        for (var x = 0; x < width; x++)
        {
            Ctx.RectXyWh(x, y, 1, 1).Fill.Solid(Color.FromArgb(255, r, g, b));
        }
    }
    
    // ---------------------------------------------------------------
    // 3. BERGE ZEICHNEN - STRUKTURIERTE SILHOUETTE
    // Mehrschichtige Berge mit Zacken, Farbvariation und Schneekapppen
    // ---------------------------------------------------------------
    
    /// <summary>
    /// Zeichnet eine Bergschicht mit natürlichem Profil
    /// - startY: Startzeile für diese Bergschicht
    /// - height: Maximale Höhe der Berge
    /// - baseColor: Grundfarbe (Grau/Braun)
    /// - scale: Skalierung für Wellenmuster (größer = breitere Berge)
    /// </summary>
    void DrawMountainLayer(int startY, int height, (byte r, byte g, byte b) baseColor, double scale)
    {
        // Erstelle Bergprofil mit Sinuswellen für natürliche Form
        var peaks = new double[width];
        
        for (var x = 0; x < width; x++)
        {
            // Hauptwelle: breite Bergformen
            var mainWave = Math.Sin(x * 0.4 * scale) * height * 0.5;
            
            // Detailwelle: kleinere Zacken und Variationen
            var detailWave = Math.Sin(x * 1.5 * scale + 2.7) * height * 0.35;
            
            // Kombiniere Wellen für komplexes Profil
            peaks[x] = startY + height - (mainWave + detailWave);
        }
        
        // Zeichne Berge pixel für pixel
        for (var x = 0; x < width; x++)
        {
            var peakY = (int)Math.Round(peaks[x]);
            
            // Zeichne von Gipfel bis zum unteren Rand (height=24)
            for (var y = peakY; y < height; y++)
            {
                // Farbvariation für Struktur und Textur
                var variance = (x + y) % 3 - 1;  // -1, 0, oder +1
                var r = (byte)Math.Clamp(baseColor.r + variance * 10, 0, 255);
                var g = (byte)Math.Clamp(baseColor.g + variance * 10, 0, 255);
                var b = (byte)Math.Clamp(baseColor.b + variance * 10, 0, 255);
                
                // Anpassung an Tageszeit (dunkler bei Nacht)
                if (progress < 0.15 || progress > 0.85)
                {
                    var nightDarken = 0.3;
                    r = (byte)(r * nightDarken);
                    g = (byte)(g * nightDarken);
                    b = (byte)(b * nightDarken);
                }
                
                Ctx.RectXyWh(x, y, 1, 1).Fill.Solid(Color.FromArgb(255, r, g, b));
            }
        }
        
        // Schneekappen auf hohen Gipfeln
        for (var x = 0; x < width; x++)
        {
            var peakY = (int)Math.Round(peaks[x]);
            
            // Nur auf den höchsten Gipfeln (1-2 Pixel an der Spitze)
            if (peakY < startY + height / 3)
            {
                // Heller Schnee mit leichter Transparenz
                Ctx.RectXyWh(x, peakY, 1, 1).Fill.Solid(
                    Color.FromArgb(220, 250, 250, 255)
                );
                
                // Etwas Schnee auch eine Zeile darunter
                if (peakY + 1 < height && random.Next(2) == 0)
                {
                    Ctx.RectXyWh(x, peakY + 1, 1, 1).Fill.Solid(
                        Color.FromArgb(150, 240, 240, 250)
                    );
                }
            }
        }
    }
    
    // Zeichne 3 Bergschichten für Tiefeneffekt
    // Hintere Berge (dunkler, weiter weg)
    DrawMountainLayer(18, 6, (50, 45, 55), 0.8);
    
    // Mittlere Berge
    DrawMountainLayer(19, 5, (70, 65, 75), 1.0);
    
    // Vordere Berge (heller, näher, höher)
    DrawMountainLayer(20, 4, (90, 85, 95), 1.3);
    
    // ---------------------------------------------------------------
    // 4. SONNE ZEICHNEN - AUFGEHEND MIT GLANZ
    // ---------------------------------------------------------------
    
    /// <summary>
    /// Berechnet Sonnenfarbe basierend auf Höhe (sunProgress)
    /// Niedrig = Rot, Mittel = Orange, Hoch = Gelb-Weiß
    /// </summary>
    (byte r, byte g, byte b) GetSunColor()
    {
        if (sunProgress < 0.2)
        {
            // Tief am Horizont - Tiefrot
            return (255, 70, 20);
        }
        else if (sunProgress < 0.45)
        {
            // Aufsteigend - Orange
            var orangeProgress = (sunProgress - 0.2) / 0.25;
            return (
                255,
                (byte)(70 + 160 * orangeProgress),
                (byte)(20 + 100 * orangeProgress)
            );
        }
        else
        {
            // Hoch am Himmel - Gelb-Weiß
            var yellowProgress = (sunProgress - 0.45) / 0.55;
            return (
                255,
                (byte)(230 + 25 * yellowProgress),
                (byte)(120 + 135 * yellowProgress)
            );
        }
    }
    
    // Sonne nur zeichnen wenn über dem Horizont (Zeile 18)
    if (sunProgress > 0.05)
    {
        // Sonnenposition: vertikal von unten nach oben
        var sunX = 12.0;  // Zentrum horizontal (24/2)
        var sunY = 18.0 - sunProgress * 11.0;  // Von Y=18 (Horizont) zu Y=7 (hoch)
        var sunRadius = 2.2;
        
        var (sunR, sunG, sunB) = GetSunColor();
        
        // Sonnenglow - mehrere Schichten für weichen Übergang
        // Äußerer großer Glow (atmosphärischer Effekt)
        Ctx.Circle(sunX, sunY, sunRadius + 2.0).Fill.Solid(
            Color.FromArgb(30, sunR, (byte)(sunG * 0.7), (byte)(sunB * 0.4))
        );
        
        // Mittlerer Glow
        Ctx.Circle(sunX, sunY, sunRadius + 1.2).Fill.Solid(
            Color.FromArgb(80, sunR, (byte)(sunG * 0.85), (byte)(sunB * 0.6))
        );
        
        // Innerer heller Glow
        Ctx.Circle(sunX, sunY, sunRadius + 0.6).Fill.Solid(
            Color.FromArgb(150, 255, sunG, (byte)(sunB * 0.9))
        );
        
        // Sonnenkern (leuchtend hell)
        Ctx.Circle(sunX, sunY, sunRadius).Fill.Solid(
            Color.FromArgb(255, sunR, sunG, sunB)
        );
        
        // Sonnenstrahlen (nur bei mittlerer bis hoher Position)
        if (sunProgress > 0.3 && sunProgress < 0.8)
        {
            var rayCount = 8;
            var rayLength = 1.2 + Math.Sin(currentTime * 0.8) * 0.3;
            var rayRotation = currentTime * 0.05;  // Langsame Rotation
            
            for (var i = 0; i < rayCount; i++)
            {
                var angle = (i / (double)rayCount) * Math.PI * 2 + rayRotation;
                
                // Strahlrichtung
                var rayEndX = sunX + Math.Cos(angle) * (sunRadius + rayLength + 0.8);
                var rayEndY = sunY + Math.Sin(angle) * (sunRadius + rayLength + 0.8);
                
                // Zeichne Strahl als kleine Punkte
                for (var step = 0.0; step <= 1.0; step += 0.25)
                {
                    var rayX = sunX + Math.Cos(angle) * (sunRadius + 0.8 + rayLength * step);
                    var rayY = sunY + Math.Sin(angle) * (sunRadius + 0.8 + rayLength * step);
                    
                    var rayAlpha = (byte)(120 * (1.0 - step * 0.7) * sunProgress);
                    
                    Ctx.RectXyWh(rayX - 0.3, rayY - 0.3, 0.6, 0.6).Fill.Solid(
                        Color.FromArgb(rayAlpha, 255, 245, 200)
                    );
                }
            }
        }
    }
    
    // ---------------------------------------------------------------
    // 5. WOLKEN ZEICHNEN UND ANIMIEREN
    // Wolken bewegen sich kontinuierlich von links nach rechts
    // ---------------------------------------------------------------
    
    /// <summary>
    /// Zeichnet eine einzelne Wolke aus überlappenden Kreisen
    /// - x, y: Position der Wolke
    /// - brightness: Helligkeit (0.0 - 1.0), verblasst nachts
    /// - phase: Zeitversatz für Pulsation
    /// - sunX, sunY: Position der Sonne für Abstandsprüfung
    /// </summary>
    void DrawCloud(double x, double y, double brightness, double phase, double sunX, double sunY)
    {
        // Wolke außerhalb des sichtbaren Bereichs? Überspringen
        if (x < -4 || x > width + 2) return;
        
        // PRÜFE ABSTAND ZUR SONNE - Wolken dürfen nicht zu nah an Sonne sein
        var distanceToSun = Math.Sqrt(Math.Pow(x + 1.5 - sunX, 2) + Math.Pow(y - sunY, 2));
        var minDistance = 6.5;  // Mindestabstand in Pixeln
        
        // Wenn Wolke zu nah an Sonne ist, NICHT zeichnen
        if (sunProgress > 0.05 && distanceToSun < minDistance)
        {
            return;  // Wolke wird übersprungen
        }
        
        // Leichte Pulsation (Wolke "atmet") - reduziert für kleinere Wolken
        var pulse = Math.Sin(currentTime * 0.4 + phase) * 0.05 + 1.0;
        
        // Wolkenfarbe (weiß/hellgrau) - VOLLSTÄNDIG DECKEND
        var cloudAlpha = (byte)(255 * brightness);  // Volle Deckkraft
        
        // Kleinere Wolke aus 5 überlappenden Kreisen
        // Alle Radien reduziert für kompaktere Form
        
        Ctx.Circle(x, y, 0.7 * pulse).Fill.Solid(
            Color.FromArgb(cloudAlpha, 240, 245, 250)
        );
        
        Ctx.Circle(x + 1.0, y - 0.2, 0.85 * pulse).Fill.Solid(
            Color.FromArgb(cloudAlpha, 255, 255, 255)
        );
        
        Ctx.Circle(x + 1.9, y, 0.8 * pulse).Fill.Solid(
            Color.FromArgb(cloudAlpha, 250, 252, 255)
        );
        
        Ctx.Circle(x + 2.7, y + 0.15, 0.65 * pulse).Fill.Solid(
            Color.FromArgb(cloudAlpha, 245, 248, 252)
        );
        
        Ctx.Circle(x + 1.4, y + 0.5, 0.6 * pulse).Fill.Solid(
            Color.FromArgb(cloudAlpha, 235, 240, 245)
        );
    }
    
    // Wolkenhelligkeit abhängig von Tageszeit
    var cloudBrightness = 1.0;
    if (progress < 0.2)
        cloudBrightness = progress / 0.2;  // Fade in bei Sonnenaufgang
    else if (progress > 0.8)
        cloudBrightness = (1.0 - progress) / 0.2;  // Fade out bei Sonnenuntergang
    
    cloudBrightness = Math.Max(0.15, cloudBrightness);
    
    // Berechne Sonnenposition für Wolken-Kollisionsvermeidung
    var cloudSunX = 12.0;  // Zentrum horizontal (24/2)
    var cloudSunY = sunProgress > 0.05 ? 18.0 - sunProgress * 11.0 : 25.0;  // Wenn Sonne unter Horizont, weit weg
    
    // Zeichne 4 Wolken mit großem Abstand zueinander UND zur Sonne
    // Verschiedene Geschwindigkeiten und zeitliche Versätze verhindern Überlappung
    
    // Wolke 1: Oben links, sehr langsam
    var cloud1X = (currentTime * 0.04) % (width + 12) - 6;
    DrawCloud(cloud1X, 3.0, cloudBrightness, 0, cloudSunX, cloudSunY);
    
    // Wolke 2: Mitte rechts, mittelschnell, großer zeitlicher Versatz
    var cloud2X = (currentTime * 0.095 + width * 1.8) % (width + 12) - 6;
    DrawCloud(cloud2X, 7.5, cloudBrightness * 0.85, 2.5, cloudSunX, cloudSunY);
    
    // Wolke 3: Unten, schnell, maximaler zeitlicher Versatz
    var cloud3X = (currentTime * 0.13 + width * 3.2) % (width + 12) - 6;
    DrawCloud(cloud3X, 13.0, cloudBrightness * 0.75, 5.0, cloudSunX, cloudSunY);
    
    // Wolke 4: Oben Mitte, langsam, mittlerer Versatz
    var cloud4X = (currentTime * 0.055 + width * 2.4) % (width + 12) - 6;
    DrawCloud(cloud4X, 5.0, cloudBrightness * 0.9, 7.5, cloudSunX, cloudSunY);
    
    // ---------------------------------------------------------------
    // 6. REGEN - FALLENDE TROPFEN VON WOLKEN
    // Tropfen spawnen unter Wolken, fallen mit unterschiedlichen Geschwindigkeiten
    // ---------------------------------------------------------------
    
    // Regen nur während bestimmten Zeitfenstern (30-50% des Zyklus)
    var isRainTime = progress > 0.3 && progress < 0.5;
    
    if (isRainTime)
    {
        // Neue Regentropfen spawnen (zufällig unter Wolken)
        if (raindrops.Count < maxRaindrops && random.NextDouble() < rainSpawnChance)
        {
            // Wähle zufällige Wolke (jetzt 4 kleinere Wolken)
            var cloudIndex = random.Next(4);
            double cloudX = 0;
            double cloudY = 0;
            
            if (cloudIndex == 0) { cloudX = cloud1X + 1.5; cloudY = 3.0; }
            else if (cloudIndex == 1) { cloudX = cloud2X + 1.5; cloudY = 7.5; }
            else if (cloudIndex == 2) { cloudX = cloud3X + 1.5; cloudY = 13.0; }
            else { cloudX = cloud4X + 1.5; cloudY = 5.0; }
            
            // Nur spawnen wenn Wolke sichtbar ist
            if (cloudX >= 0 && cloudX < width)
            {
                // Zufällige horizontale Streuung unter der Wolke
                var spawnX = cloudX + (random.NextDouble() - 0.5) * 3.0;
                var spawnY = cloudY + 1.2;
                
                // Zufällige Fallgeschwindigkeit
                var speed = 2.5 + random.NextDouble() * 2.0;  // 2.5 - 4.5 Pixel/Sekunde
                
                // Zufällige Helligkeit
                var dropBrightness = (byte)(140 + random.Next(70));
                
                raindrops.Add((spawnX, spawnY, speed, dropBrightness, currentTime));
            }
        }
    }
    
    // Aktualisiere und zeichne alle Regentropfen
    for (var i = raindrops.Count - 1; i >= 0; i--)
    {
        var drop = raindrops[i];
        
        // Berechne neue Position basierend auf vergangener Zeit
        var deltaTime = currentTime - drop.lastTime;
        var newY = drop.y + drop.speed * deltaTime * 0.016;  // 60 FPS Normalisierung
        
        // Aktualisiere Tropfen
        raindrops[i] = (drop.x, newY, drop.speed, drop.brightness, currentTime);
        
        // Entferne Tropfen wenn:
        // 1. Sie den Boden erreicht haben (Y >= 11, Berge beginnen)
        // 2. Sie außerhalb des Bildschirms sind
        if (newY >= 11.0 || drop.x < 0 || drop.x >= width)
        {
            raindrops.RemoveAt(i);
            continue;
        }
        
        // Zeichne Regentropfen als kleine vertikale Striche
        // Länge variiert leicht für Bewegungseffekt
        var dropLength = 1.0 + drop.speed / 10.0;
        
        Ctx.RectXyWh(drop.x - 0.2, newY, 0.4, dropLength).Fill.Solid(
            Color.FromArgb(180, drop.brightness, drop.brightness, (byte)(drop.brightness + 40))
        );
        
        // Kleiner Schatten/Spur für Bewegungseffekt
        if (newY > 0.8)
        {
            Ctx.RectXyWh(drop.x - 0.15, newY - 0.7, 0.3, 0.5).Fill.Solid(
                Color.FromArgb(60, drop.brightness, drop.brightness, drop.brightness)
            );
        }
    }
    
    // ---------------------------------------------------------------
    // 7. BLITZE - ZUFÄLLIGE LICHTEFFEKTE
    // Kurze, helle Blitze während der Regenzeit
    // ---------------------------------------------------------------
    
    if (isRainTime)
    {
        // Prüfe ob neuer Blitz spawnen soll
        // Mindestens 2 Sekunden zwischen Blitzen
        if (currentTime - lastLightningTime > 2.0 && random.NextDouble() < lightningChance)
        {
            lastLightningTime = currentTime;
        }
        
        // Zeichne aktiven Blitz (nur kurz sichtbar)
        var timeSinceLightning = currentTime - lastLightningTime;
        if (timeSinceLightning < lightningDuration)
        {
            // Blitz-Fortschritt (0.0 = Beginn, 1.0 = Ende)
            var blitzProgress = timeSinceLightning / lightningDuration;
            
            // Intensität: schnell aufblitzen, langsam verblassen
            var intensity = Math.Pow(1.0 - blitzProgress, 2.0);
            var blitzAlpha = (byte)(255 * intensity);
            
            // Zufällige Position (wird beim Spawnen festgelegt)
            var blitzX = ((int)(lastLightningTime * 1000) % width);
            var blitzY = 3.0 + ((int)(lastLightningTime * 1000) % 4);
            
            // Blitz besteht aus mehreren verzweigten Segmenten
            // Hauptblitz von Wolke nach unten
            for (var segment = 0; segment < 4; segment++)
            {
                var segmentY = blitzY + segment * 2.0;
                var segmentX = blitzX + (random.NextDouble() - 0.5) * 1.2;
                
                // Blitzsegment zeichnen
                Ctx.RectXyWh(segmentX - 0.3, segmentY, 0.6, 1.8).Fill.Solid(
                    Color.FromArgb(blitzAlpha, 255, 255, 230)
                );
                
                // Zusätzlicher Glow um Blitz
                Ctx.RectXyWh(segmentX - 0.8, segmentY - 0.3, 1.6, 2.4).Fill.Solid(
                    Color.FromArgb((byte)(blitzAlpha * 0.4), 255, 255, 200)
                );
            }
            
            // Blitzleuchten: Himmel wird kurz aufgehellt
            for (var y = 0; y < 18; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Ctx.RectXyWh(x, y, 1, 1).Fill.Solid(
                        Color.FromArgb((byte)(blitzAlpha * 0.12), 255, 255, 255)
                    );
                }
            }
        }
    }
    
    // ---------------------------------------------------------------
    // 8. NEBEL AUF BERGEN - ATMOSPHÄRISCHER EFFEKT (OPTIONAL)
    // Subtile Nebelschwaden für mehr Tiefe
    // ---------------------------------------------------------------
    
    // Nebel nur bei bestimmten Tageszeiten (Morgen/Abend)
    if ((progress > 0.15 && progress < 0.35) || (progress > 0.65 && progress < 0.85))
    {
        // Nebelintensität
        var fogIntensity = 1.0;
        if (progress < 0.35)
            fogIntensity = Math.Min((progress - 0.15) / 0.1, (0.35 - progress) / 0.1);
        else
            fogIntensity = Math.Min((progress - 0.65) / 0.1, (0.85 - progress) / 0.1);
        
        fogIntensity = Math.Clamp(fogIntensity, 0.0, 1.0);
        
        // 3 Nebelbänder, die sich langsam bewegen
        for (var i = 0; i < 3; i++)
        {
            var fogX = ((currentTime * 0.04 + i * 5.5) % (width + 6)) - 3;
            var fogY = 17.0 + Math.Sin(currentTime * 0.3 + i * 2) * 0.8;
            var fogSize = 2.8 + Math.Cos(currentTime * 0.35 + i * 1.5) * 0.5;
            
            // Nebel als transparente, weiche Kreise
            Ctx.Circle(fogX, fogY, fogSize).Fill.Solid(
                Color.FromArgb((byte)(50 * fogIntensity), 220, 225, 230)
            );
            
            Ctx.Circle(fogX + 1.5, fogY + 0.4, fogSize * 0.7).Fill.Solid(
                Color.FromArgb((byte)(35 * fogIntensity), 230, 235, 240)
            );
        }
    }
};

// ===================================================================
// DOKUMENTATION & ANLEITUNG
// ===================================================================
//
// BERG-SONNENAUFGANG MIT REGEN - KOMPLETTE ANIMATIONSSZENE
// 
// Diese Animation zeigt eine vollständige Tag-Nacht-Szene auf einer
// 24x24 Pixel-Matrix mit folgenden Elementen:
//
// 1. HIMMEL - Dynamischer Farbverlauf
//    - Nacht: Tiefes Nachtblau
//    - Dämmerung: Violett und dramatisches Orange
//    - Tag: Heller blauer Himmel
//    - Abend: Intensives Abendrot
//
// 2. BERGE - Mehrschichtige Silhouette
//    - 3 Bergschichten für Tiefeneffekt
//    - Natürliche Zackenprofile
//    - Schneekappen auf hohen Gipfeln
//    - Farbvariationen für Textur
//    - Anpassung an Tageszeit
//
// 3. SONNE - Aufgehender gelber Kreis
//    - Vertikale Bewegung: vom Horizont nach oben
//    - Farbwechsel: Rot → Orange → Gelb → Weiß
//    - Mehrschichtiger atmosphärischer Glow
//    - Rotierende Sonnenstrahlen (bei mittlerer Höhe)
//
// 4. WOLKEN - Bewegliche weiße Pixel-Objekte
//    - 3 Wolken mit unterschiedlichen Geschwindigkeiten
//    - Kontinuierliche Bewegung von links nach rechts
//    - Leichte Pulsation ("Atmung")
//    - Verblassen bei Nacht
//
// 5. REGEN - Fallende animierte Tropfen
//    - Spawnen unter Wolken (30-50% des Zyklus)
//    - Unterschiedliche Fallgeschwindigkeiten (2.5-4.5 Pixel/s)
//    - Bis zu 25 gleichzeitige Tropfen
//    - Bewegungsspur für Realismus
//
// 6. BLITZE - Zufällige Lichteffekte
//    - Nur während Regenzeit
//    - Kurze, helle Aufblitze (0.2s)
//    - Verzweigte Blitzlinien
//    - Aufhellung des gesamten Himmels
//
// 7. NEBEL - Atmosphärischer Effekt
//    - Subtile Nebelschwaden auf Bergen
//    - Nur bei Dämmerung (Morgen/Abend)
//    - Langsame horizontale Bewegung
//
// ZEITABLAUF (120 Sekunden Zyklus):
// - 0-18s:   Tiefe Nacht
// - 18-36s:  Morgendämmerung (violett/orange)
// - 36-54s:  Sonnenaufgang (goldene Stunde) + REGEN
// - 54-66s:  Tag (klarer Himmel)
// - 66-84s:  Nachmittag
// - 84-102s: Abenddämmerung (abendrot)
// - 102-120s: Zurück zur Nacht
//
// TECHNISCHE DETAILS:
// - Matrix-Größe: 16x16 Pixel
// - Frame-Rate: ~60 FPS
// - Zyklus-Dauer: 120 Sekunden (konfigurierbar)
// - Modular aufgebaut: Jedes Element ist eigene Funktion
// - Ausführlich kommentiert für einfache Anpassung
//
// ANPASSUNGSMÖGLICHKEITEN:
// - cycleDuration: Gesamtdauer des Animations-Zyklus
// - maxRaindrops: Maximale Anzahl Regentropfen
// - rainSpawnChance: Wahrscheinlichkeit für neue Tropfen
// - lightningChance: Wahrscheinlichkeit für Blitze
// - Bergfarben in DrawMountainLayer anpassen
// - Wolkengeschwindigkeiten ändern (cloud1X, cloud2X, cloud3X)
//
// ===================================================================

//await PXL.Simulate(mainScene);
await PXL.SendToDevice(mainScene, "192.168.178.110");
