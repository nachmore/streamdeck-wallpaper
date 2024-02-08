using Imageflow.Fluent;

namespace StreamDeckWallpaper
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      var sourceGif = @"c:\temp\test.gif";
      var outputGif = @"c:\temp\test-out";

      if (args.Length > 0)
        sourceGif = args[0];

      byte[] buff = System.IO.File.ReadAllBytes(sourceGif);

      for (var y = 0; y < 3; y++)
      {
        for (var x = 0; x < 5; x++)
        {
          // new + Decode needs to be done for each action, even though it's on the same set of bytes
          var ij = new ImageJob();

          var x1 = x * 72;
          var y1 = y * 72;
          var x2 = x1 + 72;
          var y2 = y1 + 72;

          var cropped = await ij.Decode(buff)
                                .Crop(x1, y1, x2, y2)
                                .EncodeToBytes(new GifEncoder())
                                .Finish()
                                .InProcessAsync();

          var bytes = cropped?.First?.TryGetBytes()?.Array;

          if (bytes == null)
          {
            Console.Error.WriteLine("ERROR: Cannot read image");
            return;
          }

          try
          {
            var outFilename = $"{outputGif}-{y*5 + x + 1}.gif";
            using (var fs = new FileStream(outFilename, FileMode.Create, FileAccess.Write))
            {
              Console.WriteLine($"Writing: {outFilename}");
              fs.Write(bytes, 0, bytes.Length);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine("Exception caught in process: {0}", ex);
          }
        }
      }

      Console.WriteLine("Hello, World!");
    }
  }
}
