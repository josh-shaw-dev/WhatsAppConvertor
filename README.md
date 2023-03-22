# WhatsAppConvertor
Converts unencrypted whats app message and contact db to text, json and html

You will need the unencrypted message and contact db, this does not deal with decryption

Requirements:
dotnet runtime that supports net 6.0

How to use:
Open up a terminal
cd WhatsAppConvertor
dotnet run

Defaults are shown below
--Export:Json true
--Export:Text true
--Export:Html true
--Export:Directory "./Output"

--WaDatabase:ConnectionString "Data Source=./wa.db"

--MessageDatabase:ConnectionString "Data Source=./messages.decrypted.db"

#TODO
 - Add a release to github, bundle single file
 - Scrollable windows
 - Better looking css
 - Media links to open in new tab
 - Save files to individual folders for easy sharing with one person
   - Also have option to copy media
 - Write tests