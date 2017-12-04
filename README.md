# e-Manifest-Toy-Client
## Project Description
This is a toy .NET client for USEPA's e-Manifest Service API as updated in November 2017.  The client can authenticate, perform a GET for JSON data and/or .zip file attachments, and POST a manifest save with optional .zip file attachment.

See [USEPA's e-Manifest Github](https://github.com/USEPA/e-manifest) for info on the Program.

The cient just a toy proof of concept with no real error handling or user interface.  It might be helpful to get someone going on their own client code. I'm not intending to develop this publically any further, but use it freely as you see fit

## Set-up

The code is a C# "Solution" which was developed using Visual Studio 2015.  It should compile as-is, but it's possible you might need to add some .dll references.  You will need to put your own API id and API key in the app.config file to actually authenticate to USEPA's Services.  You can get your id and key by following the instructions on USEPA's site.


## Good Luck!

