# Introduction
It is so embrassing that the text search feature in Microsoft OneNote is crappy! OneNote just do a text matching over all the pages and return the exact result to handle the query. That's why I want to use Lucene.NET to enhance it.  

# Prerequisite
Lucene.Net 3.0.3  
Visual Studio 2017

# Manual
## Installation
Please download latest release file and unzip it. Then click `setup.exe` to install.
## First start
You can find this application in Start menu. Click to launch. You will see a simple user interface.
Click `Index` button and wait, it will index all page content for you.
![Alt text](docs/images/index.jpg?raw=true "Title")
## Search
After finish indexing, type anything you want in search box and click `Search` button. It will accept your query and return top 10 relevant paragraph.
![Alt text](docs/images/search.jpg?raw=true "Title")
You can navigate to page where the paragraph from, just double click the row. The target page will display in OneNote.
