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
- new apps, you can place in the folder `apps/ai-generated` (create it when not present)


PXL Hints
===

When you need to do per-pixel-stuff, use the appropriate abstraction (here: The Pixels Array of the Ctx) and do direct array element assignment (see in the pxl-api repo how this is done)


CRITICAL: API Usage
===

**ALWAYS check the pxl-api repository before writing code!**

- Use the `github_repo` tool to search SchlenkR/pxl-api for correct API usage
- Look at existing examples in this workspace (apps/ folder) for patterns
- NEVER guess or assume API methods - always verify first
- The API uses specific patterns that must be followed exactly

**Common mistakes to avoid:**
- Using `DateTime.Now` instead of `Ctx.Now`
- Using `new Color()` instead of `Color.FromArgb()`
- Calling methods on `Ctx` that don't exist (like `Ctx.Save()` - must use `Ctx.RenderCtx.SkiaCanvas.Save()`)

**When in doubt:** Search the pxl-api repo or look at working examples in the workspace first!

Workflow for Copilot
===

**ALWAYS check for errors after making changes:**
- Use `get_errors` tool to check for compile/lint errors in files
- Fix any errors before considering the task complete
- Verify the code compiles and runs correctly
