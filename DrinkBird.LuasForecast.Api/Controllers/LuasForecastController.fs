namespace DrinkBird.LuasForecast.Api.Controllers

open System
open System.Xml.Serialization
open System.Net.Http
open Microsoft.AspNetCore.Mvc

[<XmlRoot(ElementName="tram"); CLIMutable>]
type Tram =
    {
        [<XmlAttribute(AttributeName="dueMins")>]
        DueMins : string
        [<XmlAttribute(AttributeName="destination")>]
        Destination : string
    }

[<XmlRoot(ElementName="direction"); CLIMutable>]
type Direction =
    {
        [<XmlElement(ElementName="tram")>]
        Tram : Tram[]
        [<XmlAttribute(AttributeName="name")>]
        Name : string
    }

[<XmlRoot(ElementName="stopInfo"); CLIMutable>]
type StopInfo =
    {
        [<XmlElement(ElementName="message")>]
        Message : string
        [<XmlElement(ElementName="direction")>]
        Direction : Direction[]
        [<XmlAttribute(AttributeName="created")>]
        Created : string
        [<XmlAttribute(AttributeName="stop")>]
        Stop : string
        [<XmlAttribute(AttributeName="stopAbv")>]
        StopAbv : string
    }

type ILuasForecastService =
    abstract GetForecastFor: stopCode: string -> StopInfo option

module LuasForecastService =
    open System.IO

    let stops =
        dict[
            "STX", "St. Stephen's Green"
            "HIN", "Heuston"
            "HCT", "Heuston"
            "TPT", "The Point"
            "SDK", "Spencer Dock"
            "MYS", "Mayor Square NCI"
            "GDK", "George's Dock"
            "CON", "Connolly"
            "BUS", "Busáras"
            "ABB", "Abbey Street"
            "JER", "Jervis"
            "FOU", "Four Courts"
            "SMI", "Smithfield"
            "MUS", "Museum"
            "HEU", "Heuston"
            "JAM", "James's"
            "FAT", "Fatima"
            "RIA", "Rialto"
            "SUI", "Suir Road"
            "GOL", "Goldenbridge"
            "DRI", "Drimnagh"
            "BLA", "Blackhorse"
            "BLU", "Bluebell"
            "KYL", "Kylemore"
            "RED", "Red Cow"
            "KIN", "Kingswood"
            "BEL", "Belgard"
            "COO", "Cookstown"
            "HOS", "Hospital"
            "TAL", "Tallaght"
            "FET", "Fettercairn"
            "CVN", "Cheeverstown"
            "CIT", "Citywest Campus"
            "FOR", "Fortunestown"
            "SAG", "Saggart"
            "DEP", "Depot"
            "BRO", "Broombridge"
            "CAB", "Cabra"
            "PHI", "Phibsborough"
            "GRA", "Grangegorman"
            "BRD", "Broadstone DIT"
            "DOM", "Dominick"
            "PAR", "Parnell"
            "OUP", "O'Connell Upper"
            "OGP", "O'Connell GPO"
            "MAR", "Marlborough"
            "WES", "Westmoreland"
            "TRY", "Trinity"
            "DAW", "Dawson"
            "STS", "St. Stephen's Green"
            "HAR", "Harcourt"
            "CHA", "Charlemont"
            "RAN", "Ranelagh"
            "BEE", "Beechwood"
            "COW", "Cowper"
            "MIL", "Milltown"
            "WIN", "Windy Arbour"
            "DUN", "Dundrum"
            "BAL", "Balally"
            "KIL", "Kilmacud"
            "STI", "Stillorgan"
            "SAN", "Sandyford"
            "CPK", "Central Park"
            "GLE", "Glencairn"
            "GAL", "The Gallops"
            "LEO", "Leopardstown Valley"
            "BAW", "Ballyogan Wood"
            "RCC", "Racecourse"
            "CCK", "Carrickmines"
            "BRE", "Brennanstown"
            "LAU", "Laughanstown"
            "CHE", "Cherrywood"
            "BRI", "Bride's Glen"
        ]

    let getForecastUrlFor stopCode = 
        if stops.ContainsKey stopCode then
            Some <| sprintf "http://luasforecasts.rpa.ie/xml/get.ashx?action=forecast&stop=%s&encrypt=false" stopCode
        else None

    let getForecast (httpClient : HttpClient) stopCode =
        getForecastUrlFor stopCode
        |> Option.map (fun url ->
            let resultXml = httpClient.GetStringAsync(url).Result
            use textReader = new StringReader(resultXml)
            let serializer : XmlSerializer = new XmlSerializer(typeof<StopInfo>)
            let stopInfo = serializer.Deserialize textReader :?> StopInfo
            stopInfo
        )

type TramDto =
    {
        destination : string
        dueMins : int
    }
with
    static member FromServiceType (tram : Tram) : TramDto =
        {
            destination = tram.Destination
            dueMins =
                match Int32.TryParse tram.DueMins with
                | true, mins -> mins
                | false, _ -> 0
        }

type StopInfoDto =
    {
        stopName : string
        message : string
        updatedOn : DateTime
        InboundTrams : TramDto[]
        OutboundTrams : TramDto[]
    }
with
    static member FromServiceType (stopInfo : StopInfo) : StopInfoDto =
        let mapDirection direction =
            stopInfo.Direction
            |> Array.tryFind (fun entry -> entry.Name = direction)
            |> Option.map (fun entry -> entry.Tram |> Array.map TramDto.FromServiceType)
            |> Option.defaultValue [||]

        {
            stopName = stopInfo.Stop
            message = stopInfo.Message
            updatedOn = stopInfo.Created |> DateTime.Parse
            InboundTrams = mapDirection "Inbound"
            OutboundTrams = mapDirection "Outbound"
        }

[<Route("api/luas-forecast")>]
[<ApiController>]
type LuasForecastController (luasForecastService : ILuasForecastService) =
    inherit ControllerBase()

    [<HttpGet("{stopCode}")>]
    member this.Get(stopCode: string) =
        match luasForecastService.GetForecastFor stopCode with
        | None -> this.NotFound() :> IActionResult
        | Some stopInfo ->
            let stopInfoDto = stopInfo |> StopInfoDto.FromServiceType
            this.Ok(stopInfoDto) :> IActionResult
