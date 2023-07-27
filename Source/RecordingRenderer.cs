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

    private const float PaddingVerySmall = 16.0f;
    private const float PaddingSmall = 32.0f;
    private const float PaddingLarge = 40.0f;

    private const float WaveLength = 2.0f;
    private const float FadeInTime = 0.5f;
    private const string RecordingText = "REC";
    private const int YPos = 60; // Same as speedrun timer

    private static float circleSine = 0.0f;
    private static float bannerFadeIn = 0.0f;
    private static int recordedFrames = 0;

    private static float numberWidth = -1.0f;
    private static float spacerWidth = -1.0f;
    private static float oParenWidth = -1.0f;
    private static float cParenWidth = -1.0f;

    // Use the same (inaccurate) calculation as the speedrun timer, to stay in sync with it.
    private static long RecordedTicks => recordedFrames * TimeSpan.FromSeconds(Engine.RawDeltaTime).Ticks;
    private static string RecordedTimeString => TimeSpan.FromTicks(RecordedTicks).ShortGameplayFormat();
    private static float RecordingIndicatorWidth => ActiveFont.Measure(RecordingText).X + PaddingSmall + Circle.Width;
    private static float RecordingTimeWidth {
        get {
            string timeString = RecordedTimeString;
            float scale = 1.0f;
            float width = 0.0f;
            for (int i = 0; i < timeString.Length; i++)
            {
                char c = timeString[i];
                if (c == '.') scale = 0.7f;

                float charWidth = (((c == ':' || c == '.') ? spacerWidth : numberWidth) + 4f) * scale;
                width += charWidth;
            }

            return width;
        }
    }
    private static float RecordingFramesWidth => oParenWidth + cParenWidth + numberWidth * recordedFrames.ToString().Length;

    public static void Start() {
        recordedFrames = 0;

        // Calculate numberWidth/spacerWidth. Taken from SpeedrunTimerDisplay.CalculateBaseSizes()
        var font = Dialog.Languages["english"].Font;
        float fontFaceSize = Dialog.Languages["english"].FontFaceSize;
        var pixelFontSize = font.Get(fontFaceSize);

        for (int i = 0; i < 10; i++) {
            float x = pixelFontSize.Measure(i.ToString()).X;
            if (x > numberWidth) numberWidth = x;
        }
        spacerWidth = pixelFontSize.Measure('.').X;
        oParenWidth = pixelFontSize.Measure('(').X;
        cParenWidth = pixelFontSize.Measure(')').X;
    }

    public static bool ShouldUpdate() => bannerFadeIn > 0.0f;

    public static void Update() {
        circleSine += Engine.RawDeltaTime / WaveLength;
        circleSine %= MathF.Tau;

        if (bannerFadeIn < 1.0f && TASRecorderModule.Recording)
            bannerFadeIn += Engine.RawDeltaTime / FadeInTime;
        else if (bannerFadeIn > 0.0f && !TASRecorderModule.Recording)
            bannerFadeIn -= Engine.RawDeltaTime / FadeInTime;
        bannerFadeIn = Math.Clamp(bannerFadeIn, 0.0f, 1.0f);

        if (!TASRecorderModule.Recording) return;

        recordedFrames++;
    }

    public static void Render() {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);

        if (!TASRecorderModule.Settings.RecordingIndicator && !TASRecorderModule.Settings.RecordingTime) return;

        float bannerWidth = PaddingSmall * 2.0f; // Include start/end padding
        if (TASRecorderModule.Settings.RecordingIndicator)
            bannerWidth += PaddingLarge + RecordingIndicatorWidth;
        if (TASRecorderModule.Settings.RecordingTime)
            bannerWidth += PaddingLarge + RecordingTimeWidth + RecordingFramesWidth;
        bannerWidth -= PaddingLarge; // Trim the last padding

        float totalOffset = Lerp(bannerWidth, 0.0f, bannerFadeIn);

        BG.DrawJustified(position: new Vector2(Celeste.TargetWidth - bannerWidth + totalOffset, YPos),
                         justify: new Vector2(1.0f, 0.0f),
                         color: Color.White,
                         scale: new Vector2(-1.0f, 1.0f));

        float offset = -PaddingSmall + totalOffset;
        if (TASRecorderModule.Settings.RecordingTime) {
            DrawFrameCount(offset, recordedFrames);
            offset -= PaddingVerySmall + RecordingFramesWidth;
            DrawRecordingTime(offset);
            offset -= PaddingLarge + RecordingTimeWidth;
        }
        if (TASRecorderModule.Settings.RecordingIndicator) {
            DrawRecordingIndicator(offset);
        }

        Draw.SpriteBatch.End();
    }

    private static void DrawRecordingIndicator(float offset) {
        var textSize = ActiveFont.Measure(RecordingText);
        var circleSize = new Vector2(Circle.Width, Circle.Height);
        var color = Color.Lerp(CircleColorA, CircleColorB, (MathF.Sin(circleSine) + 1.0f) / 2.0f);

        ActiveFont.DrawOutline(RecordingText, position: new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f),
                                     justify:  new Vector2(1, 1),
                                     scale: Vector2.One,
                                     color: Color.White,
                                     stroke: 2.0f,
                                     strokeColor: Color.Black);
        offset -= PaddingSmall + textSize.X;
        Circle.DrawStrokeJustified(position: new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f - (textSize.Y - circleSize.Y) / 2.0f),
                                   justify: new Vector2(0.75f, 1.0f),
                                   color,
                                   scale: 1.0f,
                                   stroke: 2.0f);
    }

    private static void DrawRecordingTime(float offset) {
        SpeedrunTimerDisplay.DrawTime(position: new Vector2(Celeste.TargetWidth + offset - RecordingTimeWidth, YPos + 44.0f), RecordedTimeString);
    }

    private static void DrawFrameCount(float offset, int frames) {
        const float Scale = 0.75f;
        const float YOffset = Scale * 5.0f;

        var font = Dialog.Languages["english"].Font;
        float fontFaceSize = Dialog.Languages["english"].FontFaceSize;

        offset += oParenWidth / 2.0f * Scale;
        font.DrawOutline(fontFaceSize, "(", new Vector2(Celeste.TargetWidth + offset - RecordingFramesWidth, YPos + 44.0f - YOffset), new Vector2(0.5f, 1f), Vector2.One * Scale, Calc.HexToColor("7a6f6d"), 2f, Color.Black);
        offset += oParenWidth / 2.0f * Scale;

        string numberString = frames.ToString();
        foreach (char c in numberString) {
            offset += numberWidth / 2.0f * Scale;
            font.DrawOutline(fontFaceSize, c.ToString(), new Vector2(Celeste.TargetWidth + offset - RecordingFramesWidth, YPos + 44.0f - YOffset), new Vector2(0.5f, 1f), Vector2.One * Scale, Calc.HexToColor("918988"), 2f, Color.Black);
            offset += numberWidth / 2.0f * Scale;
        }

        offset += cParenWidth / 2.0f * Scale;
        font.DrawOutline(fontFaceSize, ")", new Vector2(Celeste.TargetWidth + offset - RecordingFramesWidth, YPos + 44.0f - YOffset), new Vector2(0.5f, 1f), Vector2.One * Scale, Calc.HexToColor("7a6f6d"), 2f, Color.Black);
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
