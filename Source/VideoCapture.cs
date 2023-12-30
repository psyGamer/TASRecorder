using Celeste.Mod.TASRecorder.Util;
using Celeste.Pico8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Celeste.Mod.TASRecorder;

public static class VideoCapture {

    private static Hook? hook_Game_Tick;
    private static Hook? hook_Celeste_Update;
    private static Hook? hook_GraphicsDevice_SetRenderTarget;

    internal static void Load() {
        hook_Game_Tick = new Hook(
            typeof(Game).GetMethod("Tick")
            ?? throw new Exception($"{typeof(Game)} without Tick???"),
            On_Game_Tick
        );
        hook_Celeste_Update = new Hook(
            typeof(Celeste).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception($"{typeof(Celeste)} without Update???"),
            On_Celeste_Update
        );
        hook_GraphicsDevice_SetRenderTarget = new Hook(
            typeof(GraphicsDevice).GetMethod("SetRenderTarget", new[] { typeof(RenderTarget2D) })
            ?? throw new Exception($"{typeof(GraphicsDevice)} without SetRenderTarget???"),
            On_GraphicsDevice_SetRenderTarget
        );

        On.Monocle.Engine.RenderCore += On_Engine_RenderCore;
        IL.Monocle.Engine.RenderCore += IL_Engine_RenderCore;
    }
    internal static void Unload() {
        hook_Game_Tick?.Dispose();
        hook_Celeste_Update?.Dispose();
        hook_GraphicsDevice_SetRenderTarget?.Dispose();

        On.Monocle.Engine.RenderCore -= On_Engine_RenderCore;
        IL.Monocle.Engine.RenderCore -= IL_Engine_RenderCore;
    }

    internal static void StartRecording() {
        BlackFadeStart = 0.0f;
        BlackFadeEnd = 0.0f;
        blackFadeTimer = 0.0f;

        BlackFadeText = "";
        BlackFadeTextPosition = Celeste.TargetCenter;
        BlackFadeTextScale = 1.0f;
        BlackFadeTextColor = Color.White;
    }

    public static float BlackFadeStart = 0.0f;
    public static float BlackFadeEnd = 0.0f;

    public static string BlackFadeText = "";
    public static Vector2 BlackFadeTextPosition = Celeste.TargetCenter;
    public static float BlackFadeTextScale = 1.0f;
    public static Color BlackFadeTextColor = Color.White;

    private static float blackFadeTimer = 0.0f;
    private static float blackFadeAlpha => BlackFadeStart <= BlackFadeEnd
        ? Calc.Map(blackFadeTimer, BlackFadeStart, BlackFadeEnd)
        : Calc.Map(blackFadeTimer, BlackFadeEnd, BlackFadeStart);

    private static TimeSpan RecordingDeltaTime => TASRecorderModule.Settings.FPS switch {
        60 => TimeSpan.FromTicks(166667L),
        30 => TimeSpan.FromTicks(333334L),
        24 => TimeSpan.FromTicks(416667L),
        _ => TimeSpan.FromSeconds(1.0f / TASRecorderModule.Settings.FPS),
    } * TASRecorderModule.Settings.Speed;

    // Hijacks SetRenderTarget(null) calls to point to our captureTarget instead of the back buffer.
    private static bool hijackBackBuffer = false;
    private static RenderTarget2D? captureTarget = null;
    private static bool tickHookActive = false;

    private static int oldWidth;
    private static int oldHeight;
    private static Matrix oldMatrix;
    private static Viewport oldViewport;
    private static bool updateHappened;

    private static unsafe void CaptureFrame() {
        if (captureTarget == null) {
            Log.Error("Capture target was null!");
            return;
        }

        int width = captureTarget.Width;
        int height = captureTarget.Height;

        Color[] buffer = new Color[width * height];
        captureTarget.GetData(buffer);

        RecordingManager.Encoder.PrepareVideo(width, height);
        fixed (Color* srcData = buffer) {
            int srcRowStride = width * sizeof(Color);
            int dstRowStride = RecordingManager.Encoder.VideoRowStride;

            byte* src = (byte*) srcData;
            byte* dst = RecordingManager.Encoder.VideoData;

            for (int i = 0; i < height; i++) {
                NativeMemory.Clear(dst, (nuint) dstRowStride);
                NativeMemory.Copy(src, dst, (nuint) srcRowStride);
                src += srcRowStride;
                dst += dstRowStride;
            }
        }
        RecordingManager.Encoder.FinishVideo();
    }

