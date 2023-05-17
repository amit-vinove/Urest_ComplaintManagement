using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using UrestComplaintManagement.Context;

namespace UrestComplaintManagement
{
    public class ContentRepository
    {
        private readonly DBContext db = new DBContext();
        public int UploadImageInDataBase(HttpPostedFileBase file, ContentViewModel contentViewModel, string connstring)
        {
            SqlConnection sqlConnection = new SqlConnection(connstring);
            sqlConnection.Open();
            int ret = 0;

            try
            {
                contentViewModel.Image = ConvertToBytes(file);

                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = sqlConnection;

                sqlCommand.CommandText = "delete from KycImages where kycid='" + contentViewModel.KycID + "' and description = '" + contentViewModel.Description + "'";
                sqlCommand.ExecuteScalar();

                sqlCommand.CommandText = "Insert into KycImages([Title],[Description],[ProfileImage],[Contents],[KycID],[Image]) values('" + contentViewModel.Title + "','" + contentViewModel.Description + "', @Image, ' ', " + contentViewModel.KycID + ",' ')";
                sqlCommand.Parameters.AddWithValue("@Image", contentViewModel.Image);
                sqlCommand.ExecuteNonQuery();

                ret = 1;
            }
            catch(Exception ex)
            {
                string error = ex.Message;
            }

            sqlConnection.Close();
            return ret;
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
    }
}