using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.TASRecorder;

internal static class RecordingRenderer {
    private static readonly MTexture BG = GFX.Gui["TASRecorder/extendedStrawberryCountBG"];
    private static readonly MTexture Circle = GFX.Gui["TASRecorder/recordingCircle"];
    private static readonly Color CircleColorA = Color.Red;
    private static readonly Color CircleColorB = new(255, 40, 40);

    private const float WaveLength = 2.0f;
    private const float FadeInTime = 0.5f;
    private const float PaddingSmall = 32.0f;
    private const string RecordingText = "REC";
    private const int YPos = 60; // Same as speedrun timer

    private static float circleSine = 0.0f;
    private static float bannerFadeIn = 0.0f;

    public static void Update() {
        circleSine += Engine.RawDeltaTime / WaveLength;
        circleSine %= MathF.Tau;

        if (bannerFadeIn < 1.0f && TASRecorderModule.Recording)
            bannerFadeIn += Engine.RawDeltaTime / FadeInTime;
        else if (bannerFadeIn > 0.0f && !TASRecorderModule.Recording)
            bannerFadeIn -= Engine.RawDeltaTime / FadeInTime;
        bannerFadeIn = Math.Clamp(bannerFadeIn, 0.0f, 1.0f);
    }

    public static void Render() {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);

        float bannerWidth = 0.0f;
        if (TASRecorderModule.Settings.RecordingIndicator)
            bannerWidth += PaddingSmall * 3.0f + ActiveFont.Measure(RecordingText).X + Circle.Width;

        float totalOffset = Lerp(bannerWidth, 0.0f, bannerFadeIn);

        BG.DrawJustified(position: new Vector2(Celeste.TargetWidth - bannerWidth + totalOffset, YPos),
                         justify: new Vector2(1.0f, 0.0f),
                         color: Color.White,
                         scale: new Vector2(-1.0f, 1.0f));

        if (TASRecorderModule.Settings.RecordingIndicator)
            DrawRecordingGizmos(offset: 0.0f + totalOffset);

        Draw.SpriteBatch.End();
    }

    private static void DrawRecordingGizmos(float offset) {
        var textSize = ActiveFont.Measure(RecordingText);
        var circleSize = new Vector2(Circle.Width, Circle.Height);
        var color = Color.Lerp(CircleColorA, CircleColorB, (MathF.Sin(circleSine) + 1.0f) / 2.0f);

        Circle.DrawStrokeJustified(position: new Vector2(Celeste.TargetWidth - PaddingSmall * 2 - textSize.X + offset, YPos + 44.0f - (textSize.Y - circleSize.Y) / 2.0f),
                                   justify: new Vector2(0.75f, 1.0f),
                                   color,
                                   scale: 1.0f,
                                   stroke: 2.0f);
        ActiveFont.DrawOutline(RecordingText, position: new Vector2(Celeste.TargetWidth - PaddingSmall + offset, YPos + 44.0f),
                                     justify:  new Vector2(1, 1),
                                     scale: Vector2.One,
                                     color: Color.White,
                                     stroke: 2.0f,
                                     strokeColor: Color.Black);

    }

    private static float Lerp(float a, float b, float t) {
        return a * (1 - t) + b * t;
    }

    private static void DrawStrokeJustified(this MTexture texture, Vector2 position, Vector2 justify, Color color, float scale, float stroke) {
        float scaleFix = texture.ScaleFix;
        scale *= scaleFix;
        var clipRect = texture.ClipRect;
        var origin = (new Vector2(texture.Width * justify.X, texture.Height * justify.Y) - texture.DrawOffset) / scaleFix;

        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (i != 0 || j != 0)
                Draw.SpriteBatch.Draw(texture.Texture.Texture_Safe, position + new Vector2(i * stroke, j * stroke), clipRect, Color.Black, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
        Draw.SpriteBatch.Draw(texture.Texture.Texture_Safe, position, clipRect, color, 0f, origin, scale, SpriteEffects.None, 0f);
    }
}
