using Pxl.Ui.CSharp;
using static Pxl.Ui.CSharp.Drawing;

static class PixelPlay_1
{
    public static Action Scene = () =>
    {
        Ctx.Background().Solid(Color.Blue);

        var pixels = Ctx.Pixels();

        var random = new Random();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.FromRgb(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256));
        }

        Ctx.Text().Mono4x5("HELLO", 0, 10, Color.Black);

        for (int i = 0; i < pixels.Length; i++)
        {
            var color = pixels[i];
            if (color.Red == 0 && color.Green == 0 && color.Blue == 0)
            {
                pixels[i] = Color.Blue;
            }
        }
    };
}
