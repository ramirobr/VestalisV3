using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Cotecna.Vestalis.Web.Common
{
    /// <summary>
    /// Class used to modify the appearance of the image in our Captcha test. 
    /// Use this class in the ImageBuilder of the XCaptcha constructor.
    /// </summary>
    public class CotecnaCanvas : XCaptcha.Canvas
    {
        internal static Color CotecnaBlue = Color.FromArgb(97, 188, 237);

        public CotecnaCanvas()
            : base(150 /* width*/, 50 /*height*/, new HatchBrush(HatchStyle.Trellis, CotecnaBlue, CotecnaBlue) /* brush */)
        {

        }
    }

    /// <summary>
    /// Class used to provide the text style in the ImageBuilder of the XCaptcha constructor
    /// </summary>
    public class CotecnaTextStyle : XCaptcha.TextStyle
    {
        public CotecnaTextStyle()
            : base(new Font("Consolas", 24, FontStyle.Regular) /*font*/,
            new HatchBrush(HatchStyle.Cross, Color.DarkBlue, Color.DarkBlue) /*brush*/)
        {

        }
    }

    /// <summary>
    /// Class used to provide noise in the ImageBuilder of the XCaptcha constructor
    /// </summary>
    public class CotecnaProvidedNoise : XCaptcha.Noise
    {

        public CotecnaProvidedNoise()
            : base(new HatchBrush(HatchStyle.Shingle, Color.DarkBlue, Color.DarkBlue))
        {

        }


        public override void Create(Graphics graphics, XCaptcha.ICanvas canvas)
        {
            var random = new Random();


            for (var i = 0; i < (canvas.Width * canvas.Height / 10); i++)
            {
                var x = random.Next(canvas.Width);
                var y = random.Next(canvas.Height);
                var w = random.Next(3);
                var h = random.Next(3);

                graphics.FillEllipse(Brush, x, y, w, h);
            }

        }

    }

    /// <summary>
    /// Class used to implement distort in the ImageBuilder of the XCaptcha constructor
    /// </summary>
    public class CotecnaDistort : XCaptcha.Distort
    {

        public override GraphicsPath Create(GraphicsPath path, XCaptcha.ICanvas canvas)
        {


            var random = new Random();
            var rect = new Rectangle(0, 0, canvas.Width, canvas.Height);

            const float wrapFactor = 8F;

            PointF[] points =
                {
                    new PointF(random.Next(rect.Width) / wrapFactor,random.Next(rect.Height) / wrapFactor),
                    new PointF(rect.Width - random.Next(rect.Width) / wrapFactor, random.Next(rect.Height) / wrapFactor),
                    new PointF(random.Next(rect.Width) / wrapFactor, rect.Height - random.Next(rect.Height) / wrapFactor),
                    new PointF(rect.Width - random.Next(rect.Width) / wrapFactor, rect.Height - random.Next(rect.Height) / wrapFactor)
                };

            var matrix = new Matrix();
            matrix.Translate(0F, 0F);

            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

            return path;
        }
    }
}