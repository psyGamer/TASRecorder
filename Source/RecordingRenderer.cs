using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Celeste.Mod.TASRecorder;

internal static class RecordingRenderer {
    private static readonly MTexture BG = GFX.Gui["TASRecorder/extendedStrawberryCountBG"];
    private static readonly MTexture Circle = GFX.Gui["TASRecorder/recordingCircle"];
    private static readonly Color CircleColorA = Color.Red;
    private static readonly Color CircleColorB = new(255, 40, 40);

    private static PixelFont Font => Dialog.Languages["english"].Font;
    private static float FontFaceSize => Dialog.Languages["english"].FontFaceSize;

    private const float PaddingVerySmall = 16.0f;
    private const float PaddingSmall = 32.0f;
    private const float PaddingLarge = 40.0f;

    private const float WaveLength = 2.0f;
    private const float FadeInTime = 0.5f;
    private const int YPos = 60; // Same as speedrun timer
    const float FramesScale = 0.75f;

    private const string RecordingText = "REC";

    private static float circleSine = 0.0f;
    private static float bannerFadeIn = 0.0f;

    private static float numberWidth = -1.0f;
    private static float spacerWidth = -1.0f;
    private static float oParenWidth = -1.0f;
    private static float cParenWidth = -1.0f;
    private static float oSquareWidth = -1.0f;
    private static float cSquareWidth = -1.0f;
    private static float percentWidth = -1.0f;

    private static float RecordingIndicatorWidth => ActiveFont.Measure(RecordingText).X + PaddingSmall + Circle.Width;
    // Use the same (inaccurate) calculation as the speedrun timer, to stay in sync with it.
    private static long RecordedTicks => VideoCapture.CurrentFrameCount * TimeSpan.FromSeconds(Engine.RawDeltaTime).Ticks;
    private static long TargetTicks => VideoCapture.TargetFrameCount * TimeSpan.FromSeconds(Engine.RawDeltaTime).Ticks;
    private static string RecordedTimeString => TimeSpan.FromTicks(RecordedTicks).ShortGameplayFormat();
    private static string TargetTimeString => TimeSpan.FromTicks(TargetTicks).ShortGameplayFormat();

    public static void Start() {
        // Calculate numberWidth/spacerWidth. Taken from SpeedrunTimerDisplay.CalculateBaseSizes()
        var pixelFontSize = Font.Get(FontFaceSize);

        for (int i = 0; i < 10; i++) {
            float x = pixelFontSize.Measure(i.ToString()).X;
            if (x > numberWidth) numberWidth = x;
        }
        spacerWidth = pixelFontSize.Measure('.').X;
        oParenWidth = pixelFontSize.Measure('(').X;
        cParenWidth = pixelFontSize.Measure(')').X;
        oSquareWidth = pixelFontSize.Measure('[').X;
        cSquareWidth = pixelFontSize.Measure(']').X;
        percentWidth = pixelFontSize.Measure('%').X;

        // This only gets called when the speedrun timer is constructed
        // However a recording could happen before that
        SpeedrunTimerDisplay.CalculateBaseSizes();
    }

    public static bool ShouldUpdate => bannerFadeIn > 0.0f;

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

        if (!TASRecorderModule.Settings.RecordingIndicator && TASRecorderModule.Settings.RecordingTime == RecordingTimeIndicator.NoTime) return;

        float bannerWidth = PaddingSmall * 2.0f; // Include start/end padding
        if (TASRecorderModule.Settings.RecordingIndicator)
            bannerWidth += PaddingLarge + RecordingIndicatorWidth;
        if (TASRecorderModule.Settings.RecordingTime != RecordingTimeIndicator.NoTime) {
            bannerWidth += PaddingLarge + GetTimeWidth(RecordedTimeString);
            if (TASRecorderModule.Settings.RecordingTime == RecordingTimeIndicator.RegularFrames) {
                bannerWidth += PaddingVerySmall + GetFramesWidth(VideoCapture.CurrentFrameCount);
            }

            if (VideoCapture.TargetFrameCount >= 0) {
                bannerWidth += PaddingLarge * 2 + GetTimeWidth(TargetTimeString);
                if (TASRecorderModule.Settings.RecordingTime == RecordingTimeIndicator.RegularFrames) {
                    bannerWidth += PaddingVerySmall + GetFramesWidth(VideoCapture.TargetFrameCount);
                }
                if (TASRecorderModule.Settings.RecordingProgress) {
                    bannerWidth += PaddingSmall + GetProgressWidth();
                }
            }
        }
        bannerWidth -= PaddingLarge; // Trim the last padding

        float fadeinOffset = Lerp(bannerWidth, 0.0f, bannerFadeIn);

        BG.DrawJustified(position: new Vector2(Celeste.TargetWidth - bannerWidth + fadeinOffset, YPos),
                         justify: new Vector2(1.0f, 0.0f),
                         color: Color.White,
                         scale: new Vector2(-1.0f, 1.0f));

