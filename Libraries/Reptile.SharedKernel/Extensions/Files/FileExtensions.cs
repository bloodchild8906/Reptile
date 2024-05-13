namespace Reptile.SharedKernel.Extensions.Files;

public static class FileExtensions
{
    public static string? ToJavaScriptExtension(this string str, bool appendToExisting = false)
    {
        var extension = Path.GetExtension(str);
        if (string.IsNullOrEmpty(extension) || appendToExisting)
        {
            return $"{str}.js";
        }
        else if(appendToExisting)
        {
            return $"{Path.GetFileNameWithoutExtension(str)}.js";
        }
        return $"{Path.GetFileNameWithoutExtension(str)}.js";
    }


    public static bool IsFileExtensionValid(this string fExt)
    {
        if (string.IsNullOrEmpty(fExt) || !fExt.StartsWith("."))
        {
            return false;
        }

        var validExtensions = new string[]
        {
            FileTypeExtensions.Jpg, FileTypeExtensions.Jpeg, FileTypeExtensions.Png,
            FileTypeExtensions.Gif, FileTypeExtensions.Pdf, FileTypeExtensions.Doc,
            FileTypeExtensions.Docx, FileTypeExtensions.Xls, FileTypeExtensions.Xlsx,
            FileTypeExtensions.Ppt, FileTypeExtensions.Pptx, FileTypeExtensions.Js,
            FileTypeExtensions.Css, FileTypeExtensions.Bmp, FileTypeExtensions.Wmv,
            FileTypeExtensions.Swf, FileTypeExtensions.Txt
        };

        return validExtensions.Contains(fExt);
    }

    public static class FileTypeExtensions
    {
        public const string Jpg = ".jpg";
        public const string Jpeg = ".jpeg";
        public const string Png = ".png";
        public const string Gif = ".gif";
        public const string Pdf = ".pdf";
        public const string Doc = ".doc";
        public const string Docx = ".docx";
        public const string Xls = ".xls";
        public const string Xlsx = ".xlsx";
        public const string Ppt = ".ppt";
        public const string Pptx = ".pptx";
        public const string Js = ".js";
        public const string Css = ".css";
        public const string Bmp = ".bmp";
        public const string Wmv = ".wmv";
        public const string Swf = ".swf";
        public const string Txt = ".txt";
        
    }
}