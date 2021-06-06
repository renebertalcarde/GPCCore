using System.ComponentModel.DataAnnotations;
using System;

namespace Module.DB
{

    public partial class tblUserPhotosMetaData
    { }
    [MetadataType(typeof(tblUserPhotosMetaData))]
    public partial class tblUserPhoto
    {
        public string GetPhotoString()
        {
            byte[] arr = Photo.ToArray();
            return Convert.ToBase64String(arr);
        }
    }
}