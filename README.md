# Introduction
It is so embrassing that the text search feature in Microsoft OneNote is crappy! OneNote just do a text matching over all the pages and return the exact result to handle the query. That's why I want to use Lucene.NET to enhance it.  
This application aims to reimplement all functionallity of [OneNote Search Bar](https://www.onenotegem.com/onenote-search-bar.html)  

# Prerequisite
Lucene.Net 3.0.3  
Visual Studio 2017

# Manual
## Installation
Please download latest release file and unzip it. Then click `setup.exe` to install.
## First start
You can find this application in Start menu. Click to launch. You will see a simple user interface.
Click `Index` button and wait, it will index all page content for you.

![Alt text](docs/images/Index.jpg "Title")
## Search
After finish indexing, type anything you want in search box and click `Search` button. It will accept your query and return top 10 relevant paragraph.

![Alt text](docs/images/Search.jpg "Title")  
You can navigate to page where the paragraph from, just double click the row. The target page will display in OneNote.

# Roadmap
- [x] Search and List Pages: Search pages in OneNote, and list pages.
- [ ] Search and List Paragraphs: Search and list paragraphs and highlight the keyword.
- [ ] Search and List Paragraphs with Tag Icons: If search result is a tag paragraph, "Search Bar" list this paragraph with its tag icon.
- [ ] Search and List Paragraphs with Heading Icons: If search result is a heading paragraph, "Search Bar" list this paragraph with heading icon ( 1 - 6 ).
- [x] Search and List Image OCR Text
- [ ] List all Unindexed Pages: List all the unindexed pages in OneNote notebooks.
