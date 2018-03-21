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
      if (!Directory.Exists(_args.Source)) {
        using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MM-dd-yyyy}.log")) {
          var message = $"ERROR: Source directory doesn't exist {_args.Source}";

          sw.WriteLine(message);

          throw new Exception(message);
        }
      }
      
      ProcessDirectory(_args.Source);
    }

    private void ProcessDirectory(string directory) {
      if (_args.OnlyMedia) {
        ProcessMediaFiles(directory);
      }
      else {
        ProcessAllFiles(directory);
      }

      var folders = Directory.GetDirectories(directory);
      
      foreach (var f in folders) {
        ProcessDirectory(f);
      }

      using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MM-dd-yyyy}.log")) {
        if (_args.Delete && directory != _args.Source && !Directory.EnumerateFileSystemEntries(directory).Any()) {
          Directory.Delete(directory);
          sw.WriteLine($"DELETE: Empty folder {directory} successfully deleted");
        }
      }
    }

    private void ProcessAllFiles(string directory) {
      var files = Directory.EnumerateFiles(directory);
      var newFolders = directory.Replace(_args.Source, string.Empty);
      var targetDirectory = $"{_args.Target}{newFolders}";

      CopyFiles(targetDirectory, files);
    }

    private void ProcessMediaFiles(string directory) {
      var groups =
        Directory
          .EnumerateFiles(directory)
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
            targetDirectory = $@"{_args.Target}\Movies";
          }
        }

        if (!isFileValid) {
          continue;
        }

        var files = Directory.EnumerateFiles(directory, $"{group.Name}.*");
        var newFolders = directory.Replace(_args.Source, string.Empty);
        targetDirectory = $"{targetDirectory}{newFolders}";

        CopyFiles(targetDirectory, files);
      }
    }

    private void CopyFiles(string newDirectory, IEnumerable<string> files) {
      using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MM-dd-yyyy}.log")) {
        if (!Directory.Exists(newDirectory)) {
          Directory.CreateDirectory(newDirectory);
        }

        foreach (var file in files) {
          var fileName = Path.GetFileName(file);
          var targetFile = Path.Combine(newDirectory, fileName);

          if (!File.Exists(targetFile)) {
            File.Copy(file, targetFile);
            sw.WriteLine($"COPY: {file} successfully copied to {targetFile}");
          }

          if (_args.Delete) {
            File.Delete(file);
            sw.WriteLine($"DELETE: {file} successfully deleted");
          }
        }
      }
    }
  }
}