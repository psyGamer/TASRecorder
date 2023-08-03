using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.TASRecorder.Util;

public class ValueButton : TextMenu.Button {
    public string Value;

    public ValueButton(string label, string value) : base(label) {
        Value = value;
    }

    public override void Render(Vector2 position, bool highlighted) {
        float alpha = Container.Alpha;
        var color = Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha);
        var strokeColor = Color.Black * (alpha * alpha * alpha);

        bool twoColumns = Container.InnerContent == TextMenu.InnerContentMode.TwoColumn && !AlwaysCenter;
        var leftPos = position + (twoColumns ? Vector2.Zero : new Vector2(Container.Width * 0.5f, 0f));
        var justify = twoColumns ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0.5f);
        ActiveFont.DrawOutline(Label, leftPos, justify, Vector2.One, color, 2f, strokeColor);

        const float Padding = 30.0f;
        float labelWidth = ActiveFont.Measure(Label).X;
        float valueWidth = ActiveFont.Measure(Value).X;
        float valueScale = Math.Min(1.0f, (Container.Width - labelWidth - Padding) / valueWidth);

        ActiveFont.DrawOutline(Value, leftPos + new Vector2(Container.Width - valueWidth * valueScale, 0.0f), justify, Vector2.One * valueScale, color, 2f, strokeColor);
    }
}
