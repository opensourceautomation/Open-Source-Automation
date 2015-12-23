namespace OSAE
{
    using System;
    using System.Drawing;
    using System.Data;
    using MySql.Data.MySqlClient;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using Dapper;
    using System.Linq;

    public class OSAEImageManager
    {
        /// <summary>
        /// Used to get access to the logging facility
        /// </summary>
        private Logging logging = Logging.GetLogger();

        /// <summary>
        /// Adds an image to the DB
        /// </summary>
        /// <param name="osaeImage">The image information to add</param>
        public int AddImage(OSAEImage osaeImage)
        {
            return AddImage(osaeImage.Name, osaeImage.Type, osaeImage.Data);
        }

        /// <summary>
        /// Adds an image to the OSAE DB
        /// </summary>
        /// <param name="name">The name of the image this should not include the path or extension</param>
        /// <param name="type">the type of the image e.g. jpg, png do not include the .</param>
        /// <param name="imageData">the binary data of the image</param>
        public int AddImage(string name, string type, byte[] imageData)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                MemoryStream ms1 = new MemoryStream(imageData);
                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms1;
                bitmapImage.EndInit();

                command.CommandText = "CALL osae_sp_image_add (@pimage_data, @pimage_name, @pimage_type, @pimage_width, @pimage_height, @pimage_dpi)";
                command.Parameters.AddWithValue("@pimage_data", imageData);
                command.Parameters.AddWithValue("@pimage_name", name);
                command.Parameters.AddWithValue("@pimage_type", type);
                command.Parameters.AddWithValue("@pimage_width", bitmapImage.Width);
                command.Parameters.AddWithValue("@pimage_height", bitmapImage.Height);
                command.Parameters.AddWithValue("@pimage_dpi", bitmapImage.DpiX);
                command.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32));
                command.Parameters["@id"].Direction = System.Data.ParameterDirection.Output;
                DataSet ds = OSAESql.RunQuery(command);

                return int.Parse(ds.Tables[0].Rows[0][0].ToString());

            }
        }

        /// <summary>
        /// Deletes an image from the DB
        /// </summary>
        /// <param name="imageId">The Id of the image to delete</param>
        public void DeleteImage(int imageId)
        {
            using (var connection = new MySqlConnection(Common.ConnectionString))
            {
                connection.Open();

                connection.Execute("osae_sp_image_delete",
                    new { pimage_id = imageId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Gets an image from the DB
        /// </summary>
        /// <param name="imageId">The id of the image to get</param>
        /// <param name="returnNullIfNotExist">If the image is not found, should null be returned? Otherwise return empty image object.</param>
        /// <returns>OSAEImage or null</returns>
        public OSAEImage GetImage(int imageId, bool returnNullIfNotExist)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    connection.Open();

                    var image = connection.Query<OSAEImage>("SELECT image_id as 'ID', image_name as 'Name', image_type as 'Type', image_data as 'Data' FROM osae_images WHERE image_id = @id",
                        new { id = imageId }).FirstOrDefault();

                    if (image == null && returnNullIfNotExist)
                    {
                        return null;
                    }
                    else if (image == null && returnNullIfNotExist == false)
                    {
                        throw new Exception("No Image Found");
                    }
                    else
                    {
                        return image;
                    }
                }               
            }
            catch (Exception ex)
            {
                throw new Exception("API - GetImage - Failed to get image with id: " + imageId.ToString() + ", exception encountered: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Gets an image from the DB
        /// </summary>
        /// <param name="imageId">The id of the image to get</param>
        /// <returns>the requested image</returns>
        public OSAEImage GetImage(int imageId)
        {
            return GetImage(imageId, false);
        }

        /// <summary>
        /// Gets an image from the DB
        /// </summary>
        /// <param name="imageId">The id of the image to get</param>
        /// <returns>the requested image</returns>
        public OSAEImage GetImage(string imageName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    connection.Open();

                    var image = connection.Query<OSAEImage>("SELECT image_id as 'ID', image_name as 'Name', image_type as 'Type', image_data as 'Data' FROM osae_images WHERE image_name = @name",
                        new { name = imageName }).FirstOrDefault();

                    if (image != null)
                        return image;
                }

                // We shouldn't log inside the API, so let's throw an exception to inform the calling application that there's an issue.
                throw new Exception("API - Failed to get requested image from DB: " + imageName);
            }
            catch (Exception ex)
            {
                //throw new Exception("API - Failed to get requested image from DB: " + imageName + ", exception encountered: " + ex.Message, ex);
                return null;
            }
            
        }

        /// <summary>
        /// Returns a list of available images without the type or data for performence reasons.
        /// </summary>
        /// <returns>a list of images</returns>
        public List<OSAEImage> GetImageList()
        {
            List<OSAEImage> imageList = new List<OSAEImage>();

            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT image_id, image_name FROM osae_images", connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        OSAEImage osaeImage = new OSAEImage();
                        osaeImage.ID = reader.GetUInt32("image_id");
                        osaeImage.Name = reader.GetString("image_name");

                        imageList.Add(osaeImage);
                    }
                }
                catch (Exception e)
                {
                    // Exceptions should be caught by calling application
                    throw new Exception("API - GetImageList - Failed: " + e.Message, e);
                }
            }
            return imageList;
        }

        /// <summary>
        /// Gets all the images available in the DB
        /// </summary>
        /// <returns>returns the images available in the DB</returns>
        /// <remarks>Only call this method if you have to as retrieving all images can take a short while</remarks>
        public List<OSAEImage> GetImages()
        {
            List<OSAEImage> imageList = new List<OSAEImage>();

            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT * FROM osae_images", connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        OSAEImage osaeImage = new OSAEImage();
                        osaeImage.ID = reader.GetUInt32("image_id");
                        osaeImage.Name = reader.GetString("image_name");
                        osaeImage.Type = reader.GetString("image_type");
                        osaeImage.Data = (byte[])reader.GetValue(1);

                        imageList.Add(osaeImage);
                    }
                }
                catch (Exception e)
                {
                    // Exceptions should be caught by calling application
                    throw new Exception("API - GetImages - Failed: " + e.Message, e);
                }
            }
            
            return imageList;
        }

        public byte[] GetJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }

        public byte[] GetPNGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }

        public byte[] GetGIFFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            GifBitmapEncoder encoder = new GifBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }

        public byte[] GifToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public byte[] JpgToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        public byte[] PngToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
