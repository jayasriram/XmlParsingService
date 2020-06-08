using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLParsingService.Utils
{
    /*Class to process the xml data and calculate total generation values ,
     * daily emission values & actual heat rate values for all the generators
     */
    public class CalculationUtil
    {
        public IDictionary<string, float> genTotalsDictionary { get; } = new Dictionary<string, float>();
        public IDictionary<string, float> dailyCoalEmissionDictionary { get; } = new Dictionary<string, float>();
        public IDictionary<string, float> dailyGasEmissionDictionary { get; } = new Dictionary<string, float>();

        /* Method to load the document from specified folder location in App.Config
         */
        public XmlNode LoadDoc()
        {
            XmlDocument doc = new XmlDocument();
            string referenceDataPath = System.Configuration.ConfigurationManager.AppSettings["ReferenceDataPath"];
            doc.Load(referenceDataPath);
            XmlNode root = doc.DocumentElement;
            return root;
        }


        /* Method to calculate total generation value for each generator
         * <param name="generationNode">Generation node from the input xml</param>
         * <param name="generatorType">Generator Type such as Wind,Coal & Gas</param>         
         */
        public void CalculateTotalGeneration(XmlNode generationNode, String generatorType)
        {
            float dailyGenerationValue;
            XmlNode root = LoadDoc();
            XmlNode valuefactor = root.SelectSingleNode("//ValueFactor");        
             
            
            float offshoreTotalDailyGenerationValue = 0.0F;
            float onshoreTotalDailyGenerationValue = 0.0F;
            float gasTotalDailyGenerationValue = 0.0F;
            float coalTotalDailyGenerationValue = 0.0F;

            Console.WriteLine("Inside Utils......" + generationNode);
            if (generatorType.Contains("Offshore"))
            {
                foreach (XmlNode dayNode in generationNode)
                {
                    dailyGenerationValue = ((float.Parse(dayNode["Energy"].InnerText)) *
                                       (float.Parse(dayNode["Price"].InnerText)) *
                                       (float.Parse(valuefactor["Low"].InnerText)));
                    offshoreTotalDailyGenerationValue = dailyGenerationValue + offshoreTotalDailyGenerationValue;
                }
                genTotalsDictionary.Add(generatorType, offshoreTotalDailyGenerationValue);
            }            
            else if (generatorType.Contains("Onshore"))
            {
                foreach (XmlNode dayNode in generationNode)
                {
                    dailyGenerationValue = ((float.Parse(dayNode["Energy"].InnerText)) *
                                            (float.Parse(dayNode["Price"].InnerText)) *
                                            (float.Parse(valuefactor["High"].InnerText)));
                    onshoreTotalDailyGenerationValue = dailyGenerationValue + onshoreTotalDailyGenerationValue;
                }
                genTotalsDictionary.Add(generatorType, onshoreTotalDailyGenerationValue);
            }
            else if (generatorType.Contains("Gas"))
            {
                foreach (XmlNode dayNode in generationNode)
                {
                    dailyGenerationValue = ((float.Parse(dayNode["Energy"].InnerText)) *
                                           (float.Parse(dayNode["Price"].InnerText)) *
                                         (float.Parse(valuefactor["Medium"].InnerText)));
                    gasTotalDailyGenerationValue = dailyGenerationValue + gasTotalDailyGenerationValue;
                }
                genTotalsDictionary.Add(generatorType, gasTotalDailyGenerationValue);
            }
            else if (generatorType.Contains("Coal"))
            {
                foreach (XmlNode dayNode in generationNode)
                {
                    dailyGenerationValue = ((float.Parse(dayNode["Energy"].InnerText)) *
                                       (float.Parse(dayNode["Price"].InnerText)) *
                                     (float.Parse(valuefactor["Medium"].InnerText)));
                    coalTotalDailyGenerationValue = dailyGenerationValue + coalTotalDailyGenerationValue;
                }
                genTotalsDictionary.Add(generatorType, coalTotalDailyGenerationValue);
            }

        }

        /* Method to calculate emission rating value for fossil fuel generators
        * <param name="generationNode">Generation node from the input xml</param>
        * <param name="generatorType">Generator Type such as Wind,Coal & Gas</param>         
        * <param name="emissionRating">Emission Rating Value from input xml</param>    
        */
        public void CalculateEmissionRating(XmlNode generationNode,string generationType, float emissionRating)
        {
            XmlNode root = LoadDoc();
            XmlNode emissionfactor = root.SelectSingleNode("//EmissionsFactor");
            float dailyEmissionValue;
            foreach (XmlNode dayNode in generationNode)
            {
                if (generationType.Contains("Gas")) {
                    dailyEmissionValue = ((float.Parse(dayNode["Energy"].InnerText)) *
                                           emissionRating *
                                           (float.Parse(emissionfactor["Medium"].InnerText)));
                    if (dailyEmissionValue > 0) {
                        dailyGasEmissionDictionary.Add(dayNode["Date"].InnerText, dailyEmissionValue);
                    }
                }
                else if (generationType.Contains("Coal")) {
                    dailyEmissionValue = ((float.Parse(dayNode["Energy"].InnerText)) *
                                          emissionRating *
                                          (float.Parse(emissionfactor["High"].InnerText)));
                    if (dailyEmissionValue > 0)
                    {
                        dailyCoalEmissionDictionary.Add(dayNode["Date"].InnerText, dailyEmissionValue);
                    }
                }
            }
        }

        /* Method to calculate actual heat rate value and pass values to XmlWrite to build the xml
        * <param name="generationNode">Generation node from the input xml</param>
        * <param name="totalHeatInput">Total Heat Input value from input xml</param>         
        * <param name="actualNetGeneration">Actual Net Generation Value from input xml</param>    
        */
        public void CalculateActualHeatRate(XmlNode generationNode, float totalHeatInput, float actualNetGeneration) {
            float actualHeatRate;
            actualHeatRate = totalHeatInput / actualNetGeneration;
            XmlWriterUtil xmlwriter = new XmlWriterUtil();
            xmlwriter.CreateDoc(genTotalsDictionary,dailyCoalEmissionDictionary,dailyGasEmissionDictionary, actualHeatRate);

        }
    }
}
