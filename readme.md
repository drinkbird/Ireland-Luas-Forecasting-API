# Ireland Luas Forecasting API

A REST API that serves forecasting information about Luas trams in a simple JSON format.

Request format:
```
GET http://[hostname]/api/luas-forecast/[stopcode]
```

Example call:
```
Request:
http://localhost:49779/api/luas-forecast/PHI

Response:
{
   "stopName":"Phibsborough",
   "message":"Green Line services operating normally",
   "updatedOn":"2019-01-05T23:21:29",
   "inboundTrams":[
      {
         "destination":"Broombridge",
         "dueMins":6
      },
      {
         "destination":"Broombridge",
         "dueMins":19
      }
   ],
   "outboundTrams":[
      {
         "destination":"Bride's Glen",
         "dueMins":0
      },
      {
         "destination":"Bride's Glen",
         "dueMins":14
      }
   ]
}
```

Supported LUAS stops and their codes:
```
STX, St. Stephen's Green
HIN, Heuston
HCT, Heuston
TPT, The Point
SDK, Spencer Dock
MYS, Mayor Square NCI
GDK, George's Dock
CON, Connolly
BUS, Busáras
ABB, Abbey Street
JER, Jervis
FOU, Four Courts
SMI, Smithfield
MUS, Museum
HEU, Heuston
JAM, James's
FAT, Fatima
RIA, Rialto
SUI, Suir Road
GOL, Goldenbridge
DRI, Drimnagh
BLA, Blackhorse
BLU, Bluebell
KYL, Kylemore
RED, Red Cow
KIN, Kingswood
BEL, Belgard
COO, Cookstown
HOS, Hospital
TAL, Tallaght
FET, Fettercairn
CVN, Cheeverstown
CIT, Citywest Campus
FOR, Fortunestown
SAG, Saggart
DEP, Depot
BRO, Broombridge
CAB, Cabra
PHI, Phibsborough
GRA, Grangegorman
BRD, Broadstone DIT
DOM, Dominick
PAR, Parnell
OUP, O'Connell Upper
OGP, O'Connell GPO
MAR, Marlborough
WES, Westmoreland
TRY, Trinity
DAW, Dawson
STS, St. Stephen's Green
HAR, Harcourt
CHA, Charlemont
RAN, Ranelagh
BEE, Beechwood
COW, Cowper
MIL, Milltown
WIN, Windy Arbour
DUN, Dundrum
BAL, Balally
KIL, Kilmacud
STI, Stillorgan
SAN, Sandyford
CPK, Central Park
GLE, Glencairn
GAL, The Gallops
LEO, Leopardstown Valley
BAW, Ballyogan Wood
RCC, Racecourse
CCK, Carrickmines
BRE, Brennanstown
LAU, Laughanstown
CHE, Cherrywood
BRI, Bride's Glen
```

Project notes:
- developed using aspnet core and the F# language
- targets dotnet core 2.2 so it can be developed cross-platform
- easily extensible to serve more formats via content negotiation
- this project is unofficial, and it's based on the Transport Infrastructure Ireland's [Luas Forecasting API](https://data.gov.ie/dataset/luas-forecasting-api/resource/48d3cc1a-7c4e-42e4-a513-8ae117ff40b1) licensed under the [Creative Commons Attribution 4.0](https://creativecommons.org/licenses/by/4.0/).

Developed by Tasos Piotopoulos of [DrinkBird](http://drinkbird.com)
