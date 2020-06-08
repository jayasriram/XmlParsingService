using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XMLParsingService.Utils;

namespace XMLParsingService
{
    /* Class to load the xml from specified folder location in App.Config
     * to parse and process the data based on the generator Type
     */
    class XMLDataProcessing
    {
        static void Main(string[] args)
        {
            string Path = System.Configuration.ConfigurationManager.AppSettings["InputPath"];
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);
         
            CalculationUtil calculationUtil = new CalculationUtil();
            XmlNode root = doc.DocumentElement;
            XmlNodeList nodeList = root.SelectNodes("*");

            foreach (XmlNode node in nodeList)
            {
                String name = node.Name;
                foreach (XmlNode subChildNode in node.ChildNodes)
                {
                    String generatorType = subChildNode["Name"].InnerText;
                    switch (name)
                    {
                        case "Wind":
                            foreach (XmlNode childNode in subChildNode.ChildNodes)
                            {                                
                                if (childNode.ChildNodes.Count > 1)
                                {
                                    calculationUtil.CalculateTotalGeneration(childNode, generatorType);
                                }
                            }
                            break;
                        case "Gas":
                            float coalEmissionRatingValue = float.Parse(subChildNode.LastChild.InnerText);                            
                            foreach (XmlNode childNode in subChildNode.ChildNodes)
                            {                                
                                if (childNode.ChildNodes.Count > 1)
                                {                                                             
                                    calculationUtil.CalculateTotalGeneration(childNode, generatorType);
                                    calculationUtil.CalculateEmissionRating(childNode, generatorType, coalEmissionRatingValue);
                                }
                            }
                            break;
                        case "Coal":
                            float totalHeatInput = float.Parse(subChildNode["TotalHeatInput"].InnerText);
                            float actualNetGeneration = float.Parse(subChildNode["ActualNetGeneration"].InnerText);
                            float gasEmissionRatingValue = float.Parse(subChildNode.LastChild.InnerText);
                            foreach (XmlNode childNode in subChildNode.ChildNodes)
                            {                              
                                if (childNode.ChildNodes.Count > 1)
                                {
                                    calculationUtil.CalculateTotalGeneration(childNode, generatorType);
                                    calculationUtil.CalculateEmissionRating(childNode, generatorType, gasEmissionRatingValue);
                                    calculationUtil.CalculateActualHeatRate(childNode, totalHeatInput, actualNetGeneration);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}