    // Taken from Engine.UpdateView(), but without depending on presentation parameters
    private static void UpdateEngineView(int width, int height) {
        if (width / (float) Engine.Width > height / (float) Engine.Height) {
            Engine.ViewWidth = (int) (height / (float) Engine.Height * Engine.Width);
            Engine.ViewHeight = height;
        } else {
            Engine.ViewWidth = width;
            Engine.ViewHeight = (int) (width / (float) Engine.Width * Engine.Height);
        }
        float ratio = Engine.ViewHeight / (float) Engine.ViewWidth;
        Engine.ViewWidth -= Engine.ViewPadding * 2;
        Engine.ViewHeight -= (int) (ratio * Engine.ViewPadding * 2f);
        Engine.ScreenMatrix = Matrix.CreateScale(Engine.ViewWidth / (float) Engine.Width);
        Viewport viewport = default;
        viewport.X = (int) (width / 2f - Engine.ViewWidth / 2);
        viewport.Y = (int) (height / 2f - Engine.ViewHeight / 2);
        viewport.Width = Engine.ViewWidth;
        viewport.Height = Engine.ViewHeight;
        viewport.MinDepth = 0f;
        viewport.MaxDepth = 1f;
        Engine.Viewport = viewport;
    }

    // We need to use a modified version of the main game loop to avoid skipping frames
    private delegate void orig_Game_Tick(Game self);
    private static void On_Game_Tick(orig_Game_Tick orig, Game self) {
        if (!RecordingManager.RecordingVideo) {
            updateHappened = false;
            orig(self);

            // We started recording on this frame.
            if (RecordingManager.RecordingVideo) {
                // The first half of this is inside on_Game_Update
                hijackBackBuffer = false;
                Engine.ViewWidth = oldWidth;
                Engine.ViewHeight = oldHeight;
                Engine.ScreenMatrix = oldMatrix;
                Engine.Viewport = oldViewport;

                if (captureTarget == null) {
                    Log.Error("Capture RenderTarget is null! Skipping first frame!");
                    return;
                }

                CaptureFrame();
                if (!IsLoading()) {
                    RecordingManager.CurrentFrameCount++;
                }

                // Don't rerender to the screen or display the indicator, because drawing already ended.
            }

            return;
        }

        Syncing.SyncWithAudio();

        FNAPlatform.PollEvents(self, ref self.currentAdapter, self.textInputControlDown, ref self.textInputSuppress);

        self.accumulatedElapsedTime += RecordingDeltaTime;
        if (self.accumulatedElapsedTime < self.TargetElapsedTime) return;

        var device = Celeste.Instance.GraphicsDevice;
        if (captureTarget == null || captureTarget.Width != TASRecorderModule.Settings.VideoWidth || captureTarget.Height != TASRecorderModule.Settings.VideoHeight) {
            captureTarget?.Dispose();
            captureTarget = new RenderTarget2D(device, TASRecorderModule.Settings.VideoWidth, TASRecorderModule.Settings.VideoHeight, mipMap: false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
        }

        self.gameTime.ElapsedGameTime = self.TargetElapsedTime;
        self.gameTime.TotalGameTime = self.TargetElapsedTime;

        while (self.accumulatedElapsedTime >= self.TargetElapsedTime) {
            self.accumulatedElapsedTime -= self.TargetElapsedTime;

            // Avoid recording frames twice
            tickHookActive = true;
            self.Update(self.gameTime);
            tickHookActive = false;
            RecordingRenderer.Update();

            blackFadeTimer = Calc.Approach(blackFadeTimer, BlackFadeEnd, Engine.DeltaTime);
            if (BlackFadeStart <= BlackFadeEnd) {
                blackFadeTimer = Math.Clamp(blackFadeTimer, BlackFadeStart, BlackFadeEnd);
            } else {
                blackFadeTimer = Math.Clamp(blackFadeTimer, BlackFadeEnd, BlackFadeStart);
            }

            // Don't increase frame count while loading, since CelesteTAS pauses inputs as well
            if (!IsLoading()) {
                RecordingManager.CurrentFrameCount++;
            }
        }

        if (self.BeginDraw()) {
            // Clear the first frame, since the C# Debug Console apparently does some weird stuff...
            if (RecordingManager.CurrentFrameCount == 1) {
                device.SetRenderTarget(captureTarget);
                device.Clear(Color.Black);
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
                Draw.SpriteBatch.End();
            }

            oldWidth = Engine.ViewWidth;
            oldHeight = Engine.ViewHeight;
            oldMatrix = Engine.ScreenMatrix;
            oldViewport = Engine.Viewport;
            UpdateEngineView(captureTarget.Width, captureTarget.Height);
            hijackBackBuffer = true;

            self.Draw(self.gameTime);

            hijackBackBuffer = false;
            Engine.ViewWidth = oldWidth;
            Engine.ViewHeight = oldHeight;
            Engine.ScreenMatrix = oldMatrix;
            Engine.Viewport = oldViewport;

            // Recording might have stopped with the last update
            if (RecordingManager.RecordingVideo){
                CaptureFrame();
            }

            // Render our capture to the screen
            var matrix = Matrix.CreateScale(Engine.ViewWidth / (float) captureTarget.Width);

            device.SetRenderTarget(null);
            device.Clear(Color.Black);
            device.Viewport = Engine.Viewport;
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, matrix);
            Draw.SpriteBatch.Draw(captureTarget, Vector2.Zero, Color.White);
            Draw.SpriteBatch.End();

            RecordingRenderer.Render();

            self.EndDraw();
        }
    }

