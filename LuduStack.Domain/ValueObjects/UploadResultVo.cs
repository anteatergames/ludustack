using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.ValueObjects
{
    public class UploadResultVo
    {
        public bool Success { get; set; }

        public bool Uploaded => Success;

        public string Message { get; set; }

        public MediaType? MediaType { get; set; }

        public string Filename { get; set; }

        public long? FileSize { get; set; }

        public string OldImage { get; set; }

        public Guid? RelatedId { get; set; }

        public string Url { get; set; }

        public UploadResultVo(string message)
        {
            Success = false;
            Message = message;
        }

        public UploadResultVo(bool success, MediaType mediaType, string fileName, string message)
        {
            Success = success;
            MediaType = mediaType;
            Filename = fileName;
            Message = message;
        }
    }
}