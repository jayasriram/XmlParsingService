using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLParsingService.Utils
{
    /*Class to build the output xml in the output folder location
     */
    class XmlWriterUtil
    {
        /* Method to build xml in the specified output folder location and populate values
         * <param name="totalDict">Dictionary with total values</param>
         * <param name="dailyCoalEmissionDict">Dictionary with daily emission values for coal</param>
         * <param name="dailyGasEmissionDict">Dictionary with daily emission values for gas</param>
         */
        public void CreateDoc(IDictionary<string, float> totalDict,
                              IDictionary<string, float> dailyCoalEmissionDict,
                              IDictionary<string, float> dailyGasEmissionDict,float actHeatRate)
        {
            CalculationUtil calculationUtil = new CalculationUtil();            
            string outputPath = System.Configuration.ConfigurationManager.AppSettings["OutputPath"];            

            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
                                          new XElement("GenerationOutput",
                                            new XElement("Totals",totalDict.Select(d => new XElement("Generator",
                                            new XElement("name", d.Key),
                                            new XElement("total", d.Value)))),
                                            new XElement("test", "test"),
                                            new XElement("MaxEmissionGenerators", dailyCoalEmissionDict.Select(e => new XElement("Day",
                                            new XElement("Date", e.Key),
                                            new XElement("Emission", e.Value),
                                            new XElement("Name", "Coal[1]")))),                                            
                                            new XElement("ActualHeatRates",
                                            new XElement("Name", "Coal[1]"),
                                            new XElement("HeatRate",actHeatRate)
                                            )));
                     doc.Element("GenerationOutput").Element("MaxEmissionGenerators").Add(dailyGasEmissionDict.Select(g => new XElement("Day",
                                           new XElement("Date", g.Key),
                                            new XElement("Emission", g.Value),
                                            new XElement("Name", "Gas[1]"))));
            doc.Save(outputPath);            
        }
    }
}