    // After this update, Render would be called from orig_Tick. That means we would miss the first frame.
    // If the game is currently lagging, more than 1 frame could be skipped.
    private delegate void orig_Celeste_Update(Celeste self, GameTime gameTime);
    private static void On_Celeste_Update(orig_Celeste_Update orig, Celeste self, GameTime gameTime) {
        if (updateHappened && !tickHookActive && RecordingManager.RecordingVideo) {
            // We are currently lagging. Don't update to avoid skipping frames.
            return;
        }

        orig(self, gameTime);

        // Maybe the recording started outside of an update.
        // This ensures we get at least 1 update
        updateHappened = true;

        // For some reason, when recording with CelesteTAS, the first frame is missing the level
        // when recording from here, but not when recording from the original Tick
        if (!tickHookActive && RecordingManager.RecordingVideo) {
            var device = Celeste.Instance.GraphicsDevice;
            if (captureTarget == null || captureTarget.Width != TASRecorderModule.Settings.VideoWidth || captureTarget.Height != TASRecorderModule.Settings.VideoHeight) {
                captureTarget?.Dispose();
                captureTarget = new RenderTarget2D(device, TASRecorderModule.Settings.VideoWidth, TASRecorderModule.Settings.VideoHeight, mipMap: false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
            }

            oldWidth = Engine.ViewWidth;
            oldHeight = Engine.ViewHeight;
            oldMatrix = Engine.ScreenMatrix;
            oldViewport = Engine.Viewport;
            UpdateEngineView(captureTarget.Width, captureTarget.Height);
            hijackBackBuffer = true;
            // Second half of the capture is inside on_Game_Tick
        }
    }

    private static void On_Engine_RenderCore(On.Monocle.Engine.orig_RenderCore orig, Engine self) {
        orig(self);

        if (RecordingManager.RecordingVideo && blackFadeAlpha > 0.0001) {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Engine.ScreenMatrix);
            Draw.Rect(-10.0f, -10.0f, Celeste.TargetWidth + 20.0f, Celeste.TargetHeight + 20.0f, Color.Black * blackFadeAlpha);
            if (!string.IsNullOrWhiteSpace(BlackFadeText)) {
                ActiveFont.DrawOutline(
                    BlackFadeText, BlackFadeTextPosition, new Vector2(0.5f, 0.5f), new Vector2(BlackFadeTextScale, BlackFadeTextScale),
                    BlackFadeTextColor * blackFadeAlpha, 2.0f * BlackFadeTextScale, Color.Black * blackFadeAlpha);
            }
            Draw.SpriteBatch.End();
        }

        // Render the banner fadeout after the FNA main loop hook is disabled
        if (RecordingRenderer.ShouldUpdate && !RecordingManager.Recording) {
            RecordingRenderer.Update();
            RecordingRenderer.Render();
        }
    }

    // "I would recommend joining the MonoMod Discord and letting them know you summoned a demon" - Popax21
    // For _some_ reason, on-hooking Engine.RenderCore, even when just calling orig, causes
    // the this.GraphicsDevice.SetRenderTarget(null) line to simply not execute.
    // Even more bizarre is the fact that this EMPTY il-hook fixes it...
    // See https://discord.com/channels/403698615446536203/908809001834274887/1161328853172617236 for a bit more context.
    private static void IL_Engine_RenderCore(ILContext ctx) { }

    private delegate void orig_GraphicsDevice_SetRenderTarget(GraphicsDevice self, RenderTarget2D? target);
    private static void On_GraphicsDevice_SetRenderTarget(orig_GraphicsDevice_SetRenderTarget orig, GraphicsDevice self, RenderTarget2D? target) {
        // Redirect the backbuffer to our render target, to capture the frame.
        if (hijackBackBuffer && target is null) {
            orig(self, captureTarget);
            return;
        }

        orig(self, target);
    }

    // Taken from CelesteTAS. Makes sure the recording is also paused, while CelesteTAS is paused
    private static bool IsLoading() {
        switch (Engine.Scene) {
            case Level level:
                return level.IsAutoSaving() && level.Session.Level == "end-cinematic";
            case SummitVignette summit:
                return !summit.ready;
            case Overworld overworld:
                return overworld.Current is OuiFileSelect {SlotIndex: >= 0} slot && slot.Slots[slot.SlotIndex].StartingGame ||
                       overworld.Next is OuiChapterSelect && UserIO.Saving ||
                       overworld.Next is OuiMainMenu && (UserIO.Saving || Everest._SavingSettings);
            case Emulator emulator:
                return emulator.game == null;
            default:
                bool isLoading = Engine.Scene is LevelExit or LevelLoader or GameLoader || Engine.Scene.GetType().Name == "LevelExitToLobby";
                return isLoading;
        }
    }
}
