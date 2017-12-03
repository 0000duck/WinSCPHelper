<img src="http://deepmirage.com/img/winscphelper.png" alt="WinSCPHelper" width="64px"/>

A library for uploading and downloading files from remote server via WinSCP Assembly Library. Extremely fast, flexible, and easy to use. WinSCPHelper works great on SFTP.

## NuGet Package

[WinSCP Helper](https://www.nuget.org/packages/WinSCPHelper/) - Actual WinSCP library published on NuGet<br/>

## Getting Started

- Right click on your project and select "Manage NuGet Package". Search for "WinSCPHelper". Install WinSCPHelper on your project.

- Below is a sample code for downloading files from a remote directory to your local folder

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinScpHelper;

namespace WinScpPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            WinScpConnection winScpConnection = new WinScpConnection("hostname", "sftpusername", "sftppassword");
            var currentSession = winScpConnection.Open();

            var currentList = winScpConnection.Download(currentSession, "/var/www/myremotedirectory",
                                        @"C:\mytargetfolder");

            //for verification - display names of downloaded file
            foreach (var file in currentList)
            {
                Console.WriteLine(file.FileName);
            }
        }
    }
}
```
- You can also download a single file by doing this

```c#
WinScpConnection winScpConnection = new WinScpConnection("hostname", "sftpusername", "sftppassword");
var currentSession = winScpConnection.Open();
winScpConnection.Download(currentSession, "/var/www/myremotedirectory/remotefile.jpg", @"C:\mytargetfolder\localfile.jpg");
```

- Here is a snippet in uploading files to your target remote directory

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinScpHelper;

namespace WinScpPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            WinScpConnection winScpConnection = new WinScpConnection("hostname", "sftpusername", "sftppassword");
            var currentSession = winScpConnection.Open();
            winScpConnection.Upload(currentSession, @"C:\localfolderforupload\*.*", "/var/www/myremotedirectory/*.*");
        }
    }
}
```
- Uploading a single file is also doable. Like so,

```c#
WinScpConnection winScpConnection = new WinScpConnection("hostname", "sftpusername", "sftppassword");
var currentSession = winScpConnection.Open();
winScpConnection.Upload(currentSession, @"C:\localfolderforupload\localfile.jpg", "/var/www/myremotedirectory/remotefile.jpg");
```

- That should be it.
