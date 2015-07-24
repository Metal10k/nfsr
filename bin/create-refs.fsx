#load "..\lib\Resolver.fsx"
open System
open System.IO

let scripts = seq {
            for q in Resolver.getLocals() do
                match q with
                | Resolver.Library(path) -> 
                    yield path
                | _ -> ()
    } 
//turn tags into relative paths!

let makeRelativePath fromPath toPath =  //from http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
    let fromUri = Uri(fromPath)
    let toUri = Uri(toPath)

    if not (fromUri.Scheme = toUri.Scheme) then
        toPath
    else
        let relativeUri = fromUri.MakeRelativeUri(toUri)
        let relativePath = Uri.UnescapeDataString(relativeUri.ToString())
        if (toUri.Scheme.ToUpperInvariant() = "FILE") then
            relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
        else
            relativePath

let stripBase (path : string) = 
    let dirName = Path.GetFileName(System.Environment.CurrentDirectory)
    let shortenedArr = path.Split('\\') |> Seq.skip 1
    String.Join("\\", shortenedArr)


let fromPath = System.Environment.CurrentDirectory
let loadTags = scripts 
                |> Seq.map (makeRelativePath fromPath)
                |> Seq.map stripBase
                |> Seq.map (fun s -> ("#load \"" + s + "\""))

File.WriteAllLines("_References.fsx", loadTags)