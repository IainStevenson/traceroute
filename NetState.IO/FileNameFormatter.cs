namespace NetState.IO
{
    public static class FileNameFormatter
    {
        public static string GetFilename(DirectoryInfo path, string address, string @extension)
        {
            return string.Format(@"{0}\RouteInfo-{1}-{2}.{3}", 
                    path.FullName, 
                    address,
                    string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now),
                    @extension
                    );
        }
    }
}