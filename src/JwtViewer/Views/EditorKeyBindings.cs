using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia.Input;
using AvaloniaEdit;

namespace JwtViewer.Views;

public static class EditorKeyBindings
{
    public static readonly KeyBinding[] ForMac = 
    {
        Bind(EditingCommands.MoveToLineStart, "cmd+left"),
        Bind(EditingCommands.MoveToLineEnd, "cmd+right"),
        Bind(EditingCommands.MoveToDocumentStart, "cmd+up"),
        Bind(EditingCommands.MoveToDocumentEnd, "cmd+down"),
        Bind(EditingCommands.MoveLeftByWord, "alt+left"),
        Bind(EditingCommands.MoveRightByWord, "alt+right"),
        Bind(EditingCommands.SelectLeftByWord, "shift+alt+left"),
        Bind(EditingCommands.SelectRightByWord, "shift+alt+right"),
        Bind(EditingCommands.SelectToLineStart, "shift+cmd+left"),
        Bind(EditingCommands.SelectToLineEnd, "shift+cmd+right"),
    };

    private static KeyBinding Bind(ICommand command, string gesture) => new()
    {
        Command = command,
        Gesture = KeyGesture.Parse(gesture)
    };

    public static void SetPlatformSpecificKeyBindings(this InputElement editor)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }
        
        editor.KeyBindings.AddRange(ForMac);
    }
}