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
      CheckForErrors();

      ProcessDirectory(_args.Source);
    }

    private void CheckForErrors() {
      var error = false;
      var message = string.Empty;

      if (_args.Log.Contains(_args.Source)) {
        throw new Exception($"ERROR: Cannot create log the source folder{Environment.NewLine}  Source: {_args.Source}{Environment.NewLine}  Log: {_args.Log}");
      }

      if (!Directory.Exists(_args.Source)) {
        error = true;
        message = $"ERROR: Source directory doesn't exist{Environment.NewLine}  Source: {_args.Source}";
      }

      if (_args.Target.Contains(_args.Source)) {
        error = true;
        message = $"ERROR: Cannot copy into the source folder{Environment.NewLine}  Source: {_args.Source}{Environment.NewLine}  Target: {_args.Target}";
      }

      if (!error) {
        return;
      }

      using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MM-dd-yyyy}.log")) {
        sw.WriteLine(message);
      }

      throw new Exception(message);
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

      if (!_args.Delete || directory == _args.Source || Directory.EnumerateFileSystemEntries(directory).Any()) {
        return;
      }

      using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MM-dd-yyyy}.log")) {
        Directory.Delete(directory);
        sw.WriteLine($"DELETE: Empty folder deleted successfully");
        sw.WriteLine($"  Folder: {directory}");
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
            sw.WriteLine($"COPY: File copied successfully");
            sw.WriteLine($"  From: {file}");
            sw.WriteLine($"  To: {targetFile}");
          }

          if (!_args.Delete) {
            continue;
          }

          File.Delete(file);
          sw.WriteLine($"DELETE: File deleted successfully");
          sw.WriteLine($"  File: {file}");
        }
      }
    }
  }
}