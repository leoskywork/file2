namespace File2
{
    //class Temp
    //{
    //}

    /*
     
    https://stackoverflow.com/questions/882686/asynchronous-file-copy-move-in-c-sharp
    https://stackoverflow.com/questions/14587494/writing-to-file-using-streamwriter-much-slower-than-file-copy-over-slow-network
     public static async Task CopyFileAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken)
  {
     var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
     var bufferSize = 4096;

     using (var sourceStream = 
           new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))

     using (var destinationStream = 
           new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, fileOptions))

        await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
                                   .ConfigureAwait(continueOnCapturedContext: false);
  }








     
     
     */
}
