// Step 1: Set the SOURCE Directory
string sourceDirectoryPath = GetPath("Set the SOURCE Directory");

// Step 2: Set the DESTINATION directory
string destinationDirectoryPath = GetPath("Set the DESTINATION Directory");

// Step 3: Read files
var files = CustomEnumeration(sourceDirectoryPath);

// Step 4: Set destination
var filesWtihDestination = SetDirectoriesPathsToDestination(files, destinationDirectoryPath);

// Step 5: Confirmation
bool isConfirmationValid = false;
while (!isConfirmationValid)
{
    PrintMenu(
        "Confirm",
        $"Files found:  {files.Count}\n" +
        $"From:         {sourceDirectoryPath}\n" +
        $"To:           {destinationDirectoryPath}",
        $"\n[y] to confirm\n[x] to cancel\n"
    );
    Console.WriteLine("-> ");
    string confirmationResult = Console.ReadLine()!;

    if (string.Equals(confirmationResult, "y") || string.Equals(confirmationResult, "x"))
    {
        isConfirmationValid = true;        
    } 
    
    // Stop the program
    if (string.Equals(confirmationResult, "x"))
        return;
}

// Step 6: Proceed the files
foreach (var file in filesWtihDestination)
{
    if (!Directory.Exists(file.DestinationPath))
    {
        Directory.CreateDirectory(file.DestinationPath!);
    }

    File.Copy(file.Sourcepath, file.DestinationFile!);
}

void PrintMenu(string header, string body, string? footer)
{
    Console.Clear();
    Console.WriteLine(
            "##############################################################\n" +
            $"{header}\n"+
            "##############################################################\n\n" +
            $"{body}\n" +
            "##############################################################\n" 
        );

    if (footer is not null)
        Console.WriteLine(
            
            $"{footer}\n" +
            "##############################################################\n"
        );
}

List<MyFile> CustomEnumeration(string path)
{
    // Get a list o strings with the paths of the files
    var filesPaths = Directory.EnumerateFiles(path);

    // Building a list with all the information that I need
    var filesInfo = new List<MyFile>();
    foreach (string file in filesPaths)
    {
        string fileName = file.Substring(file.LastIndexOf("\\"));
        DateTime lastWriteAt = File.GetLastWriteTime(file);
        filesInfo.Add(new MyFile
        {
            FileName = fileName,    
            Sourcepath = file,
            Date = lastWriteAt,
            Processed = false
        });
    }

    // Order the list
    var filesInfoOrdered = filesInfo.OrderBy(x => x.Date).ToList();
    return filesInfoOrdered;
}

string GetDrive(string header)
{
    bool isDriveReady = false;
    string drive = "";

    var allDrives = DriveInfo.GetDrives();

    while (!isDriveReady)
    {
        int driveChosed = 0;

        string body = "";
        int i = 1;
        foreach (var dr in allDrives)
            body += $"[{i++}] {dr.Name}\n";
        PrintMenu(header, body, null);

        try
        {
            Console.Write("->");
            driveChosed = int.Parse(Console.ReadLine()!);
        }
        catch (Exception)
        {
            Console.WriteLine("The option is not valid");
            continue;
        }

        if (driveChosed <= 0 || driveChosed > allDrives.Length)
        {
            Console.WriteLine("The option is not valid");
            continue;
        }

        drive = allDrives.ElementAt(driveChosed - 1).Name;
        isDriveReady = true;
    }

    return drive;
}

string GetPath(string header)
{
    bool ready = false;
    string path = "";
    string footer = $"[y] to confirm: {path}\n[b] to back\n";

    while (!ready)
    {
        string optionChosed;

        if (path == "")
            path = GetDrive(header);

        var directories = Directory.EnumerateDirectories(path);
        
        if (directories.Any())
        {
            int i = 1;
            string body = "";
            foreach (var d in directories)
                body += $"[{i++}] {d.Substring(d.LastIndexOf("\\") + 1)}\n";
            
            PrintMenu(header, body, footer);
        }
        else
        {
            var files = CustomEnumeration(path);
            PrintMenu(header, $"{files.Count} found\n", footer);
        }

        Console.Write("->");
        optionChosed = Console.ReadLine()!;

        // Case BACK
        if (string.Equals(optionChosed, "b"))
        {
            // When change drive
            if (Directory.GetParent(path) is null)
            {
                path = "";
                continue;
            }

            // Back directoy (remove everyting after the last "/" to back one directory)
            char[] chars = path.ToCharArray();
            Array.Reverse(chars);
            string pathReversed = new string(chars);

            int firstOcurrence = pathReversed.IndexOf("\\");
            string resultReversed = pathReversed.Substring(firstOcurrence + 1);

            char[] charsResult = resultReversed.ToCharArray();
            Array.Reverse(charsResult);
            string newPath = new string(charsResult);

            path = newPath;
            continue;   
        }

        // Case CONFIRM
        if (string.Equals(optionChosed, "y"))
        {
            ready = true;
            continue;
        }

        // Case NAVIGATE
        // Check if the option is a valid number
        int opInt = -1;
        int.TryParse(optionChosed, out opInt);

        if (opInt <= 0)
            continue;

        try
        {
            var dir = directories.ElementAt(opInt - 1);
            path = dir;
        }
        catch
        {
            continue;
        }
    }

    return path;
}

List<MyFile> SetDirectoriesPathsToDestination(List<MyFile> files, string destinationDirectory)
{
    // SPANISH MONTHS
    var spanisMonts = new Dictionary<int, string>()
    {
        { 1, "Enero" },
        { 2, "Febrero" },
        { 3, "Marzo" },
        { 4, "Abril" },
        { 5, "Mayo" },
        { 6, "Junio" },
        { 7, "Julio" },
        { 8, "Agosto" },
        { 9, "Septiembre" },
        { 10, "Octubre" },
        { 11, "Noviembre" },
        { 12, "Diciembre" }
    };

    // ENGLISH MONTHS
    var englishMonths = new Dictionary<int, string>()
    {
        { 1, "January" },
        { 2, "February" },
        { 3, "March" },
        { 4, "April" },
        { 5, "May" },
        { 6, "June" },
        { 7, "July" },
        { 8, "August" },
        { 9, "September" },
        { 10, "October" },
        { 11, "November" },
        { 12, "December" }
    };

    // Choose the language for the name of the folders
    var months = spanisMonts;

    foreach (var file in files)
    {
        var month = "";
        months.TryGetValue(file.Date.Month, out month);

        var year = file.Date.Year;

        file.DestinationPath = $"{destinationDirectory}\\{year} {month}";
        file.DestinationFile = $"{destinationDirectory}\\{year} {month}{file.FileName}";

    }

    return files;
}

class MyFile
{
    public required string FileName { get; set; }
    public required string Sourcepath {  get; set; }
    public required DateTime Date { get; set; }
    public required bool Processed { get; set; }
    public string? DestinationPath { get; set; }
    public string? DestinationFile { get; set; }
}