using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using eManifestServicesClient;

namespace eManifestServicesConsole
{
    class eManifestTestServicesConsole
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
            Console.WriteLine("Hit any key to end.");
            Console.ReadKey();
        }

        static async Task MainAsync()
        {

            // These JSON files were provided by USEPA
            string manifestJsonFileWithAttachment = "../../TestDocs/emanifest-save-valid-with-file-example.json";
            string manifestJsonFileNoAttachment = "../../TestDocs/emanifest-save-valid-no-file-example.json";
            string manifestJsonfileWithErrors = "../../TestDocs/emanifest-save-invalid-example.json";

            string manifestImageFile = "../../TestDocs/Test Manifest Image.pdf";
            string knownManifestTrackingNumber = "100002343ELC";
            eManifestServices client = new eManifestServices();

            Console.WriteLine("Test authenticate:\n");
            Console.WriteLine(await client.authenticate());

            //Console.WriteLine(DateTime.Now);
            //Console.WriteLine("Hit any key to resume - wait 20 minutes to test timeout");
            //Console.ReadKey();

            Console.WriteLine("Test get hazard classes:\n");
            Console.WriteLine(await client.getHazardClasses());

            Console.WriteLine("Test save a manifest with errors:\n");
            String errorManifest = File.ReadAllText(manifestJsonfileWithErrors);
            Console.WriteLine(await client.saveManifest(errorManifest, null));

            Console.WriteLine("Test save manifest with no attachments:\n");
            String dataOnlyManifest = File.ReadAllText(manifestJsonFileNoAttachment);
            Console.WriteLine(await client.saveManifest(dataOnlyManifest, null));

            Console.WriteLine("Test save manifest with an attachment:\n");
            String manifestAndAttachment = File.ReadAllText(manifestJsonFileWithAttachment);
            Console.WriteLine(await client.saveManifest(manifestAndAttachment, manifestImageFile));

            Console.WriteLine("Test download manifest json and zipped attachments:\n");
            Console.WriteLine(await client.eManifestAttachment(knownManifestTrackingNumber, "../../TestDocs/"));
        }
    }
}