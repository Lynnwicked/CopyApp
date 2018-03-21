using PowerArgs;

namespace CopyApp.Copy {
  public class CopyArgs {
    [ArgShortcut("source"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgRequired, ArgDescription("Path of source folder"), ArgPosition(1)]
    public string Source { get; set; }

    [ArgShortcut("target"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgRequired, ArgDescription("Path of target folder"), ArgPosition(2)]
    public string Target { get; set; }

    [ArgShortcut("log"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgRequired, ArgDescription("Path of log file"), ArgPosition(3)]
    public string Log { get; set; }

    [ArgShortcut("media"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgDefaultValue(true), ArgDescription("Only target media files (e.g. video or audio files only)")]
    public bool OnlyMedia { get; set; }

    [ArgShortcut("delete"), ArgShortcut(ArgShortcutPolicy.ShortcutsOnly), ArgDefaultValue(false), ArgDescription("Delete copied source files and any empty folders found")]
    public bool Delete { get; set; }
  }
}