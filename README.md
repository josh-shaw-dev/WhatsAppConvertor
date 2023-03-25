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
--Export:Json:Enabled true
--Export:Text:Enabled true
--Export:Html:Enabled true
--Export:Html:MediaPath ""
--Export:Html:CopyMedia false

--Export:Directory "./Output"

--WaDatabase:ConnectionString "Data Source=./wa.db"

--MessageDatabase:ConnectionString "Data Source=./messages.decrypted.db"

Media copy:
It is expected that path contains the media folder found here
Internal Storage/Android/media/com.whatsapp/WhatsApp/Media

--Export:Html:MediaPath "./File/Path/"

#TODO
 - Add a release to github, bundle single file
 - Scrollable windows
 - Better looking css
 - Make mobile friendly
 - Write tests