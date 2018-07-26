using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CopyApp.Copy {
  public class CopyProvider {
    private readonly CopyArgs _args;
    private bool _error;

    public CopyProvider(CopyArgs args) {
      _args = args;
    }

    public void Copy() {
      Prep();
      ProcessDirectory(_args.Source);
    }

    private void Prep() {
      var sb = new StringBuilder();

      PrepLogDirectory();
      ValidateLog(sb);
      ValidateSource(sb);
      ValidateTarget(sb);

      if (_error) {
        using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MMddyyyy}.log")) {
          sw.WriteLine(sb.ToString());
        }

        throw new Exception(sb.ToString());
      }
    }

    private void ValidateLog(StringBuilder sb) {
      if (_args.Log.Contains(_args.Source)) {
        _error = true;

        sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [ERROR]: Log file cannot be in source folder")
          .AppendLine($"  [Source]: {_args.Source}")
          .AppendLine($"  [Log]: {_args.Log}");
      }
    }

    private void ValidateSource(StringBuilder sb) {
      if (string.IsNullOrWhiteSpace(_args.Source)) {
        _error = true;

        sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [ERROR]: Source path cannot be empty");
      }

      if (!Directory.Exists(_args.Source)) {
        _error = true;

        sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [ERROR]: Source directory doesn't exist")
          .AppendLine($"  [Source]: {_args.Source}");
      }
    }

    private void ValidateTarget(StringBuilder sb) {
      if (string.IsNullOrWhiteSpace(_args.Target)) {
        _error = true;

        sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [ERROR]: Target path cannot be empty");
      }

      if (_args.Target.Contains(_args.Source)) {
        _error = true;

        sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [ERROR]: Cannot copy into the source folder")
          .AppendLine($"  [Source]: {_args.Source}")
          .AppendLine($"  [Target]: {_args.Target}");
      }
    }

    private void PrepLogDirectory() {
      if (string.IsNullOrWhiteSpace(_args.Log)) {
        throw new Exception($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [ERROR]: Log path cannot be empty");
      }

      if (!Directory.Exists(_args.Log)) {
        Directory.CreateDirectory(_args.Log);
      }
    }

    private void ProcessDirectory(string directory) {
      if (_args.Media) {
        ProcessMediaFiles(directory);
      }
      else {
        ProcessAllFiles(directory);
      }

      ProcessDirectories(directory);
    }

    private void ProcessAllFiles(string directory) {
      var files = Directory.EnumerateFiles(directory);
      var newDirectory = directory.Replace(_args.Source, string.Empty);
      var targetDirectory = $"{_args.Target}{newDirectory}";

      CopyFiles(targetDirectory, files);
    }

    private void ProcessMediaFiles(string directory) {
      var files = Directory.GetFiles(directory);

      var containsValidFile = false;
      var targetDirectory = string.Empty;

      foreach (var file in files) {
        var extension = Path.GetExtension(file);

        if (Constants.FileExtensions.VideoExtensions.Contains(extension)) {
          containsValidFile = true;
          targetDirectory = $@"{_args.Target}\Movies";

          break;
        }

        if (!Constants.FileExtensions.AudioExtensions.Contains(extension)) {
          continue;
        }

        containsValidFile = true;
        targetDirectory = $@"{_args.Target}\Music";
      }

      if (!containsValidFile) {
        using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MMddyyyy}.log")) {
          sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [IGNORE]: Directory had no valid media files");
          sw.WriteLine($"  [Directory]: {directory}");
        }

        return;
      }

      var newFolders = directory.Replace(_args.Source, string.Empty);
      targetDirectory = $"{targetDirectory}{newFolders}";

      CopyFiles(targetDirectory, files);
    }

    private void ProcessDirectories(string directory) {
      var directories = Directory.GetDirectories(directory);

      foreach (var d in directories) {
        ProcessDirectory(d);
      }

      if (!_args.Delete || directory == _args.Source || Directory.EnumerateFileSystemEntries(directory).Any()) {
        return;
      }

      using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MMddyyyy}.log")) {
        Directory.Delete(directory);
        sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [DELETE]: Empty folder deleted successfully");
        sw.WriteLine($"  [Folder]: {directory}");
      }
    }

    private void CopyFiles(string newDirectory, IEnumerable<string> files) {
      using (var sw = File.AppendText($@"{_args.Log}\{DateTime.Now:MMddyyyy}.log")) {
        if (!Directory.Exists(newDirectory)) {
          Directory.CreateDirectory(newDirectory);
        }

        foreach (var file in files) {
          var fileName = Path.GetFileName(file);
          var targetFile = Path.Combine(newDirectory, fileName);

          if (!File.Exists(targetFile)) {
            File.Copy(file, targetFile);
            sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [COPY]: File copied successfully");
            sw.WriteLine($"  [From]: {file}");
            sw.WriteLine($"  [To]: {targetFile}");
          }

          if (!_args.Delete) {
            continue;
          }

          File.Delete(file);
          sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss tt} [DELETE]: File deleted successfully");
          sw.WriteLine($"  [File]: {file}");
        }
      }
    }
  }
}