# File2

A tool to manipulate files

## features

* aggregate files to one folder
* scan folder for file size info, so you can clean up disk

## todo

* [x] sum up folder size and sort them
* [x] showing progress message
* [x] support cancellation for aggregating
  * [x] graceful cancellation
  * [ ] immediate cancellation/abort current moving - kill the task and remove unfinished target file
* [x] what if user close program in the middle of moving file? redo moving next time? - yes
* [x] stopwatch for aggregating task
* [ ] hide cancel buttons by default
* [x] minor issues
  * [ ] aggregating progress msg too long when more than 1k files
  * [x] support multi-language
    * [x] English
    * [ ] Chinese
  * [ ] fix fixme, todo
  * [ ] message label overlap with button when main form shrink
    * [ ] use textbox(readonly mode) to replace label?
