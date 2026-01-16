#:package Pxl@0.0.34

using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.DrawingContext;

// Side-scrolling jump and run mit Feuerwerk am Ende
var mainScene = () =>
{
    // Dark green background
    Ctx.RectXyWh(0, 0, 24, 24).Fill.Solid(Color.FromArgb(255, 0, 80, 0));
    
    var t = Ctx.Now.TimeOfDay.TotalSeconds;
    
    // Level duration - nach 15 Sekunden kommt Feuerwerk
    var levelDuration = 15.0;
    var cycleTime = t % (levelDuration + 3.0); // +3 Sekunden für Feuerwerk
    
    if (cycleTime < levelDuration)
    {
        // === SPIEL-PHASE ===
        var gameTime = cycleTime;
        
        // Scrolling speed - langsamer für flüssigere Animation ohne Bugs
        var scrollSpeed = 2.5;
        var worldOffset = gameTime * scrollSpeed;
        
        // Übersichtliche Plattformen mit besseren Abständen
        var platforms = new[]
        {
            new { X = 0.0, Y = 18.0, Width = 7.0, Height = 2.0 },
            new { X = 10.0, Y = 16.0, Width = 5.0, Height = 2.0 },
            new { X = 18.0, Y = 14.0, Width = 6.0, Height = 2.0 },
            new { X = 27.0, Y = 16.0, Width = 5.0, Height = 2.0 },
            new { X = 35.0, Y = 18.0, Width = 6.0, Height = 2.0 },
            new { X = 44.0, Y = 15.0, Width = 7.0, Height = 2.0 },
            new { X = 54.0, Y = 17.0, Width = 8.0, Height = 2.0 } // Ziel-Plattform
        };
        
        // Zeichne realistische Plattformen mit 3D-Effekt
        var platformIndex = 0;
        foreach (var platform in platforms)
        {
            var screenX = platform.X - worldOffset;
            
            // Nur zeichnen wenn sichtbar
            if (screenX > -platform.Width && screenX < 24)
            {
                // Realistische Stein-Plattformen
                var baseColor = Color.FromArgb(255, 80, 80, 90); // Dunkelgrau/Blau
                
                // Schatten unter der Plattform
                Ctx.RectXyWh(screenX + 0.3, platform.Y + platform.Height, platform.Width - 0.3, 0.8)
                    .Fill.Solid(Color.FromArgb(100, 0, 0, 0));
                
                // Hauptplattform (Stein-Textur)
                Ctx.RectXyWh(screenX, platform.Y, platform.Width, platform.Height)
                    .Fill.Solid(baseColor);
                
                // 3D-Effekt: Rechte Seite dunkler (Tiefe)
                Ctx.RectXyWh(screenX + platform.Width - 0.4, platform.Y, 0.4, platform.Height)
                    .Fill.Solid(Color.FromArgb(255, 50, 50, 60));
                
                // Obere Kante heller (Licht)
                Ctx.RectXyWh(screenX, platform.Y, platform.Width, 0.4)
                    .Fill.Solid(Color.FromArgb(255, 110, 110, 120));
                
                // ZIEL-PLATTFORM: Letzte Plattform mit ZIEL-Flagge
                if (platformIndex == platforms.Length - 1)
                {
                    // Goldene Ziel-Plattform
                    Ctx.RectXyWh(screenX, platform.Y, platform.Width, platform.Height)
                        .Fill.Solid(Color.FromArgb(255, 200, 150, 0));
                    Ctx.RectXyWh(screenX, platform.Y, platform.Width, 0.4)
                        .Fill.Solid(Color.FromArgb(255, 255, 200, 50));
                    
                    // Zielflagge in der Mitte der Plattform
                    var flagX = screenX + platform.Width / 2.0;
                    var flagY = platform.Y - 5.0;
                    
                    // Fahnenstange (schwarz)
                    Ctx.RectXyWh(flagX - 0.15, flagY, 0.3, 5.0)
                        .Fill.Solid(Color.FromArgb(255, 50, 50, 50));
                    
                    // Flagge (schwarz-weiß kariert)
                    var flagWidth = 3.0;
                    var flagHeight = 2.0;
                    // Weiße Basis
                    Ctx.RectXyWh(flagX, flagY, flagWidth, flagHeight)
                        .Fill.Solid(Colors.White);
                    // Schwarze Karos
                    Ctx.RectXyWh(flagX, flagY, flagWidth/2, flagHeight/2)
                        .Fill.Solid(Colors.Black);
                    Ctx.RectXyWh(flagX + flagWidth/2, flagY + flagHeight/2, flagWidth/2, flagHeight/2)
                        .Fill.Solid(Colors.Black);
                    
                    // "ZIEL" Text über der Flagge (pulst)
                    var textPulse = Math.Sin(gameTime * 4) * 0.3 + 1.0;
                }
                else
                {
                    // Normale Stein-Textur: kleine Punkte/Risse
                    var random = new Random((int)(platform.X * 100));
                    for (var i = 0; i < 3; i++)
                    {
                        var dotX = screenX + random.NextDouble() * platform.Width;
                        var dotY = platform.Y + random.NextDouble() * platform.Height;
                        Ctx.Circle(dotX, dotY, 0.2).Fill.Solid(Color.FromArgb(150, 60, 60, 70));
                    }
                }
            }
            platformIndex++;
        }
        
        // Spieler bleibt bei X=6 - Batman ist größer für bessere Sichtbarkeit
        var playerX = 6.0;
        var playerWidth = 3.0;  // Größer für bessere Erkennbarkeit
        var playerHeight = 4.5; // Größer
        
        // Finde aktuelle und nächste Plattform
        var currentPlatform = platforms[0];
        var nextPlatform = platforms[0];
        
        foreach (var platform in platforms)
        {
            var platformScreenX = platform.X - worldOffset;
            // Finde Plattform unter dem Spieler
            if (playerX >= platformScreenX - 1 && playerX <= platformScreenX + platform.Width + 1)
            {
                currentPlatform = platform;
            }
            // Finde nächste Plattform
            if (platformScreenX > playerX && platformScreenX < playerX + 10)
            {
                nextPlatform = platform;
                break;
            }
        }
        
        var currentPlatformScreenX = currentPlatform.X - worldOffset;
        var nextPlatformScreenX = nextPlatform.X - worldOffset;
        
        // Berechne ob Spieler springen muss
        var needsToJump = false;
        var jumpProgress = 0.0;
        
        // Spieler ist am Ende der aktuellen Plattform und muss zur nächsten
        var platformEndX = currentPlatformScreenX + currentPlatform.Width;
        var gapToNextPlatform = nextPlatformScreenX - platformEndX;
        
        // Springe nur wenn wirklich am Rand und es eine Lücke gibt
        if (playerX > platformEndX - 1.5 && gapToNextPlatform > 0.5)
        {
            needsToJump = true;
            // Sehr schneller Sprung - nur 1.5 Pixel Distanz!
            var distanceToNextPlatform = nextPlatformScreenX - playerX;
            jumpProgress = 1.0 - Math.Max(0, Math.Min(1, distanceToNextPlatform / 1.5));
        }
        
        // Batman steht mit den Füßen GENAU AUF der Plattform-Oberkante
        var playerY = currentPlatform.Y - playerHeight;
        
        if (needsToJump && jumpProgress > 0)
        {
            // Springe von aktueller zu nächster Plattform
            var startY = currentPlatform.Y - playerHeight;
            var endY = nextPlatform.Y - playerHeight;
            
            // Sehr niedrige, schnelle Sprünge
            var heightDiff = Math.Abs(endY - startY);
            var jumpHeight = Math.Max(1.5, heightDiff + 0.8);
            
            // Parabel für den Sprung - sehr flach und schnell
            var arc = -4.0 * jumpHeight * jumpProgress * (jumpProgress - 1.0);
            playerY = startY + (endY - startY) * jumpProgress - arc;
        }
        
        // Batman Farben
        var batmanBlack = Color.FromArgb(255, 20, 20, 25);
        var batmanGray = Color.FromArgb(255, 60, 60, 70);
        var batmanYellow = Color.FromArgb(255, 255, 200, 0);
        
        // Lauf-Animation - Beine bewegen sich
        var legCycle = (gameTime * 8) % 1.0;
        var legOffset = Math.Sin(legCycle * Math.PI * 2) * 0.4;
        
        // Cape im Hintergrund (weht im Wind)
        var capeWave = Math.Sin(gameTime * 5) * 0.6;
        if (needsToJump && jumpProgress > 0.1)
        {
            // Cape weht beim Springen nach hinten
            Ctx.RectXyWh(playerX - 1.2 + capeWave, playerY, 1.5, playerHeight + 1.0)
                .Fill.Solid(Color.FromArgb(220, 10, 10, 15));
        }
        else
        {
            // Cape hängt beim Laufen
            Ctx.RectXyWh(playerX + playerWidth - 1.0, playerY + 0.5, 1.2, playerHeight - 0.5)
                .Fill.Solid(Color.FromArgb(220, 10, 10, 15));
        }
        
        // Füße/Beine (auf der Plattform)
        if (!needsToJump || jumpProgress < 0.1)
        {
            // Linkes Bein
            Ctx.RectXyWh(playerX + 0.4 + legOffset, playerY + playerHeight - 1.5, 0.8, 1.5)
                .Fill.Solid(batmanGray);
            // Rechtes Bein  
            Ctx.RectXyWh(playerX + 1.6 - legOffset, playerY + playerHeight - 1.5, 0.8, 1.5)
                .Fill.Solid(batmanGray);
        }
        
        // Batman Körper (schwarzer Anzug) - muskulös
        Ctx.RectXyWh(playerX + 0.2, playerY + 1.0, playerWidth - 0.4, playerHeight - 1.0)
            .Fill.Solid(batmanBlack);
        
        // Gelber Gürtel mit Bat-Symbol
        Ctx.RectXyWh(playerX + 0.2, playerY + playerHeight - 2.2, playerWidth - 0.4, 0.6)
            .Fill.Solid(batmanYellow);
        // Gürtelschnalle (dunkler)
        Ctx.RectXyWh(playerX + playerWidth/2 - 0.3, playerY + playerHeight - 2.2, 0.6, 0.6)
            .Fill.Solid(Color.FromArgb(255, 180, 140, 0));
        
        // Schultern breiter
        Ctx.RectXyWh(playerX, playerY + 1.0, playerWidth, 1.2)
            .Fill.Solid(batmanBlack);
        
        // Batman Kopf/Maske (größer)
        Ctx.Circle(playerX + playerWidth / 2.0, playerY + 0.5, 1.2)
            .Fill.Solid(batmanBlack);
        
        // Batman Ohren (spitze Dreiecke) - markanter
        // Linkes Ohr
        Ctx.RectXyWh(playerX + 0.4, playerY - 1.2, 0.5, 1.2)
            .Fill.Solid(batmanBlack);
        // Rechtes Ohr
        Ctx.RectXyWh(playerX + 2.1, playerY - 1.2, 0.5, 1.2)
            .Fill.Solid(batmanBlack);
        
        // Augen (weiß/leuchtend) - größer und markanter
        Ctx.Circle(playerX + 0.9, playerY + 0.4, 0.35)
            .Fill.Solid(Colors.White);
        Ctx.Circle(playerX + 2.1, playerY + 0.4, 0.35)
            .Fill.Solid(Colors.White);
        
        // Bat-Symbol auf der Brust (gelb)
        Ctx.Circle(playerX + playerWidth / 2.0, playerY + 2.0, 0.8)
            .Fill.Solid(batmanYellow);
        // Bat-Flügel Form (vereinfacht)
        Ctx.RectXyWh(playerX + 0.8, playerY + 1.9, 0.4, 0.3)
            .Fill.Solid(batmanBlack);
        Ctx.RectXyWh(playerX + 1.8, playerY + 1.9, 0.4, 0.3)
            .Fill.Solid(batmanBlack);
        
        // Fortschrittsbalken am oberen Rand
        var progress = Math.Min(1.0, worldOffset / 60.0);
        Ctx.RectXyWh(0, 0, 24 * progress, 1).Fill.Solid(Colors.Gold);
    }
    else
    {
        // === FEUERWERK-PHASE ===
        var fireworkTime = cycleTime - levelDuration;
        
        // Dunkler Nachthimmel
        Ctx.RectXyWh(0, 0, 24, 24).Fill.Solid(Color.FromArgb(255, 10, 10, 40));
        
        // Text "ZIEL!"
        Ctx.RectXyWh(8, 10, 8, 5).Fill.Solid(Color.FromArgb(200, 0, 0, 0));
        
        // Kreatives Feuerwerk mit verschiedenen Effekten
        for (var i = 0; i < 20; i++)
        {
            var explosionTime = (i * 0.18) % 3.0;
            
            if (fireworkTime >= explosionTime && fireworkTime < explosionTime + 1.8)
            {
                var age = fireworkTime - explosionTime;
                var centerX = 5.0 + (i * 6.8) % 14.0;
                var centerY = 5.0 + (i * 4.3) % 8.0;
                
                // Verschiedene Feuerwerk-Typen
                var fireworkType = i % 3;
                var particleCount = fireworkType == 0 ? 20 : (fireworkType == 1 ? 12 : 16);
                
                for (var p = 0; p < particleCount; p++)
                {
                    var angle = (p / (double)particleCount) * Math.PI * 2;
                    
                    // Verschiedene Geschwindigkeiten je nach Typ
                    var baseSpeed = fireworkType == 0 ? 6.0 : (fireworkType == 1 ? 4.5 : 5.5);
                    var initialSpeed = baseSpeed + (p % 2) * 1.0;
                    var vx = Math.Cos(angle) * initialSpeed;
                    var vy = Math.Sin(angle) * initialSpeed;
                    
                    // Position mit realistischer Schwerkraft
                    var gravity = 10.0;
                    var x = centerX + vx * age;
                    var y = centerY + vy * age + 0.5 * gravity * age * age;
                    
                    // Fade out - früher für Typ 1
                    var fadeStart = fireworkType == 1 ? 0.6 : 0.9;
                    var alpha = age < fadeStart ? 255 : (int)(255 * Math.Max(0, 1.0 - (age - fadeStart) / 0.9));
                    var size = Math.Max(0.4, 1.2 - age * 0.5);
                    
                    if (alpha > 20 && x >= 0 && x < 24 && y >= 0 && y < 24)
                    {
                        // Bunte Farben mit Variation
                        var hue = (i * 83.7 + p * 15) % 360.0;
                        var saturation = fireworkType == 2 ? 0.7f : 1.0f;
                        var color = Color.FromHsl((float)hue, saturation, 0.6f);
                        var colorWithAlpha = Color.FromArgb((byte)alpha, color.Red, color.Green, color.Blue);
                        
                        Ctx.Circle(x, y, size).Fill.Solid(colorWithAlpha);
                        
                        // Glüh-Effekt um Partikel
                        if (alpha > 100 && fireworkType == 0)
                        {
                            var glowAlpha = alpha / 4;
                            var glowColor = Color.FromArgb((byte)glowAlpha, color.Red, color.Green, color.Blue);
                            Ctx.Circle(x, y, size * 1.5).Fill.Solid(glowColor);
                        }
                        
                        // Trail nur für bestimmte Partikel
                        if (p % 4 == 0 && age > 0.15 && fireworkType != 1)
                        {
                            var trailX = centerX + vx * (age - 0.15);
                            var trailY = centerY + vy * (age - 0.15) + 0.5 * gravity * (age - 0.15) * (age - 0.15);
                            var trailAlpha = alpha / 4;
                            if (trailAlpha > 10)
                            {
                                var trailColor = Color.FromArgb((byte)trailAlpha, color.Red, color.Green, color.Blue);
                                Ctx.Circle(trailX, trailY, size * 0.6).Fill.Solid(trailColor);
                            }
                        }
                    }
                }
            }
        }
        
        // Blinkende Sterne im Hintergrund
        for (var s = 0; s < 20; s++)
        {
            var starX = (s * 7.123) % 24.0;
            var starY = (s * 5.789) % 24.0;
            var brightness = (Math.Sin(fireworkTime * 3 + s) + 1.0) / 2.0;
            var starAlpha = (int)(brightness * 150);
            
            Ctx.Circle(starX, starY, 0.3).Fill.Solid(Color.FromArgb((byte)starAlpha, 255, 255, 255));
        }
    }
};

// await PXL.Simulate(mainScene);
await PXL.SendToDevice(mainScene, "192.168.178.110");
