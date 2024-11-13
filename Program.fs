open System

let clearLines () =
    Console.SetCursorPosition(0, Console.CursorTop - 1)
    Console.Write(new string (' ', Console.WindowWidth - 1))
    Console.SetCursorPosition(0, Console.CursorTop)

let availableCommands = "Commands: [0..100] for volume, [q] to quit: "
let urlPrompt = "Please enter stream address (in http:// format) or [q] to quit: "

let isValidUrl (url: string) =
    url.StartsWith("http://")
    || url.StartsWith("https://")

let rec awaitingUrl () =
    let url = Console.ReadLine()
    clearLines ()

    match url with
    | "q" -> exit 0
    | s when isValidUrl s -> playStream (url)
    | _ ->
        clearLines ()
        Console.WriteLine("Last command entered: {0}", url)
        Console.Write(urlPrompt)
        awaitingUrl ()

and playStream (url: string) =
    try
        use reader = new NAudio.Wave.MediaFoundationReader(url)
        use waveOut = new NAudio.Wave.WaveOutEvent()
        waveOut.Init(reader)

        waveOut.PlaybackStopped.Add (fun args ->
            clearLines ()
            Console.WriteLine("")
            clearLines ()
            Console.WriteLine("Playback has ended :(")
            exit 0)

        waveOut.Play()

        clearLines ()
        Console.WriteLine("Playing: {0}", url)
        Console.Write(availableCommands)

        let rec processCommands () =
            let input = Console.ReadLine()
            clearLines ()

            match input with
            | "q" -> exit 0
            | s ->
                match Int32.TryParse(s) with
                | (true, vol) when vol >= 0 && vol <= 100 ->
                    let volume = float32 vol / 100.0f
                    waveOut.Volume <- volume

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
        awaitingUrl ()

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

    argv
    |> Array.tryHead
    |> Option.filter isValidUrl
    |> Option.iter playStream

    awaitingUrl ()

    0
