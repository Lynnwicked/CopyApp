using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CopyApp.Copy {
  public class CopyProvider {
    private readonly CopyArgs _args;

    public CopyProvider(CopyArgs args) {
      _args = args;
    }

    public void Copy() {
      using (var sw = new StreamWriter($@"{_args.Log}\{DateTime.Now:MM-dd-yyyy}.log")) {

      }

//      var sourceFiles = Directory.GetFiles(_args.Source);
//
//      ProcessFiles(sourceFiles);

//      var sourceFolders = Directory.GetDirectories(_args.Source);
//      var targetFolders = Directory.GetDirectories(_args.Target);

//      ProcessFolders(sourceFolders, targetFolders);
    }

    private void ProcessFolder(string folder) {
      var folders = Directory.GetDirectories(folder);

      foreach (var f in folders) {
        ProcessFolder(f);
      }
      
      var groups = 
        Directory
          .GetFiles(folder)
          .GroupBy(Path.GetFileNameWithoutExtension)
          .Select(x => new {Name = x.Key, Files = x});

      foreach (var group in groups) {
        var isFileValid = false;
        var targetDirectory = string.Empty;

        foreach (var file in group.Files) {
          var extension = Path.GetExtension(file);

          if (Constants.FileExtensions.AudioExtensions.Contains(extension)) {
            isFileValid = true;
            targetDirectory = $@"{_args.Target}\Music";
          }
          else if (Constants.FileExtensions.VideoExtensions.Contains(extension)) {
            isFileValid = true;
            targetDirectory =  $@"{_args.Target}\Movies";
          }
        }

        if (!isFileValid) {
          continue;
        }
        
        var files = Directory.EnumerateFiles(folder, $"{group.Name}.*");
        var newFolders = folder.Replace(_args.Source, string.Empty);
        targetDirectory = Path.Combine(targetDirectory, newFolders);

        ProcessFiles(targetDirectory, files);
      }
    }

    private void ProcessFiles(string newDirectory, IEnumerable<string> files) {
      
    }

    private void ProcessFiles(IEnumerable<string> files) {
      //using (var sw = new StreamWriter($@"{_args.Log}\{DateTime.Now:MM-dd-yyyy}.log")) {
      //  foreach (var file in files) {
      //    var fileName = Path.GetFileName(file);
      //    var extension = Path.GetExtension(file).ToLower();
      //    string targetDirectory;

      //    if (_audioExtensions.Contains(extension)) {
      //      targetDirectory = $@"{_args.Target}\Music";
      //    }
      //    else if (_videoExtensions.Contains(extension)) {
      //      targetDirectory =  $@"{_args.Target}\Movies";
      //    }
      //    else {
      //      var f = Path.GetFileNameWithoutExtension(file);
      //      continue;
      //    }

      //    var targetFile = $@"{targetDirectory}\{fileName}";

      //    if (!Directory.Exists(targetDirectory)) {
      //      Directory.CreateDirectory(targetDirectory);
      //    }
          
      //    if (File.Exists(targetFile)) {
      //      continue;
      //    }

      //    File.Copy(file, targetFile);
          
      //    sw.WriteLine($"SUCCESS: {file} successfully copied to {targetFile}");
      //  }
      //}
    }
  }
}