using CopyApp.Copy;
using PowerArgs;

namespace CopyApp {
  [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
  public class Options {
    [HelpHook, ArgShortcut("-h"), ArgDescription("Shows this help")]
    public bool Help { get; set; }

    [ArgActionMethod, ArgDescription("Copy files from source to target folders")]
    public void Copy(CopyArgs args) {
      var provider = new CopyProvider(args);

      provider.Copy();
    }
  }
}