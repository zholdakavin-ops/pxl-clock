Basics
===

This workspace is for developing small PXL apps (for PXL Clock).

It's a C# API.

It uses Single-File-Based C# "scripts" (new in .Net 10)

You do not need to execute anything: There's usually a watcher running; when the user saves a file, it is automatically recompiled and executed.

To understand what you can do, check the API on GitHub: SchlenkR/pxl-api: https://github.com/SchlenkR/pxl-api
It uses Skia.Net under the hood, but that's not the focus here.

You can also find some examples here in this workspace.


Coding Hints:
===

- Always use "var" when possible
- write small comments that explain every block or line (for non-experienced developers)
- always use one exact file to write code
- when creating new apps, you always use the header as you find e.g. in the file `apps/simple_demo`
- new apps, you can place in the folder `apps/gemini` (create it when not present)


PXL Hints
===

When you need to do per-pixel-stuff, use the appropriate abstraction (here: The Pixels Array of the Ctx) and do direct array element assignment (see in the pxl-api repo how this is done)

