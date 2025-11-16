# PXL Clock

Welcome to the **PXL Clock** repository! This repo serves as a central hub for:

- **Resources for creating custom PXL Clock applications**
- **Issue tracking** and **idea proposals** (hardware, software, use cases, features)

We’re excited to see what the community will build around the PXL Clock. Below you’ll find everything you need to get started.

<p align="center">
  <a href="https://pxlclock.com">
    <img width="842" height="832" alt="image" src="https://github.com/user-attachments/assets/9b92c9d7-b20b-4316-8104-ac980fa449d5" />
  </a>
  <!--<img width="640" alt="image" src="https://github.com/user-attachments/assets/4c898f7e-56ae-4a8b-be34-464ad83a5ffb" />-->
</p>

---

## Quick-Start Development of PXL Clock Apps

Here is the most easy and convenient way to get started with developing PXL Clock apps:

- Create or open a [**Codespace**](https://github.com/CuminAndPotato/PXL-Clock)
- (!) Wait for the Codespace to initialize (this may take a few minutes)
- Choose your language: **F#** or **C#**
  - For **F#**: Run `./start-fsharp.sh` in the terminal
  - For **C#**: Run `./start-csharp.sh` in the terminal
- Open the simulator in your browser at `http://localhost:5001`
- **F# Users**: Open and save any `.fsx` file (e.g. `apps_fsharp/05_UrsEnzler/ursEnzler_ColourWheelDynamic.fsx`)
  - (!) Save the file even if you don't change anything, to trigger the simulator to run it!
- **C# Users**: Edit `apps_csharp/Program.cs` and save to see changes automatically

For anything else, have a look at the resources below :) Enjoy!

---

## Order Your PXL Clock!

