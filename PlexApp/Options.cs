using PlexApp.Plex;
using PowerArgs;

namespace PlexApp {
  [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
  public class Options {
    [HelpHook, ArgShortcut("-h"), ArgDescription("Shows this help")]
    public bool Help { get; set; }

    [ArgActionMethod, ArgDescription("Plex importer")]
    public void Import(PlexArgs args) {
      var importer = new Importer(args);

      importer.Process();
    }
  }
}