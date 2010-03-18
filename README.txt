SINGER DISPATCH README
======================


Building instructions:
----------------------

Before building each project you must go into the folder 'Singer Data Importer' and
copy the file app.sample.config to app.config. Once the new file is in place you 
must edit the database details inside to match the database will be using. The same
must then be done inside the directory 'Singer Dispatch'.

One these config files have been created, you can open up the project in Visual 
Studio 2010 or later and compile it. 


Running instructions:
---------------------

Once the application has been compile, the 'Singer Data Importer' application MUST
be run first. This will create the database structure and import data from the old
MS Access database.

Once the importer is run, the 'Singer Dispatch' application is the main application
to use.