Exciting news: ordering the PXL Clock will soon be possible! 🎉 You can find more information and updates on our official website: [pxlclock.com](https://pxlclock.com)

We’re currently working on the first 100 units, the MK1 edition! We’re in the certification and refining all the little details that make this a fine product. We’re fully committed to delivering something amazing, and we’ll keep you updated every step of the way.

Stay in the loop by following the #pxlclock hashtag on our channels for the latest news and progress updates.

Thank you for your patience and support! 💡

---

## Get In Touch

### Discord

Get in touch with us and others on our [**Discord Server**](https://discord.gg/KDbVdKQh5j)

<p align="center">
  <h3>Join the PXL Clock Community on Discord</h3>
  <a href="https://discord.gg/KDbVdKQh5j">
    <img src="https://img.shields.io/badge/Discord-Join%20Server-blue?style=flat-square&logo=discord" alt="Join Our Discord">
  </a>
</p>

### Bluesky

Follow the [**#pxlclock hashtag**](https://bsky.app/hashtag/PXLclock) on **Bluesky** for getting news and see what others do!

---

## Table of Contents

1. [Order Your PXL Clock](#order-your-pxl-clock)
2. [Get In Touch](#get-in-touch)
3. [About PXL Clock](#about-pxl-clock)
4. [Releases](#releases)
5. [Filing Issues and Ideas](#filing-issues-and-ideas)
6. [Developing Your Own Apps](#developing-your-own-apps)
7. [Contributing](#contributing)
8. [License](LICENSE.md)

---

## About PXL Clock

The **PXL Clock** is a device designed to display various fun clocks, animations, short stories, visuals and other creative things - all on a 24x24 pixel display. Whether you want to keep track of the current time in a futuristic manner or develop your own mini-apps to run on the clock, this project provides a flexible platform for creativity.

---

## Releases

You’ll find our official firmware and software packages under the [**Releases**](../../releases) section. The PXL Clock updates itself over-the-air, so no manual steps required.

---

## Filing Issues and Ideas

Have an idea for a new feature or discovered a bug? Help us improve the PXL Clock by creating a new issue in this repository. We welcome:

- Hardware-related feedback or design modifications
- Software feature requests, improvements, or bug reports
- Use case suggestions or creative ways to integrate PXL Clock into your projects

Just head over to the [**Issues**](../../issues) tab and click **New Issue** to get started.

---

## Developing Your Own Apps

[![NuGet](https://img.shields.io/nuget/v/Pxl.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Pxl)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Pxl.svg?style=flat-square)](https://www.nuget.org/packages/Pxl)

Whether you're a seasoned developer or new to programming, we hope these resources will jumpstart your creativity.

You can use this repository as a reference point for developing your own custom PXL Clock applications in **F#** or **C#**. We provide examples, documentation, and tools to help you get started.

### Choose Your Language: F# or C#

PXL Clock supports development in both **F#** and **C#**, giving you the flexibility to use whichever language you prefer:

#### **F# Development**
- **Interactive scripting**: Write `.fsx` files that run immediately when saved
- **Great for**: Rapid prototyping, experimenting with ideas, creative coding
- **Examples**: Check out `apps_fsharp/` directory for numerous examples
- **API**: Functional-first API with composable functions

#### **C# Development**
- **Familiar syntax**: Use standard C# with top-level statements
- **Great for**: Object-oriented developers, integration with existing C# codebases
- **Examples**: See `apps_csharp/Program.cs` for comprehensive examples
- **API**: Fluent API with method chaining (e.g., `Ctx.Circle(12, 12, 10).Fill.Solid(Colors.Yellow)`)

Both languages use the same underlying rendering engine and share similar capabilities for creating graphics, animations, and interactive clock displays.

### Getting Started

To program PXL-Apps, you can either **start a Codespace (recommended)** or **set up locally with VSCode**.

### Option 1: Start a Codespace (Recommended)

- Click the **Code** button on GitHub and select **Open with Codespaces**.
- This will launch a ready-to-use development environment in your browser.
- All prerequisites are pre-installed; you can start coding and testing immediately.
   
To keep your simulator and watcher running continuously in Codespaces, use our helper script:

**For F#:**
```bash
./start-fsharp.sh
```

**For C#:**
```bash
./start-csharp.sh
```

### Option 2: Set Up Locally with VSCode

#### Prerequisites

- [**.NET 8 SDK**](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [**Visual Studio Code (VSCode)**](https://code.visualstudio.com/)
- **For F#**: [**Ionide-fsharp Extension for VSCode**](https://marketplace.visualstudio.com/items?itemName=Ionide.Ionide-fsharp)
- **For C#**: [**C# Dev Kit Extension for VSCode**](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) (optional but recommended)

#### Fork the Repository

Best practice is to fork this repository to your GitHub account. This way, you can experiment with the code and save your changes, and maybe there will be some surprises along the way. 🎁

#### Create Your First App

**F# Apps:**
- Create a new `.fsx` file in the `./apps_fsharp` directory
- Write your scene using the F# API
- Save the file to run it in the simulator
- You can use the existing apps as a starting point

**C# Apps:**
- Edit `./apps_csharp/Program.cs`
- Define scenes as lambda functions:
  ```csharp
  var myScene = () =>
  {
      Ctx.Circle(12, 12, 10).Fill.Solid(Colors.Yellow);
      Ctx.Text().Mono6x6("Hi!", 0, 18).Fill.Solid(Colors.White);
  };
  ```
- Set your scene: `var currentScene = myScene;`
- Save to see changes in the simulator

### 🚀 Start the Simulator

**Using VSCode Tasks (Recommended):**

- Open the VSCode build task (press `Ctrl+Shift+B` or `Cmd+Shift+B`)
- Select:
  - `PXL :: Start F# (Simulator + F# Watcher)` for F# development
  - `PXL :: Start C# (Simulator + C# Watcher)` for C# development
- Open the simulator in your browser at `http://localhost:5001`

**Using Shell Scripts:**

**For F#:**
```bash
./start-fsharp.sh      # macOS/Linux
./start-fsharp.ps1     # Windows PowerShell
```

**For C#:**
```bash
./start-csharp.sh      # macOS/Linux
./start-csharp.ps1     # Windows PowerShell
```

After starting:
- **F# Users**: Save any `.fsx` file in `./apps_fsharp` to run it
- **C# Users**: Save `Program.cs` to rebuild and restart automatically

### C# API Quick Reference

The C# API uses a fluent, chainable syntax:

```csharp
using Pxl.Ui.CSharp;
using SkiaSharp;
using static Pxl.Ui.CSharp.Drawing;

// Drawing shapes
Ctx.Circle(12, 12, 10).Fill.Solid(Colors.Yellow);
Ctx.RectXyWh(2, 2, 20, 20).Stroke.Solid(Colors.Red, strokeWidth: 2);
Ctx.Line(0, 0, 24, 24).Stroke.Solid(Colors.Cyan);

// Gradients
Ctx.Circle(12, 12, 10).Fill.RadialGradient(
    new SKPoint(12, 12), 12,
    [Colors.Yellow, Colors.Orange, Colors.Red]
);

// Text
Ctx.Text().Mono6x6("Hello", 0, 0).Fill.Solid(Colors.White);
Ctx.Text().Var3x5("PXL", 0, 7).Fill.Solid(Colors.Cyan);
```

See `apps_csharp/Program.cs` for comprehensive examples including animations, gradients, and more.

### Submit Your App

When you’re ready to submit your app, create a pull request (PR) with your changes. We’ll review your app, provide feedback or merge it.

Follow-up PRs (updates) for your app in case you want to improve it are welcome!

### The PizzaMampf Sprite

Check out the sprite 🖼️ `./apps_fsharp/03_ Demos/assets/pizzaMampf.png`) and swap them with your own custom artwork to personalize your app.

### Deploying an App or an Image to the PXL Clock

Here are 2 ways of deploying an app or an image to the PXL Clock. Keep in mind that

- the PXL Clock needs to be connected to the same network as your computer.
- the artifacts you deploy are not persistent (for now) and will be lost after a reboot.

There are two ways to deploy an app or an image to the PXL Clock:

**Using the VSCode Build Tasks**

Open the list of build tasks in VSCode:

- Press `Ctrl+Shift+B` (Windows/Linux) or `Cmd+Shift+B` (macOS).
- Select **Deploy App** or **Deploy Image** from the list.

**Using the Scripts Directly**

In your terminal, run the following scripts:

- `./deploy-app.sh` (Mac) or `./deploy-app.ps1` (Windows) to deploy an app.
- `./deploy-image.sh` (Mac) or `./deploy-image.ps1` (Windows) to deploy an image.

---

## Contributing

Contributions from the community are highly encouraged. If you want to help make PXL Clock better, you can:

1. **Create an Issue:** File a new issue for suggestions, bug reports, or feature requests.
2. **Submit a Pull Request:** Fork this repo, make your changes, and submit a pull request. Make sure to include a clear description of what you’ve changed or fixed.

Before contributing, please review our [**Code of Conduct**](CODE_OF_CONDUCT.md) (if available) to ensure a positive experience for everyone.

---

see: [LICENSE.md](LICENSE.md)

---

Thank you for your interest in the PXL Clock! We look forward to seeing your ideas and contributions. If you have any questions or suggestions, feel free to open an issue or start a discussion. Let’s make time more fun—together!
