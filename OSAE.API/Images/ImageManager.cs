namespace OSAE.API.Images
{
    using System;
    using MySql.Data.MySqlClient;    
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;

    public class ImageManager
    {
        OSAE osae = new OSAE("OSAE.API");

        /// <summary>
        /// Adds an image to the DB
        /// </summary>
        /// <param name="osaeImage">The image information to add</param>
        public void AddImage(OSAEImage osaeImage)
        {
            AddImage(osaeImage.Name, osaeImage.Type, osaeImage.Data);         
        }

        /// <summary>
        /// Adds an image to the OSAE DB
        /// </summary>
        /// <param name="name">The name of the image this should not include the path or extension</param>
        /// <param name="type">the type of the image e.g. jpg, png do not include the .</param>
        /// <param name="imageData">the binary data of the image</param>
        public void AddImage(string name, string type, byte[] imageData)
        {
            using (MySqlCommand command = new MySqlCommand())
            {                
                command.CommandText = "CALL osae_sp_image_add (@pimage_data, @pimage_name, @pimage_type)";
                command.Parameters.AddWithValue("@pimage_data", imageData);
                command.Parameters.AddWithValue("@pimage_name", name);
                command.Parameters.AddWithValue("@pimage_type", type);
                osae.RunQuery(command);
            }
        }

        /// <summary>
        /// Deletes an image from the DB
        /// </summary>
        /// <param name="imageId">The Id of the image to delete</param>
        public void DeleteImage(int imageId)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_image_delete (@pimage_id)";
                command.Parameters.AddWithValue("@pimage_id", imageId);               
                osae.RunQuery(command);
            }        
        }       

        /// <summary>
        /// Gets an image from the DB
        /// </summary>
        /// <param name="imageId">The id of the image to get</param>
        /// <returns>the requested image</returns>
        public OSAEImage GetImage(int imageId)
        {
            OSAEImage osaeImage = new OSAEImage();
            if (API.Common.TestConnection())
            {
                using (MySqlConnection connection = new MySqlConnection(API.Common.ConnectionString))
                {
                    MySqlCommand command = new MySqlCommand("SELECT * FROM osae_images WHERE image_id = " + imageId, connection);
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        osaeImage.ID = reader.GetUInt32("image_id");
                        osaeImage.Name = reader.GetString("image_name");
                        osaeImage.Type = reader.GetString("image_type");
                        osaeImage.Data = (byte[])reader.GetValue(1);                       
                    }
                    else
                    {
                        osae.AddToLog("API - Failed to get requested image from DB: ", true);
                    }
                }
            }

            return new OSAEImage();
        }

        /// <summary>
        /// Returns a list of available images without the type or data for performence reasons.
        /// </summary>
        /// <returns>a list of images</returns>
        public List<OSAEImage> GetImageList()
        {
            List<OSAEImage> imageList = new List<OSAEImage>();
            
            if (API.Common.TestConnection())
            {
                using (MySqlConnection connection = new MySqlConnection(API.Common.ConnectionString))
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
                        osae.AddToLog("API - GetImageList - Failed \r\n\r\n" + e.Message, true);
                    }
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

            if (API.Common.TestConnection())
            {
                using (MySqlConnection connection = new MySqlConnection(API.Common.ConnectionString))
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
                        osae.AddToLog("API - GetImages - Failed \r\n\r\n" + e.Message, true);
                    }
                }
            }
            return imageList;                
        }
    }
}