        float offset = -PaddingSmall + fadeinOffset;
        if (TASRecorderModule.Settings.RecordingTime != RecordingTimeIndicator.NoTime) {
            if (VideoCapture.TargetFrameCount >= 0) {
                if (TASRecorderModule.Settings.RecordingProgress) {
                    DrawProgress(offset);
                    offset -= PaddingSmall + GetProgressWidth();
                }
                if (TASRecorderModule.Settings.RecordingTime == RecordingTimeIndicator.RegularFrames) {
                    DrawFrameCount(offset, VideoCapture.TargetFrameCount);
                    offset -= PaddingVerySmall + GetFramesWidth(VideoCapture.TargetFrameCount);
                }
                DrawRecordingTime(offset, TargetTimeString);
                offset -= PaddingLarge + GetTimeWidth(TargetTimeString);
                DrawSeparator(offset);
                offset -= PaddingLarge;
            }
            if (TASRecorderModule.Settings.RecordingTime == RecordingTimeIndicator.RegularFrames) {
                DrawFrameCount(offset, VideoCapture.CurrentFrameCount);
                offset -= PaddingVerySmall + GetFramesWidth(VideoCapture.CurrentFrameCount);
            }
            DrawRecordingTime(offset, RecordedTimeString);
            offset -= PaddingLarge + GetTimeWidth(RecordedTimeString);
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
                                     justify: new Vector2(1, 1),
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

    private static void DrawRecordingTime(float offset, string timeString) {
        SpeedrunTimerDisplay.DrawTime(position: new Vector2(Celeste.TargetWidth + offset - GetTimeWidth(timeString), YPos + 44.0f), timeString);
    }

    private static void DrawFrameCount(float offset, int frames) {
        const float YOffset = FramesScale * 5.0f;

        offset -= GetFramesWidth(frames);
        offset += oParenWidth / 2.0f * FramesScale;
        Font.DrawOutline(FontFaceSize, "(", new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f - YOffset), new Vector2(0.5f, 1f), Vector2.One * FramesScale, Calc.HexToColor("7a6f6d"), 2f, Color.Black);
        offset += oParenWidth / 2.0f * FramesScale;

        string numberString = frames.ToString();
        foreach (char c in numberString) {
            offset += numberWidth / 2.0f * FramesScale;
            Font.DrawOutline(FontFaceSize, c.ToString(), new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f - YOffset), new Vector2(0.5f, 1f), Vector2.One * FramesScale, Calc.HexToColor("918988"), 2f, Color.Black);
            offset += numberWidth / 2.0f * FramesScale;
        }

        offset += cParenWidth / 2.0f * FramesScale;
        Font.DrawOutline(FontFaceSize, ")", new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f - YOffset), new Vector2(0.5f, 1f), Vector2.One * FramesScale, Calc.HexToColor("7a6f6d"), 2f, Color.Black);
    }

    private static void DrawProgress(float offset) {
        string progressText = (VideoCapture.CurrentFrameCount / (float) VideoCapture.TargetFrameCount * 100.0f).ToString("0.00");

        offset -= GetProgressWidth();
        offset += oSquareWidth / 2.0f;
        Font.DrawOutline(FontFaceSize, "[", new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f), new Vector2(0.5f, 1f), Vector2.One, Calc.HexToColor("#7a6f6d"), 2f, Color.Black);
        offset += oSquareWidth / 2.0f;

        SpeedrunTimerDisplay.DrawTime(position: new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f), progressText);
        offset += GetTimeWidth(progressText);

        offset += percentWidth / 2.0f;
        Font.DrawOutline(FontFaceSize, "%", new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f), new Vector2(0.5f, 1f), Vector2.One, Calc.HexToColor("7a6f6d"), 2f, Color.Black);
        offset += percentWidth / 2.0f;

        offset += cSquareWidth / 2.0f;
        Font.DrawOutline(FontFaceSize, "]", new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f), new Vector2(0.5f, 1f), Vector2.One, Calc.HexToColor("7a6f6d"), 2f, Color.Black);
    }

    private static void DrawSeparator(float offset) {
        Font.DrawOutline(FontFaceSize, "/", new Vector2(Celeste.TargetWidth + offset, YPos + 44.0f), new Vector2(0.5f, 1f), Vector2.One, Calc.HexToColor("7a6f6d"), 2f, Color.Black);
    }

    private static float GetTimeWidth(string timeString) {
        float scale = 1.0f;
        float width = 0.0f;

        foreach (char c in timeString) {
            if (c == '.') scale = 0.7f;

            float charWidth = (((c == ':' || c == '.') ? spacerWidth : numberWidth) + 4f) * scale;
            width += charWidth;
        }

        return width;
    }

    private static float GetFramesWidth(int frames) {
        return (oParenWidth + cParenWidth + numberWidth * frames.ToString().Length) * FramesScale;
    }

    private static float GetProgressWidth() {
        return GetTimeWidth((VideoCapture.CurrentFrameCount / (float) VideoCapture.TargetFrameCount * 100.0f).ToString("0.00")) + oSquareWidth + cSquareWidth + percentWidth;
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
