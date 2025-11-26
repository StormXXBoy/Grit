using System.Drawing;
using System.Windows.Forms.PropertyGridInternal;

public class Platform
{
    public RectangleF bounds;
    public Brush color = Brushes.Firebrick;
    public Bitmap texture = Platformer.Properties.Resources.brick;

    public Platform(float x, float y, float width, float height)
    {
        bounds = new RectangleF(x, y, width, height);
    }

    public void Draw(Graphics g)
    {
        //g.FillRectangle(color, bounds);
        g.DrawImage(texture, bounds);
    }
}