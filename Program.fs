open System
open NAudio.Wave

[<EntryPoint>]
let main argv =
    Console.Write("Please enter stream address: ")
    let url = Console.ReadLine()

    printfn "Now playing stream from %s" url

    try
        use reader = new MediaFoundationReader(url)
        use waveOut = new WaveOutEvent()
        waveOut.Init(reader)
        waveOut.Play()

        Console.WriteLine("Press any key to stop playback...")
        Console.ReadKey() |> ignore

        waveOut.Stop()
    with
    | ex -> Console.WriteLine("An error occurred: {0}", ex.Message)

    0
