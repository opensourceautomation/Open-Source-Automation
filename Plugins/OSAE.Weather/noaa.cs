//-----------------------------------------------------------------------
// <copyright file="ObservationStations.cs" company="Eclectrics, LLC">
//     Copyright © 2009 Eclectrics, LLC. All rights reserved.
// </copyright>
//
// Free for all uses under the Creative Commons Attribution 3.0 United 
// Stated License.
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Xml;

namespace OSAE.WeatherPlugin
{

    /// <summary>
    /// Encapsulates information about an observation station.
    /// </summary>
    public class ObservationStation
    {
        /// <summary>
        /// Initializes a new instance of the ObservationStation class.
        /// </summary>
        public ObservationStation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObservationStation class from 
        /// the supplied XmlNode.
        /// </summary>
        /// <param name="stationNode">XmlNode describing the station</param>
        public ObservationStation(XmlNode stationNode)
        {
            this.InitializeFromXmlNode(stationNode);
        }

        /// <summary>
        /// Gets or sets the URL of current observations XML document.
        /// </summary>
        public string CurrentObsXmlUrl { get; set; }

        /// <summary>
        /// Gets or sets the station name.
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// Gets or sets the station id.
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// Gets or sets the observation station latitude.
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the observation station longitude.
        /// </summary>
        public decimal Longitude { get; set; }

        

        /// <summary>
        /// Initialize the ObservationStation property values from the supplied Xml element.
        /// </summary>
        /// <param name="stationNode">XmlNode describing the observation station</param>
        private void InitializeFromXmlNode(XmlNode stationNode)
        {
            if (stationNode.HasChildNodes)
            {
                

                XmlNode nameNode = stationNode.SelectSingleNode("station_name");
                if (nameNode != null)
                {
                    this.StationName = nameNode.InnerText;
                }

                XmlNode idNode = stationNode.SelectSingleNode("station_id");
                if (idNode != null)
                {
                    this.StationId = idNode.InnerText;
                }

                XmlNode xmlUrlNode = stationNode.SelectSingleNode("xml_url");
                if (xmlUrlNode != null)
                {
                    this.CurrentObsXmlUrl = xmlUrlNode.InnerText;
                }

                

                XmlNode latNode = stationNode.SelectSingleNode("latitude");
                if (latNode != null)
                {
                    decimal latitude;
                    if (decimal.TryParse(latNode.InnerText, out latitude))
                    {
                        this.Latitude = latitude;
                    }
                }

                XmlNode lonNode = stationNode.SelectSingleNode("longitude");
                if (lonNode != null)
                {
                    decimal longitude;
                    if (decimal.TryParse(lonNode.InnerText, out longitude))
                    {
                        this.Longitude = longitude;
                    }
                }
            }
        }
    }

   
}
