# BORGER DISPATCH README


### Building instructions:

Before building each project you must go into the folder 'Borger Data Importer' and
copy the file app.sample.config to app.config. Once the new file is in place you
must edit the database details inside to match the database will be using. The same
must then be done inside the directory 'Borger Dispatch'.

Once these config files have been created, you can open up the project in Visual
Studio 2010 or later and compile it.


### Debug instructions:

Once the application has been compile, the 'Borger Data Importer' application MUST
be run first on the development machine. This will create the database structure and
import some text data from the old MS Access database.

Once the importer is run, the 'Borger Dispatch' application is the main application
to use.

DO NOT RUNN THIS TOOL AGAINST THE LIVE DATABASE! DOING SO WILL DELETE EVERYTHING!


### Pushing changes out:

Before pushing out any changes, make sure you change the database settings in the file
'app.config' to point to the production database. Once this is done, select the 'Borger
Dispatch' project from the solution, then go to Build->Publish Borger Dispatch in the menu.
Click on Finish when the dialog opens, and that's it.
