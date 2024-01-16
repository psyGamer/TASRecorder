using System.Collections;
using Microsoft.Xna.Framework;
using Celeste.Mod.Core;
using Celeste.Mod.UI;
using Monocle;

namespace Celeste.Mod.TASRecorder;

public class OuiSetupHWAccel : Oui {

    internal static void Load() {
        On.Celeste.OuiTitleScreen.IsStart += On_OuiTitleScreen_IsStart;
    }
    internal static void Unload() {
        On.Celeste.OuiTitleScreen.IsStart -= On_OuiTitleScreen_IsStart;
    }

    private float fade;

    private Wiggler? confirmWiggle;
    private float confirmWiggleDelay;

    private TextMenu? menu;

    private static bool DontShow => false;

    public override IEnumerator Enter(Oui? from) {
        yield return null;
        Overworld.ShowInputUI = false;
        Focused = true;
        Visible = true;

        fade = 1.0f;

        Add(confirmWiggle = Wiggler.Create(0.4f, 4f));

        // The next field doesn't really do anything, but because it's a OuiTitleScreen, we get snow. Yay!
        // Also, this is technically accurate, because the next menu will be the title screen.
        Overworld.Next = Overworld.GetUI<OuiTitleScreen>();

        menu = CreateMenu();
        menu.X = Celeste.TargetWidth / 2.0f;
        menu.Justify.Y = -0.45f; // WHY doesnt menu.Y just work?!
        Scene.Add(menu);
    }
    public override IEnumerator Leave(Oui next) {
        Overworld.ShowInputUI = true;

        float menuX = menu!.X;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeInOut, 0.6f, start: true);
        tween.OnUpdate = delegate(Tween t) {
            fade = 1f - t.Percent;
            menu.X = MathHelper.Lerp(menuX, -menu.Width, t.Eased);
        };
        Add(tween);
        yield return tween.Wait();

        Remove(confirmWiggle);
        menu.RemoveSelf();

        Focused = false;
        Visible = false;
        Overworld.Maddy.Hide();
    }

    public override void Update() {
        if ((Input.MenuConfirm?.Pressed ?? false) && confirmWiggleDelay <= 0f) {
            confirmWiggle?.Start();
            confirmWiggleDelay = 0.5f;
        }
        confirmWiggleDelay -= Engine.DeltaTime;

        base.Update();
    }

    public override void Render() {
        Draw.Rect(-10f, -10f, Celeste.TargetWidth + 20.0f, Celeste.TargetHeight + 20.0f, Color.Black);

        const float TitleSize = 2.0f;
        string title = Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_TITLE");
        var titlePos = new Vector2((Celeste.TargetWidth - ActiveFont.Measure(title).X * TitleSize) / 2.0f, 275.0f);
        ActiveFont.DrawOutline(title, titlePos, new Vector2(0, 1), new Vector2(TitleSize), Color.White * fade, 2.0f, Color.Black * fade);

        const float SubtitleSize = 1.0f;
        string subtitle = Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_SUBTITLE");
        var subtitlePos = new Vector2((Celeste.TargetWidth - ActiveFont.Measure(subtitle).X * SubtitleSize) / 2.0f, 325.0f);
        ActiveFont.DrawOutline(subtitle, subtitlePos, new Vector2(0, 1), new Vector2(SubtitleSize), Color.White * fade, 2.0f, Color.Black * fade);

        const float DescSize = 0.75f;
        string desc = Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_DESC");
        var descPos = new Vector2((Celeste.TargetWidth - ActiveFont.Measure(desc).X * DescSize) / 2.0f, 600.0f);
        ActiveFont.DrawOutline(desc, descPos, new Vector2(0, 1), new Vector2(DescSize), Color.Gray * fade, 2.0f, Color.Black * fade);

        if (menu?.Focused ?? false) {
            var confirmPosition = new Vector2(Celeste.TargetWidth - 40.0f, Celeste.TargetHeight - 56.0f);
            string confirmLabel = Dialog.Clean("ui_confirm");
            ButtonUI.Render(confirmPosition, confirmLabel, Input.MenuConfirm, 0.5f, 1f, (confirmWiggle?.Value ?? 0.0f) * 0.05f, fade);
        }

        base.Render();
    }

    private TextMenu CreateMenu() {
        return new TextMenu {
            new TextMenu.Button(Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_None")).Pressed(() => Selection(HWAccelType.None)),
            new TextMenu.Button(Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_QSV")).Pressed(() => Selection(HWAccelType.QSV)),
            new TextMenu.Button(Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_NVENC")).Pressed(() => Selection(HWAccelType.NVENC)),
            new TextMenu.Button(Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_AMF")).Pressed(() => Selection(HWAccelType.AMF)),
            new TextMenu.Button(Dialog.Clean("TAS_RECORDER_SETUPHWACCEL_VideoToolbox")).Pressed(() => Selection(HWAccelType.VideoToolbox)),
        };
    }
    private void Selection(HWAccelType type) {
        TASRecorderModule.Settings.HardwareAccelerationType = type;
        GotoActualTitlescreen();
    }

    private void GotoActualTitlescreen() {
        var ouiTitleScreen = Overworld.Goto<OuiTitleScreen>();
        ouiTitleScreen.IsStart(Overworld, Overworld.StartMode.Titlescreen);
        ouiTitleScreen.IsStart(Overworld, Overworld.StartMode.MainMenu);
        ouiTitleScreen.alpha = 0.0f;
        ouiTitleScreen.fade = 1.0f;
        ouiTitleScreen.Visible = true;
    }

    public override bool IsStart(Overworld overworld, Overworld.StartMode start) {
        // The only menus, which trigger on Titlescreen are OuiTitleScreen and OuiOOBE. Both are handled.
        // Let's just hope there isn't another mod which does the same thing...
        if (DontShow || start != Overworld.StartMode.Titlescreen) return false;

        Add(new Coroutine(Enter(null)));
        return true;
    }

    private static bool On_OuiTitleScreen_IsStart(On.Celeste.OuiTitleScreen.orig_IsStart orig, OuiTitleScreen self, Overworld overworld, Overworld.StartMode start) {
        if (DontShow) return orig(self, overworld, start);

        if (CoreModule.Settings.CurrentVersion == null && !overworld.IsCurrent<OuiOOBE>()) {
            // The OOBE menu should be shown first. We intercept the transition from OOBE to title screen.
            start = Overworld.StartMode.MainMenu;
            return orig(self, overworld, start);
        }

        // Change the start mode, so the title screen doesnt think it should be shown.
        if (start == Overworld.StartMode.Titlescreen) {
            start = Overworld.StartMode.MainMenu;
        }
        return orig(self, overworld, start);
    }
}
