open System
open LibVLCSharp.Shared

let clearLines () =
    Console.SetCursorPosition(0, Console.CursorTop - 1)
    Console.Write(new string (' ', Console.WindowWidth - 1))
    Console.SetCursorPosition(0, Console.CursorTop)

let availableCommands = "Commands: [0..100] for volume, [q] to quit: "
let urlPrompt = "Please enter stream address (in http:// format) or [q] to quit: "

let isValidUrl (url: string) =
    url.StartsWith("http://")
    || url.StartsWith("https://")

let rec awaitingUrl (libVLC: LibVLC) =
    let url = Console.ReadLine()
    clearLines ()

    match url with
    | "q" -> exit 0
    | s when isValidUrl s -> playStream libVLC url
    | _ ->
        clearLines ()
        Console.WriteLine("Last command entered: {0}", url)
        Console.Write(urlPrompt)
        awaitingUrl libVLC

and playStream (libVLC: LibVLC) (url: string) =
    try
        use mediaPlayer = new MediaPlayer(libVLC)
        let media = new Media(libVLC, Uri(url))
        mediaPlayer.Media <- media

        mediaPlayer.Play() |> ignore

        clearLines ()
        Console.WriteLine("Playing: {0}", url)
        Console.Write(availableCommands)

        let rec processCommands () =
            let input = Console.ReadLine()
            clearLines ()

            match input with
            | "q" ->
                mediaPlayer.Stop()
                exit 0
            | s ->
                match Int32.TryParse(s) with
                | (true, vol) when vol >= 0 && vol <= 100 ->
                    let volume = vol
                    mediaPlayer.Volume <- volume

                    clearLines ()
                    Console.WriteLine("Playing: {0}, Volume: {1}%", url, vol)
                    Console.Write(availableCommands)
                    processCommands ()
                | _ ->
                    clearLines ()
                    Console.WriteLine("Last command entered: {0}", input)
                    Console.Write(availableCommands)
                    processCommands ()

        processCommands ()

    with
    | ex ->
        clearLines ()
        Console.WriteLine("Error: {0}", ex.Message)
        Console.Write(urlPrompt)
        awaitingUrl libVLC

[<EntryPoint>]
let main argv =
    Console.Clear()
    Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗")
    Console.WriteLine("║         ░░░░░░  ▄█▒░░     ▄▄█▀▀   ▄▄█▓▓▓▄ ░░░░░    ▄▓▓▓▓▓▓▓▓█▀    ░░░░░     ░║")
    Console.WriteLine("║        ░░░░░  ▄█▓▒▒    ▄▄█▓▒    ▄█▓▓▒▒▒▓▓▄ ░░   ░▓▓▓▓▓▓▓▓▓░░    ░░░░░  ▓▓█  ░║")
    Console.WriteLine("║       ░░░░  ▄▓▓▒▒   ▄▄█▓▓▒░   ▄▓▓▓▓▒░░░░▓▓▄   ░░▓▓▓▒░░░░░░░    ░░░░  ▄▓▓▓█  ░║")
    Console.WriteLine("║      ░░░░  ▓▒▒░   ▒▓▓▓     ▄█▓▓▓▒▒░    ░░▓▓█  ░░░░░    ░░░   ░░░░  ▄▓▓▓▓▓   ░║")
    Console.WriteLine("║     ░░░  ░▓▓▒░ ▒▒▓▓▓▒    ░░▒▓▓▓▒░        ░▓▓░  ░░   ░░▒▒░   ░░░   ▓▓▓▓▓▓█  ░░║")
    Console.WriteLine("║    ░░░  ▒▓▓▒░  ▒▓▓▒░   ░░▒▓▓▓▒░     ░░░   ░▓▓░     ░▒▒▓▓   ░░░  ▓▓▓▓▓▓▓█  ░░░║")
    Console.WriteLine("║   ░░   ▒▓▓▓▒░   ░░▓▓▒▒   ░░▓▓▒    ░░░░░░░  ░▓░░    ▒▒▓▓    ░  ░▓▓▓▓▓▓▓▓▀  ░░ ║")
    Console.WriteLine("║  ░░  ▒▒▓▓▓▒▒░     ░░▓▒▒░   ░▓▓▓    ░░░░░  ░▓▓░   ░▒▒▓▓      ░▓▓▓▓▒░░    ░░░  ║")
    Console.WriteLine("║ ░░  ▒▓▓▓▓▓▒▒░  ░░  ░░▓▓▓▓    ░▒▓▒   ░░   ▓▓▓░   ░▒▓▓▓     ░░▓▓▓▓▒░   ░░░░░   ║")
    Console.WriteLine("║░░  ▒▓▓▓▒▒░░░░  ░░░░  ░░▓▓▓░    ▒▒▒░    ▓▓▒▒   ░░▒▓▓░    ░░▓▓▓▒░░   ░░░░░░    ║")
    Console.WriteLine("║░  ▒▒▒▒▒░░      ░░░░░░  ░▒▒▓▓░    ░▒▒▒░░▒▒▒   ░░▒▓▓░     ░▓▓▓░░   ░░░░░░░     ║")
    Console.WriteLine("║░ ░▒░░        ░░░░░░░░░  ░▒▒▓░░    ░▒░░░░    ░▒▓▓░░    ░▓▓░░  ░░░░░░░░░░      ║")
    Console.WriteLine("║ ░░░      ░░░░░░░░░░░░░░░   ░░▒▒░░    ░░░    ░▒▓▓░   ░░ ░▒░  ░░░░░░░░░░       ║")
    Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝")
    Console.WriteLine("")
    Console.Write("Please enter stream address or [q] to quit: ")

    Core.Initialize()
    let options = [| "--quiet"; "--no-video" |]
    use libVLC = new LibVLC(options)

    argv
    |> Array.tryHead
    |> Option.filter isValidUrl
    |> Option.iter (playStream libVLC)

    awaitingUrl libVLC

    0
